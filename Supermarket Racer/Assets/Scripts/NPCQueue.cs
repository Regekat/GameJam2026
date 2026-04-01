using UnityEngine;
using System.Collections;

public class NPCQueue : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotateSpeed = 5f;
    public bool isMother;


    // QueueManager reads this to know when to send the next NPC
    public bool HasArrived { get; private set; } = false;
    public bool HasFinishedCashierRotation { get; private set; } = false;

    private Animator animator;
    private bool isMoving = false;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (animator != null)
            animator.SetFloat(SpeedHash, isMoving ? moveSpeed : 0f);
    }

    // walkMarker = the queue position (facing queue direction)
    // cashierMarker = separate transform facing the cashier, only passed for index 0
    public void MoveToPosition(Transform walkMarker, Transform cashierMarker = null)
    {
        HasArrived = false;
        HasFinishedCashierRotation = false;
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(walkMarker, cashierMarker));
    }

    IEnumerator MoveRoutine(Transform walkMarker, Transform cashierMarker)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, walkMarker.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                walkMarker.position,
                moveSpeed * Time.deltaTime
            );
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                walkMarker.rotation,
                rotateSpeed * Time.deltaTime
            );
            yield return null;
        }

        isMoving = false;
        transform.position = walkMarker.position;
        transform.rotation = walkMarker.rotation;

        HasArrived = true;

        if (cashierMarker != null)
        {
            yield return StartCoroutine(GradualRotate(cashierMarker.rotation));
            // Signal that rotation is done so QueueManager can remove immediately
            HasFinishedCashierRotation = true;
        }
    }

    IEnumerator GradualRotate(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    public void LeaveQueue(Transform leaveMarker)
    {
        HasArrived = false;
        StopAllCoroutines();
        StartCoroutine(WalkOff(leaveMarker));
    }

    IEnumerator WalkOff(Transform leaveMarker)
    {
        isMoving = false;
        yield return StartCoroutine(GradualRotate(leaveMarker.rotation));

        isMoving = true;

        while (Vector3.Distance(transform.position, leaveMarker.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                leaveMarker.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        Destroy(gameObject);
    }
}