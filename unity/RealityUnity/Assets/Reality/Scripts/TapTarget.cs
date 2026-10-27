using UnityEngine;

// Alleen de verwijzing naar de puzzel en het nummer van het aangetikte object.
public class TapTarget : MonoBehaviour
{
    public GlitchMission mission;
    public int index;
    public void Activate() { mission.Tap(index); }
}
