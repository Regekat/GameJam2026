using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    public QueueManager QueueManager;
    public GameStateManager GameStateManager;

    private void OnTriggerEnter(Collider other)
    {
        CartInventory cart = other.GetComponent<CartInventory>();

        if (cart != null)
        {
            if (cart.HasAllRequiredItems())
            {
                Debug.Log("All groceries delivered! Success!");

                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.TriggerWin();
                }
            }
            else
            {
                Debug.Log("Missing some items!");
            }
        }
    }
}