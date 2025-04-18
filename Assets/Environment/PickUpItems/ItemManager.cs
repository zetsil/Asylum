using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;

    [Header("Item Collection")]
    public Transform itemContainer; // Parent for physical items
    public List<GameObject> collectedItems = new List<GameObject>();

    [Header("UI Display")]
    public Transform uiDisplayContainer; // Parent for UI elements
    public float iconSpacing = 30f;
    public Vector2 iconSize = new Vector2(80, 80);
    public float bottomOffset = 50f;

    private List<Image> itemIcons = new List<Image>();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Create containers if they don't exist
        if (itemContainer == null)
        {
            itemContainer = new GameObject("ItemContainer").transform;
            itemContainer.SetParent(transform);
        }

        if (uiDisplayContainer == null && GetComponentInChildren<Canvas>() != null)
        {
            uiDisplayContainer = new GameObject("UIDisplay").transform;
            uiDisplayContainer.SetParent(GetComponentInChildren<Canvas>().transform);
            uiDisplayContainer.localPosition = Vector3.zero;
        }
        InitializeUI(); // Ensure UI is created on awake
    }


    public static ItemManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemManager>();

                #if UNITY_EDITOR
                // Special editor-only handling
                if (_instance == null)
                {
                    if (Application.isPlaying)
                    {
                        Debug.LogWarning("[ItemManager] Auto-creating temporary instance for play mode");
                        GameObject go = new GameObject("ItemManager (Runtime-Temporary)");
                        _instance = go.AddComponent<ItemManager>();
                        _instance.InitializeUI(); // Initialize UI for auto-created instance
                    }
                    else
                    {
                        Debug.LogError("[ItemManager] Attempted to access in edit mode! Use editor-time fallbacks instead.");
                    }
                }
                #endif

                if (_instance == null && Application.isPlaying)
                {
                    Debug.LogError("[ItemManager] Critical error - no instance found in play mode!");
                }
            }
            return _instance;
        }
    }


    private void InitializeUI()
    {
        // Create canvas as a child if it doesn't exist
        if (uiDisplayContainer == null)
        {
            GameObject uiCanvas = new GameObject("ItemUICanvas");
            uiCanvas.transform.SetParent(transform);
            
            Canvas canvas = uiCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiCanvas.AddComponent<CanvasScaler>();
            uiCanvas.AddComponent<GraphicRaycaster>();

            // Create display container
            uiDisplayContainer = new GameObject("UIDisplay").transform;
            uiDisplayContainer.SetParent(uiCanvas.transform);
            uiDisplayContainer.localPosition = Vector3.zero;
        }
    }

    public void CollectItem(GameObject itemObject)
    {
        if (itemObject == null) return;
        
        // Add to physical collection
        collectedItems.Add(itemObject);
        itemObject.transform.SetParent(itemContainer);
        
        // Disable physics
        DisablePhysics(itemObject);
        itemObject.SetActive(false);
        
        // Create UI representation
        CreateItemIcon(itemObject);
        
        Debug.Log($"Collected item: {itemObject.name}");
    }

    private void DisablePhysics(GameObject obj)
    {
        var collider = obj.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
        
        var rigidbody = obj.GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = true;
    }

    private void CreateItemIcon(GameObject itemObject)
    {
        if (uiDisplayContainer == null) return;

        // Create new UI Image
        GameObject iconObj = new GameObject("ItemIcon");
        iconObj.transform.SetParent(uiDisplayContainer);
        
        Image iconImage = iconObj.AddComponent<Image>();
        itemIcons.Add(iconImage);

        // Get sprite from collected item
        SpriteRenderer sr = itemObject.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            iconImage.sprite = sr.sprite;
            iconImage.preserveAspect = true;
        }

        // Set up RectTransform
        RectTransform rt = iconObj.GetComponent<RectTransform>();
        rt.sizeDelta = iconSize;
        rt.localScale = Vector3.one;
        
        // Position icons
        PositionIcons();
    }

    private void PositionIcons()
    {
        if (uiDisplayContainer == null) return;

        float totalWidth = itemIcons.Count * (iconSize.x + iconSpacing);
        float startX = -totalWidth / 2f;
        float yPos = Screen.height/2 - bottomOffset; // Changed from bottom to top

        for (int i = 0; i < itemIcons.Count; i++)
        {
            RectTransform rt = itemIcons[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(
                    startX + i * (iconSize.x + iconSpacing),
                    yPos // Use the top position
                );
            }
        }
    }

    public bool HasItemWithID(string itemID)
    {
        foreach (GameObject item in collectedItems)
        {
            PickupItem pickupItem = item.GetComponent<PickupItem>();
            if (pickupItem != null && pickupItem.itemData != null)
            {
                if (pickupItem.itemData.getID() == itemID)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<GameObject> GetCollectedItems()
    {
        return new List<GameObject>(collectedItems);
    }

    public void ClearAllItems()
    {
        // Clear physical items
        foreach (var item in collectedItems)
        {
            if (item != null) 
                Destroy(item);
        }
        collectedItems.Clear();
        
        // Clear UI icons
        foreach (var icon in itemIcons)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }
        itemIcons.Clear();
    }

    // Gets the first item GameObject with the specified ID (returns null if not found)
    public GameObject GetItemWithID(string itemID)
    {
        foreach (GameObject item in collectedItems)
        {
            PickupItem pickupItem = item.GetComponent<PickupItem>();
            if (pickupItem != null && pickupItem.itemData != null)
            {
                if (pickupItem.itemData.getID() == itemID)
                {
                    return item;
                }
            }
        }
        return null;
    }

    // Removes the first item with the specified ID (returns true if found and removed)
    public bool RemoveItemWithID(string itemID)
    {
        for (int i = 0; i < collectedItems.Count; i++)
        {
            PickupItem pickupItem = collectedItems[i].GetComponent<PickupItem>();
            if (pickupItem != null && pickupItem.itemData != null)
            {
                if (pickupItem.itemData.getID() == itemID)
                {
                    // Remove from physical items
                    GameObject itemToRemove = collectedItems[i];
                    collectedItems.RemoveAt(i);
                    Destroy(itemToRemove);
                    
                    // Remove corresponding UI icon
                    if (i < itemIcons.Count)
                    {
                        Destroy(itemIcons[i].gameObject);
                        itemIcons.RemoveAt(i);
                    }
                    
                    // Reposition remaining icons
                    PositionIcons();
                    
                    return true;
                }
            }
        }
        return false;
    }
}