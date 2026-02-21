using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CartInventory cart = other.GetComponent<CartInventory>();

        if (cart != null)
        {
            if (cart.HasAllRequiredItems())
            {
                Debug.Log("All groceries delivered! Success!");
                // Trigger win
            }
            else
            {
                Debug.Log("Missing some items!");
            }
        }
    }
}