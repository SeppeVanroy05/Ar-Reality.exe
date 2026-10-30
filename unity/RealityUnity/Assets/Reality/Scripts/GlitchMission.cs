using System;
using UnityEngine;

// Alleen gedeelde spelregels en voortgang. Vormgeving zit in MissionEffects.
public abstract class GlitchMission : MonoBehaviour
{
    public string glitchId;
    public string displayName;
    [TextArea] public string instruction;
    public GameObject visuals;
    public MissionEffects effects;
    public int Steps { get; protected set; }
    public bool IsTracked { get; private set; }
    public bool IsPlaying { get; private set; }
    public bool IsComplete { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public string Feedback { get; protected set; }
    public virtual string PhaseLabel => "STABILITEIT";
    public event Action<GlitchMission> Completed;

    public void Begin()
    {
        Steps = 0;
        ElapsedSeconds = 0;
        IsComplete = false;
        IsPlaying = true;
        Feedback = instruction;
        effects?.ResetEffects();
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
        if (Application.isFocused) AdvanceTime(Time.deltaTime);
    }

    // Ook de editorcontroles gebruiken dezelfde klok. Kaartverlies pauzeert het spel.
    public void AdvanceTime(float seconds)
    {
        if (!IsPlaying || !IsTracked || seconds <= 0) return;
        if (!IsComplete) ElapsedSeconds += seconds;
        Tick(seconds);
    }

    public void Tap(int index)
    {
        if (!IsPlaying || !IsTracked || IsComplete) return;
        HandleTap(index);
        if (Steps < 3) return;
        IsComplete = true;
        Feedback = "Glitch hersteld. De werkelijkheid is weer veilig.";
        effects?.Celebrate();
        Completed?.Invoke(this);
    }

    protected abstract void HandleTap(int index);
    protected abstract void ResetPuzzle();
    protected virtual void Tick(float seconds) { }
}
