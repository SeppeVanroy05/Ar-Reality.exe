using UnityEngine;

public class BlackHoleMission : GlitchMission
{
    public Renderer[] energyPoints;
    public GameObject blackHole;
    public Transform[] orbitRings;
    public LineRenderer[] beams;
    private int charges;
    private float collapse;
    public bool WindowOpen => Mathf.Repeat(ElapsedSeconds, 1.8f) < 1.15f;
    public int ActivePoint => Mathf.Min(Steps, 2);
    public override string PhaseLabel => "KERNEN VERZEGELD";

    protected override void ResetPuzzle()
    {
        charges = 0;
        collapse = 0;
        blackHole.SetActive(true);
        blackHole.transform.localScale = Vector3.one * 0.045f;
        foreach (var beam in beams) beam.enabled = false;
        Feedback = "Laad kern 1: tik tweemaal wanneer hij mintgroen oplicht.";
        UpdatePoints();
    }

    protected override void HandleTap(int index)
    {
        if (index != ActivePoint)
        {
            Feedback = "Volg de oplichtende kern. De andere kernen zijn vergrendeld.";
            effects.Reject();
            return;
        }
        if (!WindowOpen)
        {
            Feedback = "Even wachten! Tik wanneer de kern mintgroen is.";
            effects.Reject();
            return;
        }
        charges++;
        effects.Hit(energyPoints[index].transform.localPosition, false);
        if (charges < 2)
        {
            Feedback = "Half opgeladen! Raak dezelfde kern nog een keer.";
            return;
        }
        charges = 0;
        Steps++;
        beams[index].enabled = true;
        Feedback = Steps < 3 ? "Kern verzegeld! Laad nu kern " + (Steps + 1) + "." : "";
        UpdatePoints();
    }

    private void UpdatePoints()
    {
        for (int i = 0; i < energyPoints.Length; i++)
        {
            Color color = i < Steps ? MissionEffects.Mint : i == ActivePoint && !IsComplete
                ? (WindowOpen ? MissionEffects.Mint : MissionEffects.Amber) : new Color(0.22f, 0.18f, 0.36f);
            MissionEffects.Tint(energyPoints[i], color);
        }
    }

    protected override void Tick(float seconds)
    {
        if (IsComplete)
        {
            collapse += seconds;
            blackHole.transform.localScale = Vector3.one * 0.045f * Mathf.Max(0, 1 - collapse / 0.75f);
            if (collapse > 0.75f) blackHole.SetActive(false);
        }
        else blackHole.transform.localScale = Vector3.one * (0.045f + Mathf.Sin(ElapsedSeconds * 3) * 0.002f);
        for (int i = 0; i < energyPoints.Length; i++)
        {
            float angle = ElapsedSeconds * (0.35f + i * 0.08f) + i * Mathf.PI * 2 / 3;
            energyPoints[i].transform.localPosition = new Vector3(Mathf.Cos(angle) * 0.074f,
                0.043f + Mathf.Sin(angle * 2) * 0.009f, Mathf.Sin(angle) * 0.048f);
            float size = i == ActivePoint ? 0.022f + Mathf.Sin(ElapsedSeconds * 6) * 0.002f : 0.018f;
            energyPoints[i].transform.localScale = Vector3.one * size;
            beams[i].SetPosition(0, energyPoints[i].transform.localPosition);
            beams[i].SetPosition(1, blackHole.transform.localPosition);
        }
        for (int i = 0; i < orbitRings.Length; i++)
            orbitRings[i].Rotate(Vector3.up, seconds * (i % 2 == 0 ? 32 : -22), Space.Self);
        UpdatePoints();
    }
}
