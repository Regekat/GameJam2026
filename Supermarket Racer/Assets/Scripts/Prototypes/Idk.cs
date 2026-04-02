using UnityEngine;

public class RollingCartBallController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody physicsBall;
    public Transform cartVisual;

    [Header("Follow")]
    public float visualHeightOffset = 0.75f;
    public float followLerpSpeed = 20f;

    [Header("Movement")]
    public float driveTorque = 25f;
    public float maxBallSpeed = 10f;
    public float accelerationCurvePower = 1.0f;

    [Header("Turning")]
    public float turnSpeed = 120f;

    [Header("Grip")]
    public float lateralDamping = 4f;
    public float angularDampingWhenIdle = 0.5f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.7f;
    public LayerMask groundMask = ~0;
    public bool requireGroundedToDrive = true;

    private float moveInput;
    private float turnInput;
    private bool isGrounded;

    private void Reset()
    {
        if (physicsBall == null)
        {
            Rigidbody rb = GetComponentInChildren<Rigidbody>();
            if (rb != null)
                physicsBall = rb;
        }
    }

    private void Awake()
    {
        if (physicsBall == null)
        {
            Debug.LogError("RollingCartBallController: PhysicsBall Rigidbody is not assigned.");
            enabled = false;
            return;
        }

        physicsBall.interpolation = RigidbodyInterpolation.Interpolate;
        physicsBall.maxAngularVelocity = 100f;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");     // W/S
        turnInput = Input.GetAxisRaw("Horizontal");   // A/D
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        TurnCart();
        DriveBall();
        DampSidewaysSlip();
        DampIdleSpin();
        FollowBall();
    }

    private void CheckGrounded()
    {
        Vector3 origin = physicsBall.worldCenterOfMass;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void TurnCart()
    {
        float yawAmount = turnInput * turnSpeed * Time.fixedDeltaTime;
        transform.Rotate(0f, yawAmount, 0f, Space.World);
    }

    private void DriveBall()
    {
        if (requireGroundedToDrive && !isGrounded)
            return;

        if (Mathf.Abs(moveInput) < 0.01f)
            return;

        Vector3 flatVelocity = physicsBall.linearVelocity;
        flatVelocity.y = 0f;

        float speed = flatVelocity.magnitude;
        float speedFactor = Mathf.Clamp01(speed / maxBallSpeed);

        float torqueScale = 1f - Mathf.Pow(speedFactor, accelerationCurvePower);
        if (torqueScale <= 0.01f)
            return;

        Vector3 moveDirection = transform.forward * Mathf.Sign(moveInput);

        // For a sphere, torque axis must be perpendicular to movement direction.
        Vector3 torqueAxis = Vector3.Cross(Vector3.up, moveDirection).normalized;

        physicsBall.AddTorque(torqueAxis * (driveTorque * Mathf.Abs(moveInput) * torqueScale), ForceMode.Acceleration);
    }

    private void DampSidewaysSlip()
    {
        Vector3 velocity = physicsBall.linearVelocity;
        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (flatVelocity.sqrMagnitude < 0.0001f)
            return;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float forwardSpeed = Vector3.Dot(flatVelocity, forward);
        float sidewaysSpeed = Vector3.Dot(flatVelocity, right);

        Vector3 correctedFlatVelocity =
            forward * forwardSpeed +
            right * Mathf.Lerp(sidewaysSpeed, 0f, lateralDamping * Time.fixedDeltaTime);

        physicsBall.linearVelocity = new Vector3(correctedFlatVelocity.x, velocity.y, correctedFlatVelocity.z);
    }

    private void DampIdleSpin()
    {
        if (Mathf.Abs(moveInput) > 0.01f)
            return;

        physicsBall.angularVelocity = Vector3.Lerp(
            physicsBall.angularVelocity,
            physicsBall.angularVelocity * (1f - angularDampingWhenIdle),
            Time.fixedDeltaTime * 5f
        );
    }

    private void FollowBall()
    {
        Vector3 targetPosition = physicsBall.position + Vector3.up * visualHeightOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followLerpSpeed * Time.fixedDeltaTime
        );

        if (cartVisual != null)
        {
            cartVisual.position = transform.position;
            cartVisual.rotation = transform.rotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (physicsBall != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 origin = physicsBall.worldCenterOfMass;
            Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
        }
    }
}