using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of horizontal movement
    public float jumpForce = 10f; // Force applied when jumping
    private LayerMask groundLayer; // LayerMask to detect ground
    public float raycastDistance = 0.1f; // Distance to check for ground

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Initialize the groundLayer using the tag name "Ground"
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        // Handle horizontal movement
        float moveInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded) // Spacebar to jump
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // Check if the bottom of the player is touching the ground
        CheckIfGrounded();
    }

    private void CheckIfGrounded()
    {
        // Get the bottom center of the BoxCollider
        Vector2 boxColliderCenter = boxCollider.bounds.center;
        Vector2 boxColliderSize = boxCollider.bounds.size;
        Vector2 raycastOrigin = new Vector2(boxColliderCenter.x, boxColliderCenter.y - boxColliderSize.y / 2);

        // Shoot a raycast downward to check for ground
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, raycastDistance, groundLayer);

        // Check if the raycast hit something
        isGrounded = hit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the raycast in the editor for debugging
        if (boxCollider != null)
        {
            Vector2 boxColliderCenter = boxCollider.bounds.center;
            Vector2 boxColliderSize = boxCollider.bounds.size;
            Vector2 raycastOrigin = new Vector2(boxColliderCenter.x, boxColliderCenter.y - boxColliderSize.y / 2);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(raycastOrigin, raycastOrigin + Vector2.down * raycastDistance);
        }
    }
}