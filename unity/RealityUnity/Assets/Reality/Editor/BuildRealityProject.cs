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
        Directory.CreateDirectory("Assets/Reality/Meshes");
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
        cameraObject.AddComponent<AudioListener>();
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
        var effects = mission.visuals.AddComponent<MissionEffects>();
        effects.mission = mission;
        mission.effects = effects;
        var audio = mission.visuals.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.spatialBlend = 0;
        effects.sceneryRings = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            var ring = Ring("Scanner ring " + i, mission.visuals.transform,
                new Vector3(0, 0.002f + i * 0.002f, 0), 0.082f + i * 0.006f,
                i == 1 ? Purple : Mint);
            ring.transform.localRotation = Quaternion.Euler(0, i * 40, 0);
            effects.sceneryRings[i] = ring.transform;
        }
        for (int i = 0; i < 12; i++)
        {
            float angle = i * Mathf.PI / 6;
            var tile = Shape(PrimitiveType.Cube, "Scanner tick", mission.visuals.transform,
                new Vector3(Mathf.Cos(angle) * 0.098f, 0.002f, Mathf.Sin(angle) * 0.098f),
                new Vector3(0.008f, 0.001f, 0.002f), Mint);
            tile.transform.localRotation = Quaternion.Euler(0, -i * 30, 0);
        }
        effects.sparks = new Transform[18];
        for (int i = 0; i < effects.sparks.Length; i++)
        {
            var spark = Shape(PrimitiveType.Cube, "Spark " + i, mission.visuals.transform,
                Vector3.zero, Vector3.one * 0.003f, Mint);
            effects.sparks[i] = spark.transform;
            spark.SetActive(false);
        }
        return mission;
    }

    private static BlackHoleMission MakeBlackHole()
    {
        var mission = Mission<BlackHoleMission>("glitch_001", "Mini Black Hole",
            "Laad elke draaiende kern tweemaal wanneer hij mintgroen oplicht.");
        var art = mission.visuals.transform;
        mission.blackHole = Shape(PrimitiveType.Sphere, "Singularity", art,
            new Vector3(0, 0.067f, 0), Vector3.one * 0.045f, new Color(0.015f, 0.008f, 0.03f));
        mission.blackHole.AddComponent<SpinObject>().speed = 22;
        mission.orbitRings = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            var ring = Ring("Accretion arc " + i, mission.blackHole.transform, Vector3.zero,
                0.64f + i * 0.18f, i == 1 ? new Color(1f, 0.52f, 0.22f) : Purple);
            ring.transform.localRotation = Quaternion.Euler(15 + i * 14, 0, i * 12);
            mission.orbitRings[i] = ring.transform;
        }
        // Kleine puinbrokken maken de draaiing rond de singulariteit zichtbaar.
        for (int i = 0; i < 10; i++)
        {
            float angle = i * Mathf.PI / 5;
            Shape(PrimitiveType.Cube, "Orbit debris", mission.blackHole.transform,
                new Vector3(Mathf.Cos(angle) * 1.05f, Mathf.Sin(i) * 0.1f, Mathf.Sin(angle) * 1.05f),
                Vector3.one * 0.06f, i % 2 == 0 ? Purple : Mint);
        }
        mission.energyPoints = new Renderer[3];
        mission.beams = new LineRenderer[3];
        for (int i = 0; i < 3; i++)
        {
            float angle = i * Mathf.PI * 2 / 3;
            var point = Shape(PrimitiveType.Sphere, "Energy " + (i + 1), art,
                new Vector3(Mathf.Cos(angle) * 0.074f, 0.043f, Mathf.Sin(angle) * 0.048f), Vector3.one * 0.022f, Mint);
            Interactable(point, mission, i);
            Ring("Core halo", point.transform, Vector3.zero, 0.65f, Mint);
            mission.energyPoints[i] = point.GetComponent<Renderer>();
            var beam = new GameObject("Energy stream " + i).AddComponent<LineRenderer>();
            beam.transform.SetParent(art, false);
            beam.useWorldSpace = false;
            beam.positionCount = 2;
            beam.startWidth = 0.0015f;
            beam.endWidth = 0.0004f;
            beam.sharedMaterial = MaterialFor(Mint, true);
            beam.SetPosition(0, point.transform.localPosition);
            beam.SetPosition(1, mission.blackHole.transform.localPosition);
            beam.enabled = false;
            mission.beams[i] = beam;
        }
        return mission;
    }

    private static GravityMission MakeGravity()
    {
        var mission = Mission<GravityMission>("glitch_002", "Gravity Error",
            "Onthoud de lichtcode en speel hem na. Herstel drie rondes.");
        var art = mission.visuals.transform;
        mission.cube = Shape(PrimitiveType.Cube, "Reactor core", art,
            new Vector3(0, 0.085f, 0), Vector3.one * 0.032f, Purple).transform;
        for (int i = 0; i < 3; i++)
        {
            var ring = Ring("Reactor gyroscope", mission.cube, Vector3.zero, 0.9f + i * 0.13f, i == 1 ? Mint : Purple);
            ring.transform.localRotation = Quaternion.Euler(i * 60 + 25, i * 40, i * 35);
        }
        mission.fragments = new Transform[4];
        for (int i = 0; i < 4; i++)
        {
            float angle = i * Mathf.PI / 2;
            var fragment = Shape(PrimitiveType.Cube, "Floating reactor shard " + i, art,
                new Vector3(Mathf.Cos(angle) * 0.06f, 0.085f, Mathf.Sin(angle) * 0.06f),
                new Vector3(0.018f, 0.025f, 0.018f), i % 2 == 0 ? Mint : Purple);
            fragment.transform.localRotation = Quaternion.Euler(35, i * 45, 30);
            mission.fragments[i] = fragment.transform;
            Ring("Shard band", fragment.transform, Vector3.zero, 0.85f, Mint);
        }
        mission.buttons = new Renderer[3];
        for (int i = 0; i < 3; i++)
        {
            var button = Shape(PrimitiveType.Cylinder, "Code " + (i + 1), art,
                new Vector3((i - 1) * 0.055f, 0.012f, -0.071f), new Vector3(0.039f, 0.009f, 0.039f), Mint);
            Interactable(button, mission, i);
            mission.buttons[i] = button.GetComponent<Renderer>();
            Ring("Button rim", art, button.transform.localPosition - Vector3.up * 0.007f, 0.024f, Purple);
            Label((i + 1).ToString(), button.transform, new Vector3(0, 1.05f, 0), 0.005f);
        }
        return mission;
    }

    private static EntityMission MakeEntity()
    {
        var mission = Mission<EntityMission>("glitch_003", "Entity Escaped",
            "Vang het mintgroene wezen als zijn schild open is. Vermijd lokvogels.");
        var art = mission.visuals.transform;
        // Een verticaal portaal, met daaromheen losse fragmenten.
        var portal = Ring("Escape portal", art, new Vector3(0, 0.042f, 0.04f), 0.037f, Purple);
        portal.transform.localRotation = Quaternion.Euler(80, 0, 0);
        portal.AddComponent<SpinObject>().speed = 35;
        mission.entity = Drone("Entity", art, Purple, Mint);
        Interactable(mission.entity.gameObject, mission, 0);
        var shield = Ring("Protective shield", mission.entity, Vector3.zero, 0.027f, Purple);
        shield.transform.localRotation = Quaternion.Euler(65, 0, 0);
        mission.shield = shield.GetComponent<Renderer>();
        mission.decoys = new Transform[2];
        for (int i = 0; i < 2; i++)
        {
            mission.decoys[i] = Drone("Decoy " + i, art, new Color(0.3f, 0.08f, 0.16f), new Color(1, 0.22f, 0.34f));
            mission.decoys[i].localPosition = new Vector3(i == 0 ? -0.06f : 0.06f, 0.055f, 0.01f);
            Interactable(mission.decoys[i].gameObject, mission, i + 1);
        }
        mission.cage = new GameObject("Containment field").transform;
        mission.cage.SetParent(art, false);
        mission.cage.localScale = Vector3.one * 0.045f;
        for (int i = 0; i < 3; i++)
        {
            var ring = Ring("Containment hoop", mission.cage, Vector3.zero, 0.65f, Mint);
            ring.transform.localRotation = Quaternion.Euler(i * 60, 0, i * 60);
        }
        mission.cage.gameObject.SetActive(false);
        return mission;
    }

    private static Transform Drone(string name, Transform parent, Color shell, Color eyes)
    {
        var root = new GameObject(name).transform;
        root.SetParent(parent, false);
        root.localPosition = new Vector3(0, 0.065f, 0);
        Shape(PrimitiveType.Sphere, "Drone shell", root, Vector3.zero, new Vector3(0.031f, 0.024f, 0.024f), shell);
        Shape(PrimitiveType.Cube, "Face visor", root, new Vector3(0, 0.001f, -0.011f), new Vector3(0.025f, 0.012f, 0.004f), new Color(0.015f, 0.025f, 0.04f));
        for (int i = -1; i <= 1; i += 2)
        {
            Shape(PrimitiveType.Cube, "Eye", root, new Vector3(i * 0.006f, 0.002f, -0.014f), new Vector3(0.004f, 0.005f, 0.002f), eyes);
            var wing = Shape(PrimitiveType.Cube, "Wing", root, new Vector3(i * 0.024f, 0.002f, 0), new Vector3(0.019f, 0.004f, 0.014f), shell);
            wing.transform.localRotation = Quaternion.Euler(0, 0, i * 24);
            Ring("Wing thruster", root, new Vector3(i * 0.025f, -0.004f, 0), 0.007f, eyes);
        }
        Shape(PrimitiveType.Cylinder, "Antenna", root, new Vector3(0, 0.017f, 0), new Vector3(0.0015f, 0.006f, 0.0015f), shell);
        Shape(PrimitiveType.Sphere, "Signal light", root, new Vector3(0, 0.024f, 0), Vector3.one * 0.004f, eyes);
        var collider = root.gameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.05f, 0.035f, 0.033f);
        return root;
    }

    private static void Label(string text, Transform parent, Vector3 position, float size)
    {
        var label = new GameObject("Label " + text).AddComponent<TextMesh>();
        label.transform.SetParent(parent, false);
        label.transform.localPosition = position;
        label.transform.localRotation = Quaternion.Euler(90, 0, 0);
        // Ouders kunnen verschillend geschaald zijn. Houd de tekens leesbaar.
        label.transform.localScale = new Vector3(1 / parent.localScale.x, 1 / parent.localScale.z, 1 / parent.localScale.y);
        label.text = text;
        label.anchor = TextAnchor.MiddleCenter;
        label.fontSize = 48;
        label.characterSize = size;
        label.color = Color.white;
    }

    private static GameObject Ring(string name, Transform parent, Vector3 position, float radius, Color color)
    {
        const string meshPath = "Assets/Reality/Meshes/HologramArc.asset";
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        if (mesh == null)
        {
            const int segments = 64;
            const int sides = 6;
            var vertices = new Vector3[(segments + 1) * sides];
            var triangles = new int[segments * sides * 6];
            for (int i = 0; i <= segments; i++)
                for (int j = 0; j < sides; j++)
                {
                    float angle = i / (float)segments * Mathf.PI * 1.85f;
                    float tube = j / (float)sides * Mathf.PI * 2;
                    float distance = 1 + Mathf.Cos(tube) * 0.018f;
                    vertices[i * sides + j] = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(tube) * 0.018f, Mathf.Sin(angle) * distance);
                    if (i == segments) continue;
                    int n = i * sides + j;
                    int next = i * sides + (j + 1) % sides;
                    int t = n * 6;
                    triangles[t] = n; triangles[t + 1] = next; triangles[t + 2] = n + sides;
                    triangles[t + 3] = next; triangles[t + 4] = next + sides; triangles[t + 5] = n + sides;
                }
            mesh = new Mesh { name = "Hologram arc", vertices = vertices, triangles = triangles };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh, meshPath);
        }
        var ring = new GameObject(name);
        ring.transform.SetParent(parent, false);
        ring.transform.localPosition = position;
        ring.transform.localScale = Vector3.one * radius;
        ring.AddComponent<MeshFilter>().sharedMesh = mesh;
        ring.AddComponent<MeshRenderer>().sharedMaterial = MaterialFor(color, true);
        return ring;
    }

    private static Material MaterialFor(Color color, bool luminous)
    {
        string materialPath = "Assets/Reality/Materials/" + (luminous ? "Glow_" : "Surface_") + ColorUtility.ToHtmlStringRGB(color) + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material != null) return material;
        material = new Material(Shader.Find(luminous ? "Unlit/Color" : "Standard"));
        material.color = color;
        if (!luminous)
        {
            material.SetFloat("_Glossiness", 0.75f);
            material.SetFloat("_Metallic", 0.35f);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 0.22f);
        }
        AssetDatabase.CreateAsset(material, materialPath);
        return material;
    }

    private static void Interactable(GameObject obj, GlitchMission mission, int index)
    {
        if (obj.GetComponent<Collider>() == null) obj.AddComponent<BoxCollider>();
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
        obj.GetComponent<Renderer>().sharedMaterial = MaterialFor(color, false);
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }
}
