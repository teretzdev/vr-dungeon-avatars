using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inventory and item management using Emerald AI framework.
/// Handles loot, item usage, and integration with VRIF.
/// </summary>
public class EmeraldItemSystem : MonoBehaviour
{
    public static EmeraldItemSystem Instance { get; private set; }

    private List<string> inventory = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddItem(string itemName)
    {
        inventory.Add(itemName);
        Debug.Log($"Item added: {itemName}");
        VRIFUIManager.Instance.UpdateInventory(inventory.ToArray());
    }

    public void UseItem(string itemName)
    {
        if (inventory.Contains(itemName))
        {
            inventory.Remove(itemName);
            Debug.Log($"Item used: {itemName}");
            // Example: Apply item effect
        }
        else
        {
            Debug.LogWarning($"Item not found: {itemName}");
        }
        VRIFUIManager.Instance.UpdateInventory(inventory.ToArray());
    }

    public void DistributeLoot(string[] loot)
    {
        foreach (var item in loot)
            AddItem(item);
        Debug.Log("Loot distributed.");
    }

    public string[] GetInventory() => inventory.ToArray();
} 