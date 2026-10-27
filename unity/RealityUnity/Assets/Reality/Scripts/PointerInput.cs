using UnityEngine;
using UnityEngine.InputSystem;

// Zoals in de workshop: van schermpositie via een raycast naar een 3D-collider.
public class PointerInput : MonoBehaviour
{
    public Camera sceneCamera;
    public float topHudFraction = 0.27f;

    private void Update()
    {
        Vector2 position;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            position = Touchscreen.current.primaryTouch.position.ReadValue();
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            position = Mouse.current.position.ReadValue();
        else return;
        if (position.y > Screen.height * (1 - topHudFraction)) return;
        Ray ray = sceneCamera.ScreenPointToRay(position);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        TapTarget target = hit.collider.GetComponent<TapTarget>();
        if (target != null) target.Activate();
    }
}
