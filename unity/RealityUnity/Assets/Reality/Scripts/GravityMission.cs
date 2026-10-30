using UnityEngine;

public class GravityMission : GlitchMission
{
    public Transform cube;
    public Transform[] fragments;
    public Renderer[] buttons;
    private readonly int[][] sequences = { new[] { 2, 0 }, new[] { 1, 2, 0 }, new[] { 2, 0, 2, 1 } };
    private int entered;
    private float previewTime;
    private float flashTime;
    private int pressed = -1;
    private int lastLit = -1;
    public bool IsShowingSequence => previewTime < Sequence.Length * 0.8f + 0.45f;
    private int[] Sequence => sequences[Mathf.Min(Steps, 2)];
    public override string PhaseLabel => "GEHEUGENRONDES";

    protected override void ResetPuzzle()
    {
        cube.localPosition = new Vector3(0, 0.085f, 0);
        cube.localScale = Vector3.one * 0.032f;
        StartRound();
    }

    private void StartRound()
    {
        entered = 0;
        previewTime = 0;
        pressed = -1;
        lastLit = -1;
        flashTime = 0;
        Feedback = "Kijk goed: onthoud de lichtcode. Daarna ben jij aan de beurt.";
        UpdateLights();
    }

    public void Replay()
    {
        if (IsPlaying && !IsComplete && IsTracked) StartRound();
    }

    protected override void HandleTap(int index)
    {
        if (index < 0 || index >= buttons.Length || IsShowingSequence) return;
        if (index != Sequence[entered])
        {
            effects.Reject();
            StartRound();
            Feedback = "Bijna! Dezelfde ronde speelt opnieuw. Let op de lichtjes.";
            return;
        }
        pressed = index;
        flashTime = 0.28f;
        entered++;
        effects.Hit(buttons[index].transform.localPosition, false);
        Feedback = "Code ingevoerd: " + entered + "/" + Sequence.Length;
        if (entered < Sequence.Length) return;
        Steps++;
        if (Steps < 3) StartRound();
    }

    private void UpdateLights()
    {
        int lit = -1;
        if (!IsComplete && IsShowingSequence && previewTime >= 0.45f)
        {
            float time = previewTime - 0.45f;
            int step = Mathf.FloorToInt(time / 0.8f);
            if (step < Sequence.Length && Mathf.Repeat(time, 0.8f) < 0.52f) lit = Sequence[step];
        }
        else if (flashTime > 0) lit = pressed;
        if (lit >= 0 && lit != lastLit && IsShowingSequence) effects.Cue(lit);
        lastLit = lit;
        for (int i = 0; i < buttons.Length; i++)
            MissionEffects.Tint(buttons[i], IsComplete ? MissionEffects.Mint :
                i == lit ? MissionEffects.Amber : new Color(0.14f, 0.29f, 0.38f));
    }

    protected override void Tick(float seconds)
    {
        bool wasShowing = IsShowingSequence;
        if (!IsComplete) previewTime += seconds;
        flashTime -= seconds;
        if (wasShowing && !IsShowingSequence && !IsComplete)
            Feedback = "Jouw beurt! Tik de " + Sequence.Length + " lichtjes in dezelfde volgorde.";
        UpdateLights();
        float height = IsComplete ? 0.036f : 0.085f + Mathf.Sin(ElapsedSeconds * 2) * 0.008f;
        cube.localPosition = Vector3.Lerp(cube.localPosition, new Vector3(0, height, 0), Mathf.Min(1, seconds * 5));
        cube.Rotate(new Vector3(14, 28, 9) * seconds);
        for (int i = 0; i < fragments.Length; i++)
        {
            float angle = i * Mathf.PI / 2 + (IsComplete ? 0 : ElapsedSeconds * 0.45f);
            float radius = IsComplete ? 0.024f : 0.06f - Steps * 0.01f;
            Vector3 destination = new Vector3(Mathf.Cos(angle) * radius,
                height + Mathf.Sin(i * 2 + ElapsedSeconds) * (IsComplete ? 0 : 0.016f), Mathf.Sin(angle) * radius);
            fragments[i].localPosition = Vector3.Lerp(fragments[i].localPosition, destination, Mathf.Min(1, seconds * 4));
            fragments[i].Rotate(new Vector3(25, -35, 15) * seconds);
        }
    }
}
