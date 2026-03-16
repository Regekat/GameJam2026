using UnityEngine;
using UnityEngine.AI;

public class CustomerPhysicsController : MonoBehaviour
{
    [Header("Ragdoll Settings")]
    public float ragdollForceThreshold = 8f;

    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    private Collider rootCollider;
    private Animator animator;
    private NavMeshAgent agent;

    public bool IsRagdoll { get; private set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rootCollider = GetComponent<Collider>(); // root capsule only

        // Children only — excludes the root
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdoll(false);
    }

    public void SetRagdoll(bool state)
    {
        IsRagdoll = state;

        // Bone rigidbodies
        foreach (Rigidbody rb in ragdollRigidbodies)
            rb.isKinematic = !state;

        // Bone colliders
        foreach (Collider col in ragdollColliders)
            col.enabled = state;

        // Root capsule is the OPPOSITE — active during animation, off during ragdoll
        if (rootCollider != null)
            rootCollider.enabled = !state;

        if (animator != null) animator.enabled = !state;

        if (agent != null)
        {
            agent.enabled = !state;
            if (state) agent.ResetPath();
        }
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        if (IsRagdoll) return;

        SetRagdoll(true);

        // Find the hip bone (closest Rigidbody to root) and apply force
        Rigidbody closestRb = null;
        float closestDist = Mathf.Infinity;

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            float dist = Vector3.Distance(rb.position, hitPoint);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestRb = rb;
            }
        }

        if (closestRb != null)
            closestRb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
}