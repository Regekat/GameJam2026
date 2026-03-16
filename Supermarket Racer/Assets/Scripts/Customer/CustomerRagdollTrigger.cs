using UnityEngine;

// Sits on the root GameObject alongside CustomerPhysicsController
// The root CapsuleCollider is what the cart actually hits during normal movement
[RequireComponent(typeof(CustomerPhysicsController))]
public class CustomerRagdollTrigger : MonoBehaviour
{
    private CustomerPhysicsController physicsController;

    void Awake()
    {
        physicsController = GetComponent<CustomerPhysicsController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (physicsController.IsRagdoll) return;
        if (!collision.gameObject.CompareTag("Cart")) return;

        float impact = collision.relativeVelocity.magnitude;

        Debug.Log($"[Ragdoll] Cart hit customer. Impact: {impact:F2}, Threshold: {physicsController.ragdollForceThreshold}");

        if (impact < physicsController.ragdollForceThreshold) return;

        Vector3 force = collision.relativeVelocity * 10f; // tweak multiplier as needed
        Vector3 hitPoint = collision.contacts[0].point;
        physicsController.TriggerRagdoll(force, hitPoint);
    }
}