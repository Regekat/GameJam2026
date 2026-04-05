using UnityEngine;

public class CartCollisionRelay : MonoBehaviour
{
    [SerializeField] private CartSoundManager cartSoundManager;

    private void Awake()
    {
        if (cartSoundManager == null)
        {
            cartSoundManager = GetComponentInParent<CartSoundManager>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Relay collision on {name} with {collision.gameObject.name}");

        if (cartSoundManager != null)
        {
            cartSoundManager.NotifyCollision();
        }
    }
}