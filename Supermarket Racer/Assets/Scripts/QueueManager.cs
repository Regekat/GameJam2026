using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("Queue Positions")]
    public List<Transform> queuePositions = new List<Transform>();

    [Header("NPC Variants")]
    public List<GameObject> npcPrefabs = new List<GameObject>();

    [Header("Mother")]
    public GameObject motherPrefab;

    [Header("Leave Marker")]
    public Transform leaveMarker;

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
            GameObject npc = (i == queuePositions.Count - 1)
                ? Instantiate(motherPrefab)
                : GetRandomNPC();

            npc.transform.position = queuePositions[i].position;
            queueMembers.Add(npc.GetComponent<NPCQueue>());
        }
    }

    GameObject GetRandomNPC()
    {
        if (npcPrefabs == null || npcPrefabs.Count == 0)
        {
            Debug.LogWarning("No NPC prefabs assigned!");
            return null;
        }
        return Instantiate(npcPrefabs[Random.Range(0, npcPrefabs.Count)]);
    }

    void AssignInitialPositions()
    {
        for (int i = 0; i < queueMembers.Count; i++)
        {
            bool isFront = (i == 0);
            queueMembers[i].MoveToPosition(queuePositions[i], isFront);
        }
    }

    IEnumerator ProcessQueue()
    {
        while (queueMembers.Count > 0)
        {
            yield return new WaitForSeconds(timeBetweenCustomers);

            if (GameStateManager.Instance != null && GameStateManager.Instance.HasGameEnded)
                yield break;

            RemoveFrontCustomer();

            if (GameStateManager.Instance != null && GameStateManager.Instance.HasGameEnded)
                yield break;

            ShiftQueueForward();
        }
    }

    void RemoveFrontCustomer()
    {
        if (queueMembers.Count == 0) return;

        if (queueMembers[0].isMother)
        {
            Debug.Log("Game Over");
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.TriggerLoss();
            return;
        }

        NPCQueue front = queueMembers[0];
        queueMembers.RemoveAt(0);
        front.LeaveQueue(leaveMarker);
    }

    void ShiftQueueForward()
    {
        for (int i = 0; i < queueMembers.Count; i++)
        {
            bool isFront = (i == 0);
            queueMembers[i].MoveToPosition(queuePositions[i], isFront);
        }
    }
}