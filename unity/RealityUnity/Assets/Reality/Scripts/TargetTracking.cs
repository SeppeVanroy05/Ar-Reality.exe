using UnityEngine;
using Vuforia;

// Deze component blijft op het Image Target, ook als de 3D-inhoud verborgen is.
public class TargetTracking : MonoBehaviour
{
    public GlitchMission mission;
    private ObserverBehaviour observer;
    private void Start()
    {
        observer = GetComponent<ObserverBehaviour>();
        observer.OnTargetStatusChanged += OnStatusChanged;
        OnStatusChanged(observer, observer.TargetStatus);
    }
    private void OnStatusChanged(ObserverBehaviour source, TargetStatus status)
    {
        // Alleen zichtbaar getrackte kaarten tellen. Verlies pauzeert interacties.
        mission.SetTracked(status.Status == Status.TRACKED);
    }
    private void OnDestroy()
    {
        if (observer != null) observer.OnTargetStatusChanged -= OnStatusChanged;
    }
}
