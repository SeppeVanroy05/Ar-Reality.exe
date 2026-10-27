using System;
using UnityEngine;

// Gemeenschappelijke spelregels. De drie concrete puzzels erven hiervan.
public abstract class GlitchMission : MonoBehaviour
{
    public string glitchId;
    public string displayName;
    [TextArea] public string instruction;
    public GameObject visuals;
    public int Steps { get; protected set; }
    public bool IsTracked { get; private set; }
    public bool IsPlaying { get; private set; }
    public bool IsComplete { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public string Feedback { get; protected set; }
    public event Action<GlitchMission> Completed;

    public void Begin()
    {
        Steps = 0;
        ElapsedSeconds = 0;
        IsComplete = false;
        IsPlaying = true;
        Feedback = instruction;
        ResetPuzzle();
        visuals.SetActive(IsTracked);
    }

    public void Stop()
    {
        IsPlaying = false;
        visuals.SetActive(false);
    }

    public void SetTracked(bool tracked)
    {
        IsTracked = tracked;
        visuals.SetActive(tracked && IsPlaying);
    }

    protected virtual void Update()
    {
        if (IsPlaying && IsTracked && !IsComplete && Application.isFocused)
            ElapsedSeconds += Time.unscaledDeltaTime;
    }

    public void Tap(int index)
    {
        if (!IsPlaying || !IsTracked || IsComplete) return;
        HandleTap(index);
        if (Steps < 3) return;
        IsComplete = true;
        Feedback = "Glitch hersteld! Goed werk, agent.";
        Completed?.Invoke(this);
    }

    protected abstract void HandleTap(int index);
    protected abstract void ResetPuzzle();
}
