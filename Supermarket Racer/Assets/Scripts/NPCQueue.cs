using UnityEngine;
using System.Collections;

public class NPCQueue : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotateSpeed = 5f;
    public bool isMother;

    public void MoveToPosition(Transform marker, bool isFrontOfLine = false)
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(marker, isFrontOfLine));
    }

    IEnumerator MoveRoutine(Transform marker, bool isFrontOfLine)
    {
        // Gradually rotate to match marker's forward while walking
        Quaternion walkRotation = marker.rotation;
        while (Vector3.Distance(transform.position, marker.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                marker.position,
                moveSpeed * Time.deltaTime
            );
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                walkRotation,
                rotateSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = marker.position;
        transform.rotation = walkRotation;

        // Only after arriving at front of line, gradually rotate to face cashier
        if (isFrontOfLine)
        {
            yield return StartCoroutine(GradualRotate(marker.rotation));
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
        StopAllCoroutines();
        StartCoroutine(WalkOff(leaveMarker));
    }

    IEnumerator WalkOff(Transform leaveMarker)
    {
        // Gradually rotate to face leave marker's forward before walking
        yield return StartCoroutine(GradualRotate(leaveMarker.rotation));

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