using System;
using UnityEngine;

// Naam van dit GameObject moet RealityBridge zijn: Flutter stuurt er berichten naartoe.
public class RealityBridge : MonoBehaviour
{
    public GlitchMission[] missions;
    public bool trainingMode;
    public bool Ready { get; set; }
    public string StartupError { get; set; }
    public GlitchMission ActiveMission { get; private set; }
    private string sessionId;

    [Serializable] private class StartRequest
    {
        public string type;
        public int version;
        public string glitchId;
        public string sessionId;
    }
    [Serializable] private class ResultMessage
    {
        public string type = "completed";
        public int version = 1;
        public string glitchId;
        public string sessionId;
        public int steps;
        public int durationSeconds;
    }
    [Serializable] private class ErrorMessage
    {
        public string type = "error";
        public string message;
    }

    private void Awake()
    {
        foreach (var mission in missions)
        {
            mission.Completed += OnCompleted;
            mission.Stop();
        }
        if (trainingMode) Ready = true;
    }

    private void Start()
    {
        if (trainingMode) StartTraining(0);
    }

    public void Ping(string unused)
    {
        if (!string.IsNullOrEmpty(StartupError))
            Send(JsonUtility.ToJson(new ErrorMessage { message = StartupError }));
        else if (Ready) Send("{\"type\":\"ready\",\"version\":1}");
    }

    public void BeginMission(string json)
    {
        if (!Ready || trainingMode) return;
        try
        {
            var request = JsonUtility.FromJson<StartRequest>(json);
            if (request == null || request.type != "start" || request.version != 1 ||
                string.IsNullOrEmpty(request.sessionId)) return;
            foreach (var mission in missions)
            {
                if (mission.glitchId != request.glitchId) continue;
                EndMission("");
                sessionId = request.sessionId;
                ActiveMission = mission;
                mission.Begin();
                return;
            }
        }
        catch (ArgumentException) { Debug.LogWarning("Ongeldig startbericht van Flutter."); }
    }

    public void EndMission(string unused)
    {
        if (ActiveMission != null) ActiveMission.Stop();
        ActiveMission = null;
        sessionId = null;
    }

    public void StartTraining(int index)
    {
        if (!trainingMode || index < 0 || index >= missions.Length) return;
        EndMission("");
        ActiveMission = missions[index];
        // Oefenmodus in Unity gebruikt een vast vlak in plaats van een camerabeeld.
        ActiveMission.SetTracked(true);
        ActiveMission.Begin();
    }

    private void OnCompleted(GlitchMission mission)
    {
        if (trainingMode) { Debug.Log("Oefenmissie voltooid: " + mission.glitchId); return; }
        if (mission != ActiveMission || string.IsNullOrEmpty(sessionId)) return;
        Send(JsonUtility.ToJson(new ResultMessage {
            glitchId = mission.glitchId, sessionId = sessionId,
            steps = mission.Steps, durationSeconds = Mathf.FloorToInt(mission.ElapsedSeconds)
        }));
    }

    private void Send(string message)
    {
        if (Application.isEditor) Debug.Log("Unity > Flutter: " + message);
        else SendToFlutter.Send(message);
    }

    private void OnDestroy()
    {
        foreach (var mission in missions)
            if (mission != null) mission.Completed -= OnCompleted;
    }
}
