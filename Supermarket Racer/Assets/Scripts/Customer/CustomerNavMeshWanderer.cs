using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class CustomerNavMeshWanderer : MonoBehaviour
{
    public enum CustomerState { Wandering, MovingToShelf, BrowsingShelf }

    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float wanderRadius = 10f;
    public float startDelayMax = 2f;   // stagger offset — each NPC waits a random amount before first path

    [Header("Behaviour Weights")]
    [Range(0f, 1f)]
    public float shelfBrowseChance = 0.5f; // 50% chance to head to a shelf instead of wandering

    [Header("Browsing")]
    public float minBrowseTime = 3f;
    public float maxBrowseTime = 8f;
    public float browseRotateSpeed = 2f;  // how fast they turn to face the shelf

    [Header("Shelf Spots")]
    public Transform[] shelfSpots; // assign ShelfSpot transforms in Inspector

    private NavMeshAgent agent;
    private CustomerPhysicsController physicsController;
    private Animator animator;
    private CustomerState currentState;
    private Transform targetShelfSpot;

    // Animator parameter hash — avoids string lookups every frame
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        physicsController = GetComponent<CustomerPhysicsController>();
        animator = GetComponent<Animator>();
        agent.speed = moveSpeed;

        // Stagger: each NPC waits a random delay before starting pathfinding
        // This spreads NavMesh path calculations across multiple frames
        float delay = Random.Range(0f, startDelayMax);
        StartCoroutine(DelayedStart(delay));
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

        // Drive the walk animation based on actual movement speed
        if (animator != null)
            animator.SetFloat(SpeedHash, agent.velocity.magnitude);

        switch (currentState)
        {
            case CustomerState.Wandering:
            case CustomerState.MovingToShelf:
                // Check if we've arrived
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
                    // Use the ShelfSpot's forward direction directly
                    // instead of calculating direction to its position
                    Quaternion targetRot = targetShelfSpot.rotation;
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation, targetRot,
                        browseRotateSpeed * Time.deltaTime
                    );
                }
                break;
        }
    }

    // Randomly pick: wander or go browse a shelf
    void DecideNextAction()
    {
        if (shelfSpots != null && shelfSpots.Length > 0 && Random.value < shelfBrowseChance)
            StartCoroutine(GoToShelf());
        else
            StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        currentState = CustomerState.Wandering;

        Vector3 dest = GetRandomNavMeshPoint();
        agent.SetDestination(dest);

        // Wait until arrived, then pause briefly before deciding again
        yield return new WaitUntil(() =>
            !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        float pause = Random.Range(0.5f, 2f);
        yield return new WaitForSeconds(pause);

        DecideNextAction();
    }

    IEnumerator GoToShelf()
    {
        currentState = CustomerState.MovingToShelf;

        // Pick a random shelf spot
        targetShelfSpot = shelfSpots[Random.Range(0, shelfSpots.Length)];
        agent.SetDestination(targetShelfSpot.position);

        yield return new WaitUntil(() =>
            !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        StartCoroutine(BrowseShelf());
    }

    IEnumerator BrowseShelf()
    {
        currentState = CustomerState.BrowsingShelf;
        agent.ResetPath(); // stop moving

        float browseTime = Random.Range(minBrowseTime, maxBrowseTime);
        yield return new WaitForSeconds(browseTime);

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

    // Called externally when ragdoll triggers — cleanly exits any running coroutine
    public void OnRagdoll()
    {
        StopAllCoroutines();
        currentState = CustomerState.Wandering;
    }
}