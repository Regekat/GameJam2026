using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("Item Pools")]
    public List<string> possibleItems = new List<string>();
    public int numberToPick = 3;

    [Header("Runtime Data")]
    public List<string> requiredItems = new List<string>();
    private List<string> collectedItems = new List<string>();

    [Header("UI")]
    [SerializeField] private TMP_Text groceryListText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateRandomList();
        UpdateUI();
    }

    void GenerateRandomList()
    {
        requiredItems.Clear();
        collectedItems.Clear();

        if (possibleItems.Count == 0)
        {
            Debug.LogWarning("No possible items assigned!");
            return;
        }

        int safePickAmount = Mathf.Min(numberToPick, possibleItems.Count);

        List<string> tempPool = new List<string>(possibleItems);

        for (int i = 0; i < safePickAmount; i++)
        {
            int randomIndex = Random.Range(0, tempPool.Count);
            requiredItems.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex); // prevents duplicates
        }

        Debug.Log("Generated Grocery List:");
        foreach (var item in requiredItems)
        {
            Debug.Log(item);
        }
    }

    public void ItemCollected(string itemID)
    {
        if (requiredItems.Contains(itemID) && !collectedItems.Contains(itemID))
        {
            collectedItems.Add(itemID);
            UpdateUI();
        }
    }

    public bool AllItemsCollected()
    {
        return collectedItems.Count == requiredItems.Count;
    }

    void UpdateUI()
    {
        if (groceryListText == null) return;

        groceryListText.text = "";

        foreach (string item in requiredItems)
        {
            bool collected = collectedItems.Contains(item);
            groceryListText.text += collected ? $"✔ {item}\n" : $"• {item}\n";
        }
    }
}