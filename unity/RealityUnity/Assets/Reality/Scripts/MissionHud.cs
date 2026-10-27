using UnityEngine;

// Kleine Unity-interface. Alle gewone apppagina's worden in Flutter getekend.
public class MissionHud : MonoBehaviour
{
    public RealityBridge bridge;
    private GUIStyle title;
    private GUIStyle body;

    private void OnGUI()
    {
        float scale = Screen.width / 540f;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
        title ??= new GUIStyle(GUI.skin.label) { fontSize = 21, fontStyle = FontStyle.Bold };
        body ??= new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
        GUI.Box(new Rect(10, 10, 520, 180), "");
        var mission = bridge.ActiveMission;
        GUI.Label(new Rect(24, 20, 490, 30), bridge.trainingMode ? "REALITY.EXE / UNITY OEFENMODUS" : "REALITY.EXE / AR", title);
        if (!string.IsNullOrEmpty(bridge.StartupError))
            GUI.Label(new Rect(24, 60, 490, 105), bridge.StartupError, body);
        else if (mission == null)
            GUI.Label(new Rect(24, 60, 490, 100), "Wachten op een missie vanuit Flutter...", body);
        else
        {
            GUI.Label(new Rect(24, 55, 490, 28), mission.displayName + " - " + mission.Steps + "/3", body);
            string text = mission.IsComplete ? mission.Feedback : !mission.IsTracked
                ? "Kaart niet zichtbaar. Breng de juiste afbeelding in beeld."
                : mission.Feedback;
            GUI.Label(new Rect(24, 90, 490, 90), text, body);
        }
        if (!bridge.trainingMode) return;
        float bottom = Screen.height / scale - 60;
        for (int i = 0; i < bridge.missions.Length; i++)
            if (GUI.Button(new Rect(10 + i * 175, bottom, 170, 45), "Missie " + (i + 1)))
                bridge.StartTraining(i);
    }
}
