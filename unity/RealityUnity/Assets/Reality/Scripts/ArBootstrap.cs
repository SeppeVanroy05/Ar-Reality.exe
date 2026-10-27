using UnityEngine;
using Vuforia;

// Start Vuforia. De Image Targets staan al in de scene en komen uit Target Manager.
public class ArBootstrap : MonoBehaviour
{
    public RealityBridge bridge;
    public Camera arCamera;
    public ImageTargetBehaviour[] imageTargets;

    private void Start()
    {
        if (imageTargets == null || imageTargets.Length != bridge.missions.Length)
        {
            bridge.StartupError = "De Image Targets zijn niet correct gekoppeld in de Unity-scene.";
            return;
        }
        foreach (var target in imageTargets)
        {
            if (target == null || target.ImageTargetType != ImageTargetType.PREDEFINED ||
                string.IsNullOrWhiteSpace(target.TargetName))
            {
                bridge.StartupError = "Koppel elk Image Target aan de Vuforia-database in de Inspector.";
                return;
            }
        }
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
        // Vuforia gebruikt de Image Target-componenten die in de editor zijn ingesteld.
        bridge.Ready = true;
    }

    private void OnDestroy()
    {
        VuforiaApplication.Instance.OnVuforiaInitialized -= OnInitialized;
        VuforiaApplication.Instance.OnVuforiaStarted -= OnStarted;
    }
}
