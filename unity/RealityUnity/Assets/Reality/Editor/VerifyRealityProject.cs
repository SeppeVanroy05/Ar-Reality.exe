using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VerifyRealityProject
{
    private static int checks;

    private static void Check(bool condition, string message)
    {
        checks++;
        if (!condition) throw new Exception("REALITY TEST FAILED: " + message);
    }

    [MenuItem("REALITY/2 - Controleer puzzels")]
    public static void Verify()
    {
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/Training.unity");
        var bridge = UnityEngine.Object.FindFirstObjectByType<RealityBridge>();
        Check(bridge.missions.Length == 3, "drie missies");
        foreach (var mission in bridge.missions) mission.Stop();
        var hole = (BlackHoleMission)bridge.missions[0];
        int events = 0;
        hole.Completed += _ => events++;
        hole.SetTracked(true);
        hole.Begin();
        checks = 0;
        hole.Tap(1);
        Check(hole.Steps == 0, "alleen de actieve kern telt");
        hole.Tap(0);
        Check(hole.Steps == 0, "een kern vraagt twee ladingen");
        hole.Tap(0); hole.Tap(0);
        Check(hole.Steps == 1, "een verzegelde kern telt niet dubbel");
        hole.SetTracked(false);
        hole.Tap(1);
        hole.AdvanceTime(3);
        Check(hole.Steps == 1 && hole.ElapsedSeconds == 0, "trackingverlies pauzeert klok en invoer");
        hole.SetTracked(true);
        hole.AdvanceTime(1.3f); hole.Tap(1); hole.Tap(1);
        Check(hole.Steps == 1, "gesloten tijdvenster blokkeert opladen");
        hole.AdvanceTime(0.6f);
        hole.Tap(1); hole.Tap(1); hole.Tap(2); hole.Tap(2); hole.Tap(2);
        Check(hole.IsComplete && events == 1, "een voltooiingsbericht na drie kernen");
        hole.AdvanceTime(1);
        Check(!hole.blackHole.activeSelf, "zwart gat stort in na de laatste kern");
        hole.Begin();
        Check(hole.Steps == 0 && !hole.IsComplete && hole.blackHole.activeSelf, "opnieuw spelen reset de puzzel");
        hole.Stop(); hole.Tap(0);
        Check(hole.Steps == 0, "gestopte missie reageert niet");
        var gravity = (GravityMission)bridge.missions[1];
        gravity.SetTracked(true); gravity.Begin();
        gravity.Tap(2); gravity.Tap(0);
        Check(gravity.Steps == 0, "tikken tijdens de lichtcode telt niet");
        gravity.AdvanceTime(3); gravity.Tap(2); gravity.Tap(1);
        Check(gravity.Steps == 0 && gravity.IsShowingSequence, "foute code herhaalt dezelfde ronde");
        gravity.AdvanceTime(3); gravity.Tap(2); gravity.Tap(0);
        Check(gravity.Steps == 1 && gravity.IsShowingSequence, "eerste code start langere ronde");
        gravity.AdvanceTime(4); gravity.Tap(1);
        gravity.Replay();
        Check(gravity.Steps == 1 && gravity.IsShowingSequence, "herhalen bewaart voltooide rondes");
        gravity.AdvanceTime(4); gravity.Tap(1); gravity.Tap(2); gravity.Tap(0);
        Check(gravity.Steps == 2, "tweede lichtcode hersteld");
        gravity.AdvanceTime(4); gravity.Tap(2); gravity.Tap(0); gravity.Tap(2); gravity.Tap(1);
        Check(gravity.IsComplete, "drie lichtcodes voltooien de missie");
        gravity.Begin();
        Check(gravity.Steps == 0 && gravity.IsShowingSequence, "geheugenspel kan opnieuw");
        gravity.Stop();
        var entity = (EntityMission)bridge.missions[2];
        entity.SetTracked(true); entity.Begin();
        entity.Tap(1); entity.Tap(2);
        Check(entity.Steps == 0, "lokvogels geven geen vangstpunten");
        entity.Tap(0); entity.Tap(0);
        Check(entity.Steps == 1, "schild voorkomt dubbel vangen");
        entity.AdvanceTime(0.9f); entity.Tap(0);
        Check(entity.Steps == 2, "vangst na schildopening");
        entity.AdvanceTime(1.1f); entity.Tap(0);
        Check(entity.Steps == 2, "periodiek schild blokkeert ook na afkoelen");
        entity.AdvanceTime(0.9f); entity.Tap(0);
        Check(entity.IsComplete && entity.cage.gameObject.activeSelf, "wezen eindigt in vangstveld");
        entity.AdvanceTime(1);
        Check(!entity.entity.gameObject.activeSelf, "wezen verdwijnt na vangstanimatie");
        entity.Begin();
        Check(entity.entity.gameObject.activeSelf && !entity.cage.gameObject.activeSelf && entity.decoys[0].gameObject.activeSelf,
            "wezen, lokvogels en veld resetten");
        entity.Stop();
        Check(EditorBuildSettings.scenes.Length == 1 &&
            EditorBuildSettings.scenes[0].path.EndsWith("RealityAR.unity"), "alleen AR-scene in export");
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/RealityAR.unity");
        var bootstrap = UnityEngine.Object.FindFirstObjectByType<ArBootstrap>();
        Check(bootstrap.imageTargets.Length == 3, "drie Image Targets in de editor");
        for (int i = 0; i < 3; i++)
        {
            var target = bootstrap.imageTargets[i];
            Check(target.ImageTargetType == Vuforia.ImageTargetType.PREDEFINED, "From Database ingesteld");
            Check(bootstrap.bridge.missions[i].transform.parent == target.transform, "puzzel is child van target");
            Check(target.GetComponent<TargetTracking>().mission == bootstrap.bridge.missions[i], "tracking gekoppeld");
        }
        DatabaseTargetSetup.RequireConfiguredTargets();
        Debug.Log("REALITY TESTS PASSED: " + checks + " controles en bestaande XML/DAT-database gecontroleerd.");
        // Open opnieuw zonder de testwijzigingen op te slaan.
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/Training.unity");
    }

    public static void VerifyAndExport()
    {
        Verify();
        PrepareAndroidExport();
    }

    public static void PrepareAndroidExport()
    {
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/RealityAR.unity");
        DatabaseTargetSetup.RequireConfiguredTargets();
        ProjectExporterBatchmode.ExportProjectAndroid();
    }

    public static void PrepareIosExport()
    {
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/RealityAR.unity");
        DatabaseTargetSetup.RequireConfiguredTargets();
        UnityEditor.PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.iOS,
            ScriptingImplementation.IL2CPP);
        PlayerSettings.iOS.targetOSVersionString = "15.0";
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.preserveFramebufferAlpha = true; // Render Over Native UI: vereist door Vuforia op iOS.
        PlayerSettings.iOS.cameraUsageDescription = "REALITY.EXE gebruikt de camera om je kaarten te herkennen en AR-puzzels te tonen.";
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS,
            new[] { UnityEngine.Rendering.GraphicsDeviceType.Metal });
