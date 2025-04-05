using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float minScale = 0.5f; // Minimum scale when far away (up)
    public float maxScale = 2f; // Maximum scale when close (down)
    public Transform topPoint; // Reference to the top point
    public Transform bottomPoint; // Reference to the bottom point

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Disable gravity while keeping the Rigidbody2D as Dynamic
        rb.gravityScale = 0f;
    }

    // FixedUpdate is called at a fixed interval, ideal for physics calculations
    void FixedUpdate()
    {
        // --- Physics Movement ---
        // Handle horizontal and vertical movement input
        float moveInputX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveInputY = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // Calculate desired velocity based on input and speed
        // Normalizing ensures consistent speed regardless of direction (e.g., diagonal)
        Vector2 desiredVelocity = new Vector2(moveInputX, moveInputY).normalized * moveSpeed;

        // Apply movement directly by setting the Rigidbody2D's velocity
        // The physics engine handles applying this velocity over the fixed time step.
        // Note: This overrides existing velocity (like from impacts or gravity if applicable).
        // Consider rb.AddForce() for a different feel that integrates with other forces.
        rb.linearVelocity = desiredVelocity;

        // --- Other Updates ---
        // Adjust scale based on proximity to top and bottom points
        // Keep this here if the scale needs to be updated in sync with physics
        UpdateScaleBasedOnProximity();
    }

    void Update()
    {

    }

    private void UpdateScaleBasedOnProximity()
    {
        // Get the player's current Y position
        float currentY = transform.position.y;

        // Get the Y positions of the top and bottom points
        float topY = topPoint.position.y;
        float bottomY = bottomPoint.position.y;

        // Calculate the normalized distance between the player and the top/bottom points
        float normalizedDistance = Mathf.InverseLerp(topY, bottomY, currentY);

        // Clamp the normalized distance to ensure it stays within 0 and 1
        normalizedDistance = Mathf.Clamp01(normalizedDistance);

        // Interpolate the scale between minScale and maxScale based on normalizedDistance
        float newScale = Mathf.Lerp(minScale, maxScale, normalizedDistance);

        // Apply the new scale to the player
        transform.localScale = new Vector3(newScale, newScale, 1f);
    }
}