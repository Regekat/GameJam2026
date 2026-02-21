using System.Collections.Generic;
using UnityEngine;

public class CartInventory : MonoBehaviour
{
    private List<GroceryItem> itemsInCart = new List<GroceryItem>();

    private void OnTriggerEnter(Collider other)
    {
        GroceryItem item = other.GetComponent<GroceryItem>();

        if (item != null && !item.isCollected)
        {
            item.isCollected = true;
            itemsInCart.Add(item);

            ObjectiveManager.Instance.ItemCollected(item.itemID);

            Debug.Log($"Added {item.itemID} to cart");
        }
    }

    public bool HasAllRequiredItems()
    {
        return ObjectiveManager.Instance.AllItemsCollected();
    }
}