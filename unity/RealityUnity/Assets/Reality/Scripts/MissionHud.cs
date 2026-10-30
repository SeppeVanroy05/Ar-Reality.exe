using UnityEngine;

// Compacte spelinterface. Gewone pagina's en XP blijven de taak van Flutter.
public class MissionHud : MonoBehaviour
{
    public RealityBridge bridge;
    private GUIStyle heading;
    private GUIStyle small;
    private GUIStyle body;
    private GUIStyle button;
    private static readonly Color Ink = new Color(0.025f, 0.04f, 0.075f, 0.94f);

    public static bool BlocksPointer(Vector2 screenPosition)
    {
        float scale = Screen.width / 540f;
        float y = (Screen.height - screenPosition.y) / scale;
        return y < 145 || y > Screen.height / scale - 110;
    }

    private static void Panel(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void OnGUI()
    {
        float scale = Screen.width / 540f;
        float height = Screen.height / scale;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
        heading ??= new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold };
        small ??= new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
        body ??= new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
        button ??= new GUIStyle(GUI.skin.button) { fontSize = 15, fontStyle = FontStyle.Bold };
        var mission = bridge.ActiveMission;
        Color accent = mission != null && mission.IsComplete ? MissionEffects.Mint : MissionEffects.Violet;
        Panel(new Rect(14, 14, 512, 120), Ink);
        Panel(new Rect(14, 14, 4, 120), accent);
        GUI.color = MissionEffects.Mint;
        GUI.Label(new Rect(30, 23, 300, 22), bridge.trainingMode ? "REALITY.EXE  /  TRAINING" : "REALITY.EXE  /  LIVE OPERATIE", small);
        GUI.color = Color.white;
        GUI.Label(new Rect(30, 46, 470, 35), mission == null ? "Systeem verbinden..." : mission.displayName, heading);
        if (mission != null)
        {
            GUI.color = new Color(0.65f, 0.74f, 0.83f);
            GUI.Label(new Rect(30, 86, 360, 23), mission.PhaseLabel + "   " + mission.Steps + " / 3", small);
            GUI.color = Color.white;
            for (int i = 0; i < 3; i++)
                Panel(new Rect(30 + i * 158, 116, 148, 4), i < mission.Steps ? MissionEffects.Mint : new Color(0.17f, 0.23f, 0.31f));
            float bottom = height - (bridge.trainingMode ? 162 : 100);
            Panel(new Rect(14, bottom, 512, 86), Ink);
            GUI.color = mission.IsComplete || mission.IsTracked ? MissionEffects.Mint : MissionEffects.Amber;
            GUI.Label(new Rect(28, bottom + 8, 460, 22), mission.IsComplete ? "HERSTEL VOLTOOID" : mission.IsTracked
                ? "KAART GEVONDEN  /  " + Mathf.FloorToInt(mission.ElapsedSeconds) + " SEC" : "SCAN JE MISSIEKAART", small);
            GUI.color = Color.white;
            GUI.Label(new Rect(28, bottom + 31, mission is GravityMission ? 336 : 476, 52), mission.IsTracked
                ? mission.Feedback : "Breng de volledige kaart in beeld. Je spel blijft bewaard.", body);
            if (mission is GravityMission gravity && !mission.IsComplete && mission.IsTracked)
                if (GUI.Button(new Rect(378, bottom + 30, 132, 43), "Code herhalen", button)) gravity.Replay();
        }
        else if (!string.IsNullOrEmpty(bridge.StartupError))
            GUI.Label(new Rect(30, 85, 460, 50), bridge.StartupError, body);
        if (bridge.trainingMode)
            for (int i = 0; i < bridge.missions.Length; i++)
                if (GUI.Button(new Rect(14 + i * 174, height - 60, 164, 45), "Missie " + (i + 1), button)) bridge.StartTraining(i);
        GUI.color = Color.white;
        GUI.matrix = Matrix4x4.identity;
    }
}
