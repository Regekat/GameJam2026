using UnityEngine;

public class RollingCartBallController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody physicsBall;
    public Transform visualRoot;

    [Header("Follow")]
    public float visualHeightOffset = 0.75f;
    public float followLerpSpeed = 20f;
    public bool snapToBallOnStart = true;

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
    public LayerMask groundMask;
    public bool requireGroundedToDrive = true;

    private float moveInput;
    private float turnInput;
    private bool isGrounded;

    private float visualYaw;

    private void Awake()
    {
        if (physicsBall == null)
        {
            Debug.LogError("RollingCartBallController: PhysicsBall Rigidbody is not assigned.");
            enabled = false;
            return;
        }

        if (visualRoot == null)
        {
            Debug.LogError("RollingCartBallController: VisualRoot is not assigned.");
            enabled = false;
            return;
        }

        physicsBall.interpolation = RigidbodyInterpolation.Interpolate;
        physicsBall.maxAngularVelocity = 100f;
        physicsBall.linearVelocity = Vector3.zero;
        physicsBall.angularVelocity = Vector3.zero;

        visualYaw = visualRoot.eulerAngles.y;

        if (snapToBallOnStart)
        {
            Vector3 startPos = physicsBall.position + Vector3.up * visualHeightOffset;
            visualRoot.position = startPos;
        }
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        TurnVisual();
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

    private void TurnVisual()
    {
        visualYaw += turnInput * turnSpeed * Time.fixedDeltaTime;
        visualRoot.rotation = Quaternion.Euler(0f, visualYaw, 0f);
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

        Vector3 moveDirection = visualRoot.forward * Mathf.Sign(moveInput);
        Vector3 torqueAxis = Vector3.Cross(Vector3.up, moveDirection).normalized;

        physicsBall.AddTorque(
            torqueAxis * (driveTorque * Mathf.Abs(moveInput) * torqueScale),
            ForceMode.Acceleration
        );
    }

    private void DampSidewaysSlip()
    {
        Vector3 velocity = physicsBall.linearVelocity;
        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

        if (flatVelocity.sqrMagnitude < 0.0001f)
            return;

        Vector3 forward = visualRoot.forward;
        Vector3 right = visualRoot.right;

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

        visualRoot.position = Vector3.Lerp(
            visualRoot.position,
            targetPosition,
            followLerpSpeed * Time.fixedDeltaTime
        );
    }
}