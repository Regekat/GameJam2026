using UnityEngine;
using UnityEngine.InputSystem;

public class SubtleCameraTilt : MonoBehaviour
{
    [Header("Tilt Settings")]
    public float maxTiltAngle = 3f;
    public float smoothSpeed = 5f;

    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        float x = (mousePos.x / Screen.width) * 2f - 1f;
        float y = (mousePos.y / Screen.height) * 2f - 1f;

        y = -y;

        float tiltX = y * maxTiltAngle;
        float tiltY = x * maxTiltAngle;

        Quaternion targetRotation = originalRotation * Quaternion.Euler(tiltX, tiltY, 0f);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.unscaledDeltaTime * smoothSpeed
        );
    }
}