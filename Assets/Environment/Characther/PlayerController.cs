using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float minScale = 0.5f;
    public float maxScale = 2f;
    public Transform topPoint;
    public Transform bottomPoint;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = false; // false means facing left (initial state)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rb.gravityScale = 0f;
        
        // Initialize the sprite to face left (default state)
        spriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        // Movement
        float moveInputX = Input.GetAxis("Horizontal");
        float moveInputY = Input.GetAxis("Vertical");
        
        Vector2 desiredVelocity = new Vector2(moveInputX, moveInputY).normalized * moveSpeed;
        rb.linearVelocity = desiredVelocity;

        // Animation control
        bool isMoving = desiredVelocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);

        // Flip logic for left-facing initial sprite
        if (moveInputX > 0.1f && !isFacingRight)  // Moving right but facing left
        {
            Flip();
        }
        else if (moveInputX < -0.1f && isFacingRight)  // Moving left but facing right
        {
            Flip();
        }

        UpdateScaleBasedOnProximity();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        // Since sprite initially faces left, we flipX when facing right
        spriteRenderer.flipX = isFacingRight;
    }

    private void UpdateScaleBasedOnProximity()
    {
        float currentY = transform.position.y;
        float topY = topPoint.position.y;
        float bottomY = bottomPoint.position.y;

        float normalizedDistance = Mathf.InverseLerp(topY, bottomY, currentY);
        normalizedDistance = Mathf.Clamp01(normalizedDistance);

        float newScale = Mathf.Lerp(minScale, maxScale, normalizedDistance);
        transform.localScale = new Vector3(newScale, newScale, 1f);
    }
}