using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class PreviewReality
{
    [MenuItem("REALITY/3 - Maak 3D-voorbeelden")]
    public static void Render()
    {
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/Training.unity");
        var bridge = Object.FindFirstObjectByType<RealityBridge>();
        var camera = Object.FindFirstObjectByType<Camera>();
        string output = Path.GetFullPath("../../docs/previews");
        Directory.CreateDirectory(output);
        foreach (var mission in bridge.missions) mission.Stop();
        foreach (var mission in bridge.missions)
        {
            mission.SetTracked(true);
            mission.Begin();
            var render = new RenderTexture(720, 960, 24);
            camera.targetTexture = render;
            camera.Render();
            RenderTexture.active = render;
            var image = new Texture2D(720, 960, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 720, 960), 0, 0);
            image.Apply();
            File.WriteAllBytes(Path.Combine(output, "unity-" + mission.glitchId + ".png"), image.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(image);
            Object.DestroyImmediate(render);
            mission.Stop();
        }
        EditorSceneManager.OpenScene("Assets/Reality/Scenes/Training.unity");
        Debug.Log("REALITY 3D previews opgeslagen.");
    }
}
