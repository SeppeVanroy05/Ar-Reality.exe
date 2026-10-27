using UnityEngine;

public class BlackHoleMission : GlitchMission
{
    public Renderer[] energyPoints;
    public GameObject blackHole;
    private readonly bool[] activated = new bool[3];

    protected override void ResetPuzzle()
    {
        blackHole.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            activated[i] = false;
            energyPoints[i].material.color = Color.cyan;
        }
    }

    protected override void HandleTap(int index)
    {
        if (index < 0 || index >= 3 || activated[index]) return;
        activated[index] = true;
        energyPoints[index].material.color = Color.green;
        Steps++;
        Feedback = "Energiepunten geactiveerd: " + Steps + "/3";
        if (Steps == 3) blackHole.SetActive(false);
    }
}
