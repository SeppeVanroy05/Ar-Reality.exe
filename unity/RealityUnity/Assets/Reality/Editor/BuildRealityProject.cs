using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using Vuforia;

// Hulpmiddel om de beginscènes op te bouwen. Daarna kan je alles gewoon in de editor aanpassen.
public static class BuildRealityProject
{
    private static readonly Color Mint = new Color(0.25f, 0.95f, 0.77f);
    private static readonly Color Purple = new Color(0.62f, 0.32f, 1f);

    [MenuItem("REALITY/1 - Maak scenes opnieuw (overschrijft scenes)")]
    public static void CreateScenes()
    {
        Directory.CreateDirectory("Assets/Reality/Scenes");
        Directory.CreateDirectory("Assets/Reality/Materials");
        Directory.CreateDirectory("Assets/Resources");
        PrepareTargets();
        CreateScene(false);
        CreateScene(true);
        EditorBuildSettings.scenes = new[] {
            new EditorBuildSettingsScene("Assets/Reality/Scenes/RealityAR.unity", true)
        };
        PlayerSettings.companyName = "Reality Team";
        PlayerSettings.productName = "REALITY.EXE";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "be.realityexe.reality_ar");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        // De Input System-package wordt voor zowel aanraken als muis gebruikt.
        var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        settings.FindProperty("activeInputHandler").intValue = 1;
        settings.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.SaveAssets();
        if (File.Exists("Assets/StreamingAssets/Vuforia/REALITY_EXE.xml"))
            DatabaseTargetSetup.ConfigureDatabase();
        Debug.Log("REALITY scenes klaar.");
    }

    private static void PrepareTargets()
    {
        foreach (string name in new[] { "stegosaurus", "triceratops", "tyrannosaurus" })
        {
            string location = "Assets/Reality/Targets/" + name + ".png";
            var importer = AssetImporter.GetAtPath(location) as TextureImporter;
            importer.isReadable = true;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    private static void CreateScene(bool training)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var cameraObject = new GameObject(training ? "TrainingCamera" : "ARCamera");
        cameraObject.tag = "MainCamera";
        var camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.025f, 0.04f, 0.065f);
        camera.nearClipPlane = 0.005f;
        camera.farClipPlane = 20;
        if (training)
        {
            camera.transform.position = new Vector3(0, 0.34f, -0.42f);
            camera.transform.LookAt(new Vector3(0, 0.04f, 0));
            camera.fieldOfView = 45;
        }
        var light = new GameObject("Directional Light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.4f;
        light.transform.rotation = Quaternion.Euler(40, -30, 0);
        RenderSettings.ambientLight = new Color(0.5f, 0.55f, 0.6f);
        var bridge = new GameObject("RealityBridge").AddComponent<RealityBridge>();
        bridge.trainingMode = training;
        bridge.missions = new GlitchMission[] { MakeBlackHole(), MakeGravity(), MakeEntity() };
        var input = bridge.gameObject.AddComponent<PointerInput>();
        input.sceneCamera = camera;
        var hud = bridge.gameObject.AddComponent<MissionHud>();
        hud.bridge = bridge;
        if (!training)
        {
            var bootstrap = bridge.gameObject.AddComponent<ArBootstrap>();
            bootstrap.bridge = bridge;
            bootstrap.arCamera = camera;
            bootstrap.imageTargets = new ImageTargetBehaviour[bridge.missions.Length];
            for (int i = 0; i < bridge.missions.Length; i++)
            {
                var mission = bridge.missions[i];
                var targetObject = new GameObject("ImageTarget_" + mission.glitchId);
                var target = targetObject.AddComponent<ImageTargetBehaviour>();
                target.ImageTargetType = ImageTargetType.PREDEFINED;
                // Database en Image Target kies je na import van de unitypackage in de Inspector.
                // Geen zelfgemaakte databasepaden of niet-bestaande doelgegevens invullen.
                mission.transform.SetParent(target.transform, false);
                var tracking = targetObject.AddComponent<TargetTracking>();
                tracking.mission = mission;
                bootstrap.imageTargets[i] = target;
            }
        }
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),
            "Assets/Reality/Scenes/" + (training ? "Training" : "RealityAR") + ".unity");
    }

    private static T Mission<T>(string id, string name, string instruction) where T : GlitchMission
    {
        var root = new GameObject(name);
        var mission = root.AddComponent<T>();
        mission.glitchId = id;
        mission.displayName = name;
        mission.instruction = instruction;
        mission.visuals = new GameObject("3D inhoud");
        mission.visuals.transform.SetParent(root.transform, false);
        Shape(PrimitiveType.Cylinder, "Platform", mission.visuals.transform,
            new Vector3(0, -0.005f, 0), new Vector3(0.19f, 0.004f, 0.19f), new Color(0.05f, 0.10f, 0.14f));
        return mission;
    }

    private static BlackHoleMission MakeBlackHole()
    {
        var mission = Mission<BlackHoleMission>("glitch_001", "Mini Black Hole",
            "Tik op de drie verschillende energiepunten.");
        var art = mission.visuals.transform;
        mission.blackHole = Shape(PrimitiveType.Sphere, "Black Hole", art,
            new Vector3(0, 0.065f, 0), Vector3.one * 0.065f, new Color(0.04f, 0.01f, 0.08f));
        mission.blackHole.AddComponent<SpinObject>();
        var ring = Shape(PrimitiveType.Cylinder, "Accretion Disk", mission.blackHole.transform,
            Vector3.zero, new Vector3(1.8f, 0.035f, 1.8f), Purple);
        ring.transform.localRotation = Quaternion.Euler(20, 0, 0);
        mission.energyPoints = new Renderer[3];
        for (int i = 0; i < 3; i++)
        {
            float angle = (i * 120 + 30) * Mathf.Deg2Rad;
            var point = Shape(PrimitiveType.Sphere, "Energy " + (i + 1), art,
                new Vector3(Mathf.Cos(angle) * 0.075f, 0.03f, Mathf.Sin(angle) * 0.075f), Vector3.one * 0.028f, Mint);
            Interactable(point, mission, i);
            mission.energyPoints[i] = point.GetComponent<Renderer>();
        }
        return mission;
    }

    private static GravityMission MakeGravity()
    {
        var mission = Mission<GravityMission>("glitch_002", "Gravity Error",
            "Herstel de code: tik op 3 > 1 > 2. Een fout begint opnieuw.");
        var art = mission.visuals.transform;
        mission.cube = Shape(PrimitiveType.Cube, "Floating Cube", art,
            new Vector3(0, 0.10f, 0), Vector3.one * 0.04f, Purple).transform;
        mission.buttons = new Renderer[3];
        for (int i = 0; i < 3; i++)
        {
            var button = Shape(PrimitiveType.Cube, "Code " + (i + 1), art,
                new Vector3((i - 1) * 0.055f, 0.018f, -0.07f), new Vector3(0.042f, 0.025f, 0.035f), Mint);
            Interactable(button, mission, i);
            mission.buttons[i] = button.GetComponent<Renderer>();
            var label = new GameObject("Number").AddComponent<TextMesh>();
            label.transform.SetParent(button.transform, false);
            label.transform.localPosition = new Vector3(0, 0.52f, 0);
            label.transform.localRotation = Quaternion.Euler(90, 0, 0);
            label.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            label.text = (i + 1).ToString();
            label.anchor = TextAnchor.MiddleCenter;
            label.fontSize = 45;
            label.characterSize = 1;
            label.color = Color.black;
        }
        return mission;
    }

    private static EntityMission MakeEntity()
    {
        var mission = Mission<EntityMission>("glitch_003", "Entity Escaped",
            "Vang het bewegende wezen: tik er drie keer op.");
        var art = mission.visuals.transform;
        mission.entity = Shape(PrimitiveType.Capsule, "Entity", art,
            new Vector3(0, 0.07f, 0), new Vector3(0.03f, 0.025f, 0.03f), Purple).transform;
        Interactable(mission.entity.gameObject, mission, 0);
        for (int i = -1; i <= 1; i += 2)
            Shape(PrimitiveType.Sphere, "Eye", mission.entity,
                new Vector3(i * 0.22f, 0.2f, -0.43f), new Vector3(0.25f, 0.25f, 0.12f), Mint);
        return mission;
    }

    private static void Interactable(GameObject obj, GlitchMission mission, int index)
    {
        var tap = obj.AddComponent<TapTarget>();
        tap.mission = mission;
        tap.index = index;
    }

    private static GameObject Shape(PrimitiveType type, string name, Transform parent,
        Vector3 position, Vector3 scale, Color color)
    {
        var obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        string materialPath = "Assets/Reality/Materials/" + ColorUtility.ToHtmlStringRGB(color) + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            material.color = color;
            material.SetFloat("_Glossiness", 0.65f);
            AssetDatabase.CreateAsset(material, materialPath);
        }
        obj.GetComponent<Renderer>().sharedMaterial = material;
        // Decoratie mag geen raycasts tegenhouden.
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        // Alleen de drie soorten aanklikbare objecten krijgen later een collider.
        if (name.StartsWith("Energy") || name.StartsWith("Code") || name == "Entity")
            obj.AddComponent<BoxCollider>();
        return obj;
    }
}
