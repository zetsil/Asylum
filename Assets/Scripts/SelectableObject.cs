using UnityEngine;

public class SelectableObject2D : MonoBehaviour
{
    public Color highlightColor = Color.yellow; // Color to indicate selection
    private Color originalColor; // Store the original color of the object
    private SpriteRenderer spriteRenderer; // SpriteRenderer component of the object

    void Start()
    {
        // Get the SpriteRenderer component of the object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if the SpriteRenderer component exists
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing on " + gameObject.name);
        }
        else
        {
            // Save the original color
            originalColor = spriteRenderer.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Player entered the trigger of " + gameObject.name);
            VisibilityController playerVisibility = other.GetComponent<VisibilityController>();
            if (playerVisibility != null)
            {
                playerVisibility.SetVisible();
            }

            // Apply the highlight effect
            if (spriteRenderer != null)
            {
                spriteRenderer.color = highlightColor;
                // Debug.Log("Highlight color applied to " + gameObject.name);
            }
            else
            {
                Debug.LogError("SpriteRenderer component is missing on " + gameObject.name);
            }
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the collider has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Get VisibilityController from the player and hide it
            VisibilityController playerVisibility = other.GetComponent<VisibilityController>();
            // Revert to the original color
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            else
            {
                Debug.LogError("SpriteRenderer component is missing on " + gameObject.name);
            }
            if (playerVisibility != null)
            {
                playerVisibility.SetHide();
            }
                
        }

    }
}