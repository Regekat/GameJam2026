using UnityEngine;
using System.Collections;

public class NPCQueue : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotateSpeed = 5f;
    public bool isMother;

    public bool HasArrived { get; private set; } = false;
    public bool HasFinishedCashierRotation { get; private set; } = false;
    public bool HasStartedLeaving { get; private set; } = false;

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
        HasStartedLeaving = false;
        StopAllCoroutines();
        StartCoroutine(WalkOff(leaveMarker));
    }

    IEnumerator WalkOff(Transform leaveMarker)
    {
        isMoving = false;

        // Rotate to face leave marker
        yield return StartCoroutine(GradualRotate(leaveMarker.rotation));

        // Rotation done — signal queue to start shifting
        HasStartedLeaving = true;
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