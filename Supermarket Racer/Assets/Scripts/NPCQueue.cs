using UnityEngine;
using System.Collections;

public class NPCQueue : MonoBehaviour
{
    public float moveSpeed = 2f;

    public bool isMother;

    public void MoveToPosition(Transform targetPosition)
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(targetPosition));
    }

    IEnumerator MoveRoutine(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    public void LeaveQueue()
    {
        // walk away
        StartCoroutine(WalkOff());
    }

    IEnumerator WalkOff()
    {
        Vector3 exitPoint = transform.position + Vector3.right * 5f;

        while (Vector3.Distance(transform.position, exitPoint) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                exitPoint,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}
