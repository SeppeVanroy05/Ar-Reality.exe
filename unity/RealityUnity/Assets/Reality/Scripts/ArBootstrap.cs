using System.Collections;
using UnityEngine;
using Vuforia;

// Eerst de lokale sleutel instellen, daarna pas de echte Vuforia-camera starten.
[DefaultExecutionOrder(-1000)]
public class ArBootstrap : MonoBehaviour
{
    public RealityBridge bridge;
    public Camera arCamera;
    public ImageTargetBehaviour[] imageTargets;

    private void Awake()
    {
        VuforiaConfiguration.Instance.Vuforia.DelayedInitialization = true;
        TextAsset license = Resources.Load<TextAsset>("vuforia-license");
        if (license == null || string.IsNullOrWhiteSpace(license.text))
        {
            bridge.StartupError = "Vuforia-licentiesleutel ontbreekt. Voeg die lokaal toe in Unity.";
            return;
        }
        // De sleutel moet ingesteld zijn voordat Vuforia.Initialize wordt aangeroepen.
        VuforiaConfiguration.Instance.Vuforia.LicenseKey = license.text.Trim();
        VuforiaApplication.Instance.OnVuforiaInitialized += OnInitialized;
        VuforiaApplication.Instance.OnVuforiaStarted += OnStarted;
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(bridge.StartupError)) return;
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
        // Vuforia kan zelf al een camera hebben toegevoegd. Maak geen tweede instantie.
        if (VuforiaBehaviour.Instance == null)
            arCamera.gameObject.AddComponent<VuforiaBehaviour>();
        VuforiaApplication.Instance.Initialize();
    }

    private void OnInitialized(VuforiaInitError error)
    {
        if (error == VuforiaInitError.NONE) return;
        string code = error.ToString();
        if (code.Contains("LICENSE"))
            bridge.StartupError = "Vuforia weigert de licentie (" + code + "). Controleer je Vuforia Engine App Key.";
        else
            bridge.StartupError = "Vuforia kon niet starten (" + code + "). Controleer cameratoegang en het toestel.";
    }

    private void OnStarted()
    {
        StartCoroutine(WaitForTargets());
    }

    private IEnumerator WaitForTargets()
    {
        // Camera gestart betekent nog niet dat de herkenningsdatabase geladen is.
        float deadline = Time.realtimeSinceStartup + 10;
        while (Time.realtimeSinceStartup < deadline)
        {
            bool targetsReady = true;
            foreach (var target in imageTargets)
                if (!target.ID.HasValue) targetsReady = false;
            if (targetsReady)
            {
                bridge.Ready = true;
                yield break;
            }
            yield return null;
        }
        bridge.StartupError = "De camera werkt, maar de herkenningsdatabase kon niet worden geladen. Controleer de Unity-export.";
    }

    private void OnDestroy()
    {
        VuforiaApplication.Instance.OnVuforiaInitialized -= OnInitialized;
        VuforiaApplication.Instance.OnVuforiaStarted -= OnStarted;
    }
}
