using System.Collections.Generic;
using UnityEngine;

public class CartInventory : MonoBehaviour
{
    private List<GroceryItem> itemsInCart = new List<GroceryItem>();

    private void OnTriggerEnter(Collider other)
    {
        GroceryItem item = other.GetComponent<GroceryItem>();

        if (item != null && !itemsInCart.Contains(item))
        {
            itemsInCart.Add(item);
            ObjectiveManager.Instance.ItemAdded(item.itemID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GroceryItem item = other.GetComponent<GroceryItem>();

        if (item != null && itemsInCart.Contains(item))
        {
            itemsInCart.Remove(item);
            ObjectiveManager.Instance.ItemRemoved(item.itemID);
        }
    }

    public bool HasAllRequiredItems()
    {
        return ObjectiveManager.Instance.AllItemsCollected();
    }
}