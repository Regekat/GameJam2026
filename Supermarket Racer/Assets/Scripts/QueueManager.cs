using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public List<Transform> queuePositions = new List<Transform>();
    public List<NPCQueue> queueMembers = new List<NPCQueue>();

    public float timeBetweenCustomers = 5f;

    void Start()
    {
        AssignInitialPositions();
        StartCoroutine(ProcessQueue());
    }

    void AssignInitialPositions()
    {
        for (int i = 0; i < queueMembers.Count; i++)
        {
            queueMembers[i].MoveToPosition(queuePositions[i]);
        }
    }

    IEnumerator ProcessQueue()
    {
        while (queueMembers.Count > 0)
        {
            yield return new WaitForSeconds(timeBetweenCustomers);

            RemoveFrontCustomer();
            ShiftQueueForward();
        }
    }

    void RemoveFrontCustomer()
    {
        if (queueMembers[0].isMother)
        {
            Debug.Log("Game Over");
            return;
        }

        NPCQueue front = queueMembers[0];
        queueMembers.RemoveAt(0);
        front.LeaveQueue();
    }

    void ShiftQueueForward()
    {
        for (int i = 0; i < queueMembers.Count; i++)
        {
            queueMembers[i].MoveToPosition(queuePositions[i]);
        }
    }
}
