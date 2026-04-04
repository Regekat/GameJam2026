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

    [Header("Queue Settings")]
    public float timeBetweenCustomers = 5f;
    public float delayBetweenEachStep = 0.4f;

    [Header("Markers")]
    public Transform leaveMarker;
    public Transform cashierFacingMarker;

    private List<NPCQueue> queueMembers = new List<NPCQueue>();

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
            Transform facingMarker = (i == 0) ? cashierFacingMarker : null;
            queueMembers[i].MoveToPosition(queuePositions[i], facingMarker);
        }
    }

    IEnumerator ProcessQueue()
    {
        while (queueMembers.Count > 0)
        {
            // Wait for front NPC to finish rotating to face cashier
            yield return new WaitUntil(() => queueMembers[0].HasFinishedCashierRotation);

            // This is the actual serve time — how long they stand at the counter
            yield return new WaitForSeconds(timeBetweenCustomers);

            if (GameStateManager.Instance != null && GameStateManager.Instance.HasGameEnded)
                yield break;

            // Store reference before removing from list
            NPCQueue leaving = queueMembers[0];
            RemoveFrontCustomer();

            if (GameStateManager.Instance != null && GameStateManager.Instance.HasGameEnded)
                yield break;

            // Wait until the leaving NPC has finished turning and started walking away
            yield return new WaitUntil(() => leaving == null || leaving.HasStartedLeaving);

            yield return StartCoroutine(StaggeredShift());
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

    IEnumerator StaggeredShift()
    {
        for (int i = 0; i < queueMembers.Count; i++)
        {
            Transform facingMarker = (i == 0) ? cashierFacingMarker : null;
            queueMembers[i].MoveToPosition(queuePositions[i], facingMarker);

            yield return new WaitUntil(() => queueMembers[i].HasArrived);
            yield return new WaitForSeconds(delayBetweenEachStep);
        }
    }
}