using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float minScale = 0.5f;
    public float maxScale = 2f;
    public Transform topPoint;
    public Transform bottomPoint;

    // Public GameObject to be flipped instead of SpriteRenderer
    public GameObject characterVisuals; // Assign your child GameObject here

    private Rigidbody2D rb;
    public Animator animator; // Ensure this animator is on the characterVisuals or its child
    private bool isFacingRight = false; // false means facing left (initial state)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ensure characterVisuals is assigned
        if (characterVisuals == null)
        {
            Debug.LogError("Character Visuals GameObject is not assigned in PlayerController!");
            // Optionally, try to find it as a child if not assigned
            // characterVisuals = transform.Find("YourVisualsChildName").gameObject;
        }

        // Assuming animator is on the characterVisuals GameObject or one of its children
        if (animator == null && characterVisuals != null)
        {
            animator = characterVisuals.GetComponent<Animator>();
            if (animator == null)
            {
                animator = characterVisuals.GetComponentInChildren<Animator>();
            }
        }
        if (animator == null)
        {
            Debug.LogError("Animator not found on PlayerController or its Character Visuals!");
        }
        
        rb.gravityScale = 0f;
        
        // Initialize the character visuals to face left (default state)
        // Assuming your visual asset is designed to face right by default,
        // and needs to be flipped on the Y-axis to face left initially.
        // If your visual asset faces left by default, you might not need this initial flip.
        if (characterVisuals != null)
        {
            characterVisuals.transform.localScale = new Vector3(Mathf.Abs(characterVisuals.transform.localScale.x) * -1, characterVisuals.transform.localScale.y, characterVisuals.transform.localScale.z);
            isFacingRight = false; // Confirming the initial state
        }
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
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }

        // Flip logic for left-facing initial character visuals
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
        if (characterVisuals == null) return;

        isFacingRight = !isFacingRight;

        // Get the current local scale of the visuals GameObject
        Vector3 currentScale = characterVisuals.transform.localScale;

        // Flip the X-scale to negative to mirror the object
        // We use Mathf.Abs to ensure we always flip from a positive base,
        // preventing double negatives or issues if scale was initially negative.
        if (isFacingRight)
        {
            characterVisuals.transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
        else // Facing left
        {
            characterVisuals.transform.localScale = new Vector3(Mathf.Abs(currentScale.x) * -1, currentScale.y, currentScale.z);
        }
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