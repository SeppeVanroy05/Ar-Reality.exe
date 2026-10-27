using UnityEngine;

public class SpinObject : MonoBehaviour
{
    public float speed = 35;
    private void Update() { transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self); }
}
