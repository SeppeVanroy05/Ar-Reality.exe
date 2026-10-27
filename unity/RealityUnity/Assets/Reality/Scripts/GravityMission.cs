using UnityEngine;

public class GravityMission : GlitchMission
{
    public Transform cube;
    public Renderer[] buttons;
    private readonly int[] correctOrder = { 2, 0, 1 };
    private Vector3 startPosition;
    private bool falling;

    protected override void ResetPuzzle()
    {
        startPosition = new Vector3(0, 0.10f, 0);
        cube.localPosition = startPosition;
        falling = false;
        foreach (var button in buttons) button.material.color = Color.cyan;
    }

    protected override void HandleTap(int index)
    {
        if (index != correctOrder[Steps])
        {
            Steps = 0;
            foreach (var button in buttons) button.material.color = Color.cyan;
            Feedback = "Verkeerde volgorde. Probeer opnieuw: 3 > 1 > 2.";
            return;
        }
        buttons[index].material.color = Color.green;
        Steps++;
        Feedback = "Code hersteld: " + Steps + "/3";
        falling = Steps == 3;
    }

    protected override void Update()
    {
        base.Update();
        if (!IsPlaying || !IsTracked) return;
        if (falling)
            cube.localPosition = Vector3.MoveTowards(cube.localPosition,
                new Vector3(0, 0.025f, 0), Time.deltaTime * 0.08f);
        else
            cube.localPosition = startPosition + Vector3.up * Mathf.Sin(Time.time * 2) * 0.008f;
    }
}
