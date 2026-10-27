using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VerifyRealityProject
{
    private static void Check(bool condition, string message)
    {
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
        hole.Tap(0); hole.Tap(0);
        Check(hole.Steps == 1, "dubbele energietik telt niet");
        hole.SetTracked(false); hole.Tap(1);
        Check(hole.Steps == 1, "trackingverlies blokkeert interactie");
        hole.SetTracked(true); hole.Tap(1); hole.Tap(2); hole.Tap(2);
        Check(hole.IsComplete && events == 1, "een voltooiingsbericht");
        hole.Begin();
        Check(hole.Steps == 0 && !hole.IsComplete, "opnieuw spelen reset de puzzel");
        hole.Stop(); hole.Tap(0);
        Check(hole.Steps == 0, "gestopte missie reageert niet");
        var gravity = (GravityMission)bridge.missions[1];
        gravity.SetTracked(true); gravity.Begin();
        gravity.Tap(2); gravity.Tap(1);
        Check(gravity.Steps == 0, "foute code begint opnieuw");
        gravity.Tap(2); gravity.Tap(0); gravity.Tap(1);
        Check(gravity.IsComplete, "juiste code voltooit missie");
        gravity.Stop();
        var entity = (EntityMission)bridge.missions[2];
        entity.SetTracked(true); entity.Begin();
        entity.Tap(0); entity.Tap(0); entity.Tap(0);
        Check(entity.IsComplete && !entity.entity.gameObject.activeSelf, "wezen gevangen");
        entity.Stop();
        Check(EditorBuildSettings.scenes.Length == 1 &&
            EditorBuildSettings.scenes[0].path.EndsWith("RealityAR.unity"), "alleen AR-scene in export");
        Debug.Log("REALITY TESTS PASSED: 10 controles.");
        // Open opnieuw zonder de testwijzigingen op te slaan.
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/Training.unity");
    }

    public static void PrepareAndroidExport()
    {
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/RealityAR.unity");
        ProjectExporterBatchmode.ExportProjectAndroid();
    }
}
