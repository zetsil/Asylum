using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 900f; // Speed of movement
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

    void Update()
    {
        // Handle horizontal and vertical movement
        float moveInputX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveInputY = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // Calculate movement direction
        Vector2 movement = new Vector2(moveInputX, moveInputY).normalized * moveSpeed * Time.deltaTime;

        // Apply movement to the Rigidbody2D
        rb.linearVelocity = movement;

        // Adjust scale based on proximity to top and bottom points
        UpdateScaleBasedOnProximity();
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