#if UNITY_IOS
        ConfigureIosRunner();
#endif
        ProjectExporterBatchmode.ExportProjectIos();
    }

#if UNITY_IOS
    public static void ConfigureIosRunner()
    {
        var project = new UnityEditor.iOS.Xcode.PBXProject();
        string path = Path.GetFullPath("../../ar_mobile/ios/Runner.xcodeproj/project.pbxproj");
        project.ReadFromFile(path);
        string runner = project.TargetGuidByName("Runner");
        foreach (string framework in new[] { "UnityFramework.framework", "VuforiaEngine.framework", "UnityDriver.framework" })
        {
            string file = project.FindFileGuidByProjectPath(framework);
            if (!string.IsNullOrEmpty(file)) continue;
            file = project.AddFile(framework, framework, UnityEditor.iOS.Xcode.PBXSourceTree.Source);
            project.AddFileToBuild(runner, file);
            UnityEditor.iOS.Xcode.Extensions.PBXProjectExtensions.AddFileToEmbedFrameworks(project, runner, file);
        }
        // Vuforia zoekt op iOS in de appbundel. Unity-data zit in een apart framework.
        // Neem de herkenningsdatabase daarom ook als map in de Runner-resources op.
        const string databaseFolder = "Vuforia";
        if (string.IsNullOrEmpty(project.FindFileGuidByProjectPath(databaseFolder)))
        {
            string folder = project.AddFolderReference(
                "../../unity/RealityUnity/Assets/StreamingAssets/Vuforia",
                databaseFolder, UnityEditor.iOS.Xcode.PBXSourceTree.Source);
            project.AddFileToBuild(runner, folder);
        }
        project.AddBuildProperty(runner, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)");
        project.SetBuildProperty(runner, "ENABLE_BITCODE", "NO");
        project.WriteToFile(path);
    }
#endif

}
