using UnityEngine;
using UnityEngine.InputSystem;

public class CartMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float groundDrag = 3f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = groundDrag;
    }

    void Update()
    {
        //Get Keyboard Input + Sync Camera's direction to Ball's movement
        GetInput();
    }

    void FixedUpdate()
    {
        MoveCart();
        LimitSpeed();
    }

    void GetInput()
    {
        //Call on the Keyboard for this script
        Keyboard kb = Keyboard.current;

        float horizontal = 0f;
        float vertical = 0f;

        //Convert Keyboard inputs into 2 float numbers: Vertical, and Horizontal
        if (kb.wKey.isPressed) vertical += 1f;
        if (kb.sKey.isPressed) vertical -= 1f;
        if (kb.aKey.isPressed) horizontal -= 1f;
        if (kb.dKey.isPressed) horizontal += 1f;

        //Call on Unity's "forward" and "right" vector variables
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        //Flatten Y-Axis so that if the player points the camera at the sky, the ball won't fly away
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        //Calculate the final direction the ball will move in via input
        moveDirection = (forward * vertical + right * horizontal).normalized;
    }
    void MoveCart()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            rb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);
        }
    }

    void LimitSpeed()
    {
        // Get horizontal velocity (ignore Y axis)
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Clamp speed if exceeding max
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }
}