using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Inventory and item management using Emerald AI framework.
/// Handles loot, item usage, and integration with VRIF.
/// </summary>
public class EmeraldItemSystem : MonoBehaviour
{
    public static EmeraldItemSystem Instance { get; private set; }

    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public int quantity;
        public string itemType; // weapon, consumable, key, etc.
        
        public InventoryItem(string name, int qty = 1, string type = "misc")
        {
            itemName = name;
            quantity = qty;
            itemType = type;
        }
    }

    private Dictionary<string, InventoryItem> inventory = new Dictionary<string, InventoryItem>();
    private int gold = 0;
    
    // Events
    public event Action<int> OnGoldChanged;
    public event Action<string, int> OnItemAdded;
    public event Action<string, int> OnItemRemoved;
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Gold Management
    public void AddGold(int amount)
    {
        if (amount > 0)
        {
            gold += amount;
            Debug.Log($"Gold added: {amount}. Total gold: {gold}");
            OnGoldChanged?.Invoke(gold);
            
            // Update UI if available
            if (VRIFUIManager.Instance != null)
            {
                VRIFUIManager.Instance.UpdateGold(gold);
            }
        }
    }
    
    public bool SpendGold(int amount)
    {
        if (amount > 0 && gold >= amount)
        {
            gold -= amount;
            Debug.Log($"Gold spent: {amount}. Remaining gold: {gold}");
            OnGoldChanged?.Invoke(gold);
            
            // Update UI if available
            if (VRIFUIManager.Instance != null)
            {
                VRIFUIManager.Instance.UpdateGold(gold);
            }
            return true;
        }
        return false;
    }
    
    public int GetGold() => gold;
    #endregion

    #region Item Management
    public void AddItem(string itemName, int quantity = 1, string itemType = "misc")
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName].quantity += quantity;
        }
        else
        {
            inventory[itemName] = new InventoryItem(itemName, quantity, itemType);
        }
        
        Debug.Log($"Item added: {itemName} x{quantity}");
        OnItemAdded?.Invoke(itemName, quantity);
        OnInventoryChanged?.Invoke();
        UpdateUI();
    }

    public bool UseItem(string itemName, int quantity = 1)
    {
        if (inventory.ContainsKey(itemName) && inventory[itemName].quantity >= quantity)
        {
            inventory[itemName].quantity -= quantity;
            
            if (inventory[itemName].quantity <= 0)
            {
                inventory.Remove(itemName);
            }
            
            Debug.Log($"Item used: {itemName} x{quantity}");
            OnItemRemoved?.Invoke(itemName, quantity);
            OnInventoryChanged?.Invoke();
            
            // Apply item effects based on type
            ApplyItemEffect(itemName);
            
            UpdateUI();
            return true;
        }
        else
        {
            Debug.LogWarning($"Cannot use item: {itemName} (insufficient quantity)");
            return false;
        }
    }
    
    public bool HasItem(string itemName, int quantity = 1)
    {
        return inventory.ContainsKey(itemName) && inventory[itemName].quantity >= quantity;
    }
    
    public int GetItemQuantity(string itemName)
    {
        return inventory.ContainsKey(itemName) ? inventory[itemName].quantity : 0;
    }
    #endregion

    #region Loot Distribution
    public void DistributeLoot(string[] lootItems)
    {
        foreach (var item in lootItems)
        {
            AddItem(item);
        }
        Debug.Log($"Loot distributed: {lootItems.Length} items");
    }
    
    public void DistributeLoot(Dictionary<string, int> lootTable)
    {
        foreach (var kvp in lootTable)
        {
            AddItem(kvp.Key, kvp.Value);
        }
    }
    #endregion

    #region Item Effects
    private void ApplyItemEffect(string itemName)
    {
        // Example item effects - expand based on your game's needs
        switch (itemName.ToLower())
        {
            case "health potion":
                // Heal the player
                Debug.Log("Health potion used - healing player");
                // TODO: Implement healing logic
                break;
                
            case "mana potion":
                // Restore mana
                Debug.Log("Mana potion used - restoring mana");
                // TODO: Implement mana restoration
                break;
                
            default:
                Debug.Log($"Item {itemName} has no special effect");
                break;
        }
    }
    #endregion

    #region UI Updates
    private void UpdateUI()
    {
        if (VRIFUIManager.Instance != null)
        {
            // Convert inventory to array format for UI
            List<string> itemList = new List<string>();
            foreach (var kvp in inventory)
            {
                itemList.Add($"{kvp.Value.itemName} x{kvp.Value.quantity}");
            }
            VRIFUIManager.Instance.UpdateInventory(itemList.ToArray());
        }
    }
    #endregion

    #region Save/Load
    public void SaveInventory()
    {
        // Save gold
        PlayerPrefs.SetInt("PlayerGold", gold);
        
        // Save inventory count
        PlayerPrefs.SetInt("InventoryCount", inventory.Count);
        
        // Save each item
        int index = 0;
        foreach (var kvp in inventory)
        {
            PlayerPrefs.SetString($"Item_{index}_Name", kvp.Value.itemName);
            PlayerPrefs.SetInt($"Item_{index}_Quantity", kvp.Value.quantity);
            PlayerPrefs.SetString($"Item_{index}_Type", kvp.Value.itemType);
            index++;
        }
        
        PlayerPrefs.Save();
        Debug.Log("Inventory saved");
    }
    
    public void LoadInventory()
    {
        // Load gold
        gold = PlayerPrefs.GetInt("PlayerGold", 0);
        
        // Load inventory
        inventory.Clear();
        int itemCount = PlayerPrefs.GetInt("InventoryCount", 0);
        
        for (int i = 0; i < itemCount; i++)
        {
            string itemName = PlayerPrefs.GetString($"Item_{i}_Name", "");
            int quantity = PlayerPrefs.GetInt($"Item_{i}_Quantity", 1);
            string itemType = PlayerPrefs.GetString($"Item_{i}_Type", "misc");
            
            if (!string.IsNullOrEmpty(itemName))
            {
                inventory[itemName] = new InventoryItem(itemName, quantity, itemType);
            }
        }
        
        Debug.Log($"Inventory loaded: {itemCount} items, {gold} gold");
        UpdateUI();
    }
    #endregion

    public string[] GetInventory()
    {
        List<string> items = new List<string>();
        foreach (var kvp in inventory)
        {
            items.Add($"{kvp.Value.itemName} x{kvp.Value.quantity}");
        }
        return items.ToArray();
    }
    
    public Dictionary<string, InventoryItem> GetFullInventory() => new Dictionary<string, InventoryItem>(inventory);
} 