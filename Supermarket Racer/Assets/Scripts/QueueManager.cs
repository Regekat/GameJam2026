using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("Queue Positions")]
    public List<Transform> queuePositions = new List<Transform>();

    [Header("Prefabs")]
    public GameObject npcPrefab;
    public GameObject motherPrefab;

    private List<NPCQueue> queueMembers = new List<NPCQueue>();

    public float timeBetweenCustomers = 5f;

    void Start()
    {
        SpawnQueue();
        AssignInitialPositions();
        StartCoroutine(ProcessQueue());
    }

    void SpawnQueue()
    {
        queueMembers.Clear();

        for (int i = 0; i < queuePositions.Count; i++)
        {
            GameObject npc;

            // Last position gets mother
            if (i == queuePositions.Count - 1)
            {
                npc = Instantiate(motherPrefab);
            }
            else
            {
                npc = Instantiate(npcPrefab);
            }

            NPCQueue npcQueue = npc.GetComponent<NPCQueue>();

            npc.transform.position = queuePositions[i].position;

            queueMembers.Add(npcQueue);
        }
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