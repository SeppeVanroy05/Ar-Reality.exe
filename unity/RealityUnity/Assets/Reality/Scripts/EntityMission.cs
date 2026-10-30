using UnityEngine;

public class EntityMission : GlitchMission
{
    public Transform entity;
    public Transform[] decoys;
    public Renderer shield;
    public Transform cage;
    private float shieldTime;
    private float captureTime;
    public bool IsVulnerable => shieldTime <= 0 && Mathf.Repeat(ElapsedSeconds, 2.8f) < 1.8f;
    public override string PhaseLabel => "VANGSTLADINGEN";

    protected override void ResetPuzzle()
    {
        entity.gameObject.SetActive(true);
        foreach (var decoy in decoys) decoy.gameObject.SetActive(true);
        cage.gameObject.SetActive(false);
        captureTime = 0;
        shieldTime = 0;
        Feedback = "Vang het mintgroene wezen als zijn schild open is. Vermijd rode lokvogels.";
        UpdateShield();
    }

    protected override void HandleTap(int index)
    {
        if (index != 0)
        {
            Feedback = "Dat is een lokvogel! Zoek het wezen met de mintgroene ogen.";
            effects.Reject();
            return;
        }
        if (!IsVulnerable)
        {
            Feedback = "Schild gesloten. Wacht tot de paarse beschermring verdwijnt.";
            effects.Reject();
            return;
        }
        Steps++;
        shieldTime = 0.8f;
        effects.Hit(entity.localPosition, false);
        Feedback = "Raak! Het wezen ontwijkt je en wordt sneller. Vangst: " + Steps + "/3.";
        if (Steps == 3)
        {
            cage.gameObject.SetActive(true);
            cage.localPosition = entity.localPosition;
            foreach (var decoy in decoys) decoy.gameObject.SetActive(false);
        }
        UpdateShield();
    }

    private void UpdateShield()
    {
        shield.gameObject.SetActive(!IsVulnerable && !IsComplete);
    }

    protected override void Tick(float seconds)
    {
        shieldTime -= seconds;
        if (IsComplete)
        {
            captureTime += seconds;
            cage.localScale = Vector3.one * (0.045f + Mathf.Min(captureTime, 0.5f) * 0.04f);
            entity.localScale = Vector3.one * Mathf.Max(0, 1 - captureTime * 1.7f);
            if (captureTime > 0.65f) entity.gameObject.SetActive(false);
            return;
        }
        float time = ElapsedSeconds * (1 + Steps * 0.22f);
        entity.localPosition = new Vector3(Mathf.Sin(time * 1.2f) * 0.06f,
            0.065f + Mathf.Cos(time * 1.7f) * 0.018f, Mathf.Cos(time * 0.9f) * 0.035f);
        entity.localScale = Vector3.one;
        entity.localRotation = Quaternion.Euler(Mathf.Sin(time * 2) * 12, Mathf.Sin(time) * 30, Mathf.Cos(time) * 10);
        for (int i = 0; i < decoys.Length; i++)
        {
            float phase = time + i * 3 + 1.4f;
            decoys[i].localPosition = new Vector3(Mathf.Sin(phase) * 0.075f,
                0.055f + Mathf.Cos(phase * 1.8f) * 0.02f, Mathf.Cos(phase) * 0.045f);
            decoys[i].Rotate(Vector3.up, seconds * 65);
        }
        UpdateShield();
    }
}
