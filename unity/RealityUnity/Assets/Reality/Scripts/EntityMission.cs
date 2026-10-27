using UnityEngine;

public class EntityMission : GlitchMission
{
    public Transform entity;
    private float timeOffset;

    protected override void ResetPuzzle()
    {
        entity.gameObject.SetActive(true);
        timeOffset = 0;
    }

    protected override void HandleTap(int index)
    {
        Steps++;
        timeOffset += 2;
        Feedback = "Wezen geraakt: " + Steps + "/3";
        if (Steps == 3) entity.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        if (!IsPlaying || !IsTracked || IsComplete) return;
        float time = ElapsedSeconds + timeOffset;
        entity.localPosition = new Vector3(Mathf.Sin(time * 1.2f) * 0.055f,
            0.07f + Mathf.Cos(time * 1.7f) * 0.012f, Mathf.Cos(time) * 0.035f);
        entity.localRotation = Quaternion.Euler(0, time * 25, 0);
    }
}
