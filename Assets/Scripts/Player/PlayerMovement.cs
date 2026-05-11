using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public event Action FirstGrounded;

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
    private bool hasTouchedGround;
    private bool inputEnabled = true;
    private static PhysicsMaterial2D noFrictionMaterial;

    public bool HasTouchedGround => hasTouchedGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerColliders = GetComponents<Collider2D>();

        ApplyNoFrictionMaterial();
    }

    private void Update()
    {
        if (!inputEnabled)
        {
            moveInput = 0f;
            jumpPressed = false;
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }

        Flip();
    }

    private void FixedUpdate()
    {
        bool isGrounded = IsGrounded();

        if (!hasTouchedGround && isGrounded)
        {
            hasTouchedGround = true;
            FirstGrounded?.Invoke();
        }

        Move();

        if (jumpPressed && isGrounded)
        {
            Jump();
        }

        jumpPressed = false;
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;

        if (enabled)
        {
            return;
        }

        moveInput = 0f;
        jumpPressed = false;

        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
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
