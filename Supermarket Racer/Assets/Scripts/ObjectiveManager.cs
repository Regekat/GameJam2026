using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("Item Pools")]
    public List<string> possibleItems = new List<string>();
    public int numberToPick = 3;
    public int maxQuantityPerItem = 3;

    private Dictionary<string, int> requiredItems = new Dictionary<string, int>();
    private Dictionary<string, int> collectedItems = new Dictionary<string, int>();

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

        List<string> tempPool = new List<string>(possibleItems);
        int safePickAmount = Mathf.Min(numberToPick, tempPool.Count);

        for (int i = 0; i < safePickAmount; i++)
        {
            int randomIndex = Random.Range(0, tempPool.Count);
            string item = tempPool[randomIndex];
            tempPool.RemoveAt(randomIndex);

            int quantity = Random.Range(1, maxQuantityPerItem + 1);

            requiredItems[item] = quantity;
            collectedItems[item] = 0;
        }

        UpdateUI();
    }

    public void ItemAdded(string itemID)
    {
        if (!requiredItems.ContainsKey(itemID)) return;

        collectedItems[itemID]++;
        UpdateUI();
    }

    public void ItemRemoved(string itemID)
    {
        if (!requiredItems.ContainsKey(itemID)) return;

        collectedItems[itemID] = Mathf.Max(0, collectedItems[itemID] - 1);
        UpdateUI();
    }

    public bool AllItemsCollected()
    {
        foreach (var pair in requiredItems)
        {
            if (collectedItems[pair.Key] < pair.Value)
                return false;
        }

        return true;
    }

    void UpdateUI()
    {
        if (groceryListText == null) return;

        groceryListText.text = "";

        foreach (var pair in requiredItems)
        {
            string item = pair.Key;
            int required = pair.Value;
            int collected = collectedItems[item];

            bool completed = collected >= required;

            string line = $"{item} ({collected}/{required})";

            if (completed)
            {
                line = $"<s>{line}</s>";
            }

            groceryListText.text += line + "\n";
        }
    }
}