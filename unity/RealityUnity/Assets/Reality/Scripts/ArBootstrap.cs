using UnityEngine;
using Vuforia;

// Zet Vuforia aan en maak echte Image Targets met de drie afbeeldingen uit de les.
// CreateImageTarget gebruikt hier de afbeelding rechtstreeks; er is geen .dat-database nodig.
public class ArBootstrap : MonoBehaviour
{
    public RealityBridge bridge;
    public Camera arCamera;
    public Texture2D[] targetImages;
    public float printedWidthMeters = 0.20f;

    private void Start()
    {
        TextAsset license = Resources.Load<TextAsset>("vuforia-license");
        if (license == null || string.IsNullOrWhiteSpace(license.text))
        {
            bridge.StartupError = "Vuforia-licentiesleutel ontbreekt. Voeg die lokaal toe in Unity.";
            return;
        }
        VuforiaConfiguration.Instance.Vuforia.LicenseKey = license.text.Trim();
        VuforiaApplication.Instance.OnVuforiaInitialized += OnInitialized;
        VuforiaApplication.Instance.OnVuforiaStarted += OnStarted;
        arCamera.gameObject.AddComponent<VuforiaBehaviour>();
    }

    private void OnInitialized(VuforiaInitError error)
    {
        if (error != VuforiaInitError.NONE)
            bridge.StartupError = "Vuforia kon niet starten. Controleer de licentie, cameratoegang en het toestel.";
    }

    private void OnStarted()
    {
        for (int i = 0; i < bridge.missions.Length; i++)
        {
            var mission = bridge.missions[i];
            var target = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
                targetImages[i], printedWidthMeters, mission.glitchId);
            mission.transform.SetParent(target.transform, false);
            mission.transform.localPosition = Vector3.zero;
            mission.transform.localRotation = Quaternion.identity;
            var tracking = target.gameObject.AddComponent<TargetTracking>();
            tracking.mission = mission;
        }
        bridge.Ready = true;
    }

    private void OnDestroy()
    {
        VuforiaApplication.Instance.OnVuforiaInitialized -= OnInitialized;
        VuforiaApplication.Instance.OnVuforiaStarted -= OnStarted;
    }
}
