using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string id;
    public string name;
    public string description;
    public GameObject itemObject; // Reference to the actual GameObject
    public Sprite icon; // For UI representation

    public InventoryItem(string id, string name, string description, GameObject itemObject, Sprite icon)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.itemObject = itemObject;
        this.icon = icon;
        
        // Disable the object when added to inventory
        if (itemObject != null)
        {
            itemObject.SetActive(false);
        }
    }

    // Spawn the item in the world
    public void DropItem(Vector3 position, Quaternion rotation)
    {
        if (itemObject != null)
        {
            itemObject.transform.position = position;
            itemObject.transform.rotation = rotation;
            itemObject.SetActive(true);
        }
    }
}

public class GameObjectInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public event Action OnInventoryChanged;
     // Add an item to the inventory
    public bool AddItem(InventoryItem newItem)
    {
        if (items.Count >= maxSlots)
        {
            Debug.LogWarning("Inventory is full!");
            return false;
        }

        if (HasItem(newItem.id))
        {
            Debug.LogWarning($"Item {newItem.id} already exists in inventory!");
            return false;
        }

        items.Add(newItem);
        OnInventoryChanged?.Invoke();
        return true;
    }

    // Check if inventory has an item
    public bool HasItem(string itemId)
    {
        return items.Exists(item => item.id == itemId);
    }
    
}
