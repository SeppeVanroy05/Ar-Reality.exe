using UnityEngine;
using UnityEngine.InputSystem;

// Zoals in de workshop: van schermpositie via een raycast naar een 3D-collider.
public class PointerInput : MonoBehaviour
{
    public Camera sceneCamera;

    private void Update()
    {
        Vector2 position;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            position = Touchscreen.current.primaryTouch.position.ReadValue();
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            position = Mouse.current.position.ReadValue();
        else return;
        if (MissionHud.BlocksPointer(position)) return;
        Ray ray = sceneCamera.ScreenPointToRay(position);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        TapTarget target = hit.collider.GetComponent<TapTarget>();
        if (target != null) target.Activate();
    }
}
