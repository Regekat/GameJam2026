using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CustomerNavMeshWanderer : MonoBehaviour
{
    public enum CustomerState { Wandering, MovingToShelf, BrowsingShelf }

    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float wanderRadius = 10f;
    public float startDelayMax = 2f;

    [Header("Behaviour Weights")]
    [Range(0f, 1f)]
    public float shelfBrowseChance = 0.5f;

    [Header("Browsing")]
    public float minBrowseTime = 3f;
    public float maxBrowseTime = 8f;
    public float browseRotateSpeed = 2f;

    [Header("Shelf Spots")]
    public Transform[] shelfSpots;

    private NavMeshAgent agent;
    private CustomerPhysicsController physicsController;
    private Animator animator;
    private CustomerState currentState;
    private Transform targetShelfSpot;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    // Tracks which shelf spots are currently occupied across all NPCs
    private static readonly System.Collections.Generic.HashSet<Transform> occupiedSpots
        = new System.Collections.Generic.HashSet<Transform>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        physicsController = GetComponent<CustomerPhysicsController>();
        animator = GetComponentInChildren<Animator>();
        agent.speed = moveSpeed;

        float delay = Random.Range(0f, startDelayMax);
        StartCoroutine(DelayedStart(delay));
    }

    void OnDestroy()
    {
        // Make sure spot is freed if NPC is destroyed mid-browse
        ReleaseShelfSpot();
    }

    IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        DecideNextAction();
    }

    void Update()
    {
        if (physicsController != null && physicsController.IsRagdoll) return;
        if (!agent.isActiveAndEnabled) return;

        if (animator != null)
            animator.SetFloat(SpeedHash, agent.velocity.magnitude);

        switch (currentState)
        {
            case CustomerState.Wandering:
            case CustomerState.MovingToShelf:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (currentState == CustomerState.MovingToShelf)
                        StartCoroutine(BrowseShelf());
                    else
                        DecideNextAction();
                }
                break;

            case CustomerState.BrowsingShelf:
                if (targetShelfSpot != null)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetShelfSpot.rotation,
                        browseRotateSpeed * Time.deltaTime
                    );
                }
                break;
        }
    }

    void DecideNextAction()
    {
        if (shelfSpots != null && shelfSpots.Length > 0 && Random.value < shelfBrowseChance)
        {
            Transform freeSpot = GetFreeShelfSpot();

            if (freeSpot != null)
                StartCoroutine(GoToShelf(freeSpot));
            else
                StartCoroutine(Wander()); // all spots taken, just wander instead
        }
        else
        {
            StartCoroutine(Wander());
        }
    }

    // Finds a random unclaimed shelf spot, returns null if all are taken
    Transform GetFreeShelfSpot()
    {
        // Build a shuffled list of spots to pick randomly from available ones
        System.Collections.Generic.List<Transform> available
            = new System.Collections.Generic.List<Transform>();

        foreach (Transform spot in shelfSpots)
        {
            if (!occupiedSpots.Contains(spot))
                available.Add(spot);
        }

        if (available.Count == 0) return null;

        return available[Random.Range(0, available.Count)];
    }

    void ClaimShelfSpot(Transform spot)
    {
        targetShelfSpot = spot;
        occupiedSpots.Add(spot);
    }

    void ReleaseShelfSpot()
    {
        if (targetShelfSpot != null)
        {
            occupiedSpots.Remove(targetShelfSpot);
            targetShelfSpot = null;
        }
    }

    IEnumerator Wander()
    {
        currentState = CustomerState.Wandering;

        Vector3 dest = GetRandomNavMeshPoint();
        agent.SetDestination(dest);

        yield return new WaitUntil(() =>
            !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        float pause = Random.Range(0.5f, 2f);
        yield return new WaitForSeconds(pause);

        DecideNextAction();
    }

    IEnumerator GoToShelf(Transform spot)
    {
        currentState = CustomerState.MovingToShelf;
        ClaimShelfSpot(spot);

        agent.SetDestination(spot.position);

        yield return new WaitUntil(() =>
            !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        StartCoroutine(BrowseShelf());
    }

    IEnumerator BrowseShelf()
    {
        currentState = CustomerState.BrowsingShelf;
        agent.ResetPath();

        float browseTime = Random.Range(minBrowseTime, maxBrowseTime);
        yield return new WaitForSeconds(browseTime);

        ReleaseShelfSpot();
        DecideNextAction();
    }

    Vector3 GetRandomNavMeshPoint()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
            randomDir += transform.position;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
                return hit.position;
        }
        return transform.position;
    }

    public void OnRagdoll()
    {
        StopAllCoroutines();
        ReleaseShelfSpot(); // free the spot if they get ragdolled
        currentState = CustomerState.Wandering;
    }
}