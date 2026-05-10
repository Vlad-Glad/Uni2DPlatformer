using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Flip")]
    [SerializeField] private bool isFacingRight = true;

    private Rigidbody2D rb;
    private Collider2D[] playerColliders;
    private float moveInput;
    private bool jumpPressed;
    private static PhysicsMaterial2D noFrictionMaterial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerColliders = GetComponents<Collider2D>();

        ApplyNoFrictionMaterial();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }

        Flip();
    }

    private void FixedUpdate()
    {
        Move();

        if (jumpPressed && IsGrounded())
        {
            Jump();
        }

        jumpPressed = false;
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void Flip()
    {
        if (moveInput == 0f)
        {
            return;
        }

        if ((isFacingRight && moveInput < 0f) || (!isFacingRight && moveInput > 0f))
        {
            isFacingRight = !isFacingRight;

            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void ApplyNoFrictionMaterial()
    {
        if (noFrictionMaterial == null)
        {
            noFrictionMaterial = new PhysicsMaterial2D("PlayerNoFriction")
            {
                friction = 0f,
                bounciness = 0f
            };
        }

        foreach (Collider2D playerCollider in playerColliders)
        {
            playerCollider.sharedMaterial = noFrictionMaterial;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}