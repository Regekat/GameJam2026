using UnityEngine;
using UnityEngine.InputSystem;

public class SubtleCameraTilt : MonoBehaviour
{
    [Header("Tilt Settings")]
    public float maxTiltAngle = 3f;      // how far it tilts (degrees)
    public float smoothSpeed = 5f;       // how smooth it moves

    private Quaternion originalRotation;

    void Start()
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Convert to -1 to 1 range
        float x = (mousePos.x / Screen.width) * 2f - 1f;
        float y = (mousePos.y / Screen.height) * 2f - 1f;

        // Invert Y so it feels natural
        y = -y;

        // Calculate rotation
        float tiltX = y * maxTiltAngle;
        float tiltY = x * maxTiltAngle;

        Quaternion targetRotation = originalRotation *
            Quaternion.Euler(tiltX, tiltY, 0);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothSpeed
        );
    }
}
