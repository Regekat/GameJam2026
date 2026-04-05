using UnityEngine;

[RequireComponent(typeof(CustomerPhysicsController))]
public class CustomerRagdollTrigger : MonoBehaviour
{
    private CustomerPhysicsController physicsController;
    private CartSoundManager cartSoundManager;

    void Awake()
    {
        physicsController = GetComponent<CustomerPhysicsController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (physicsController.IsRagdoll) return;
        if (!collision.gameObject.CompareTag("Cart")) return;

        float impact = collision.relativeVelocity.magnitude;

        if (cartSoundManager == null)
            cartSoundManager = collision.gameObject.GetComponentInParent<CartSoundManager>();
        if (cartSoundManager == null)
            cartSoundManager = collision.transform.root.GetComponentInChildren<CartSoundManager>();

        if (cartSoundManager != null)
            cartSoundManager.NotifyCollision();

        if (impact >= physicsController.ragdollForceThreshold)
        {
            Vector3 force = collision.relativeVelocity * 10f;
            Vector3 hitPoint = collision.contacts[0].point;
            physicsController.TriggerRagdoll(force, hitPoint);
        }
    }
}