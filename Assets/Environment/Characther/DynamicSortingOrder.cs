using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    public string envPropsSortingLayer = "EnvProps"; // Sorting layer for environment props
    public int playerBaseOrder = 0; // Base sorting order for the player
    public float yOffset = 0f; // Offset to align player and props
    public float proximityDistance = 10f; // Distance to check for nearby props

    private SpriteRenderer playerSpriteRenderer;

    void Start()
    {
        // Get the player's SpriteRenderer component
        playerSpriteRenderer = GetComponent<SpriteRenderer>();

        if (playerSpriteRenderer == null)
        {
            Debug.LogError("Player SpriteRenderer not found!");
        }
    }

    void Update()
    {
        // Update the player's sorting order based on Y position
        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        // Find all SpriteRenderers in the scene
        SpriteRenderer[] allRenderers = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer renderer in allRenderers)
        {
            // Check if the renderer is on the EnvProps sorting layer
            if (renderer.sortingLayerName == envPropsSortingLayer)
            {
                // Calculate the distance between the player and the prop
                float distance = Vector2.Distance(transform.position, renderer.transform.position);

                // Only update sorting order if the prop is within the proximity distance
                if (distance <= proximityDistance)
                {
                    // Compare the player's Y position (with offset) with the prop's Y position
                    if (transform.position.y + yOffset < renderer.transform.position.y)
                    {
                        // Player is below the prop, render player above the prop (higher sortingOrder)
                        playerSpriteRenderer.sortingOrder = renderer.sortingOrder + 1;
                    }
                    else
                    {
                        // Player is above the prop, render player below the prop (lower sortingOrder)
                        playerSpriteRenderer.sortingOrder = renderer.sortingOrder - 1;
                    }
                }
            }
        }

        // Ensure the player's sorting order is always above the base order
        playerSpriteRenderer.sortingOrder = Mathf.Max(playerSpriteRenderer.sortingOrder, playerBaseOrder);
    }
}