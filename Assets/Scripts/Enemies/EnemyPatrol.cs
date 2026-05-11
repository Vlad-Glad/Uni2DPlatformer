using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private bool startsMovingRight = true;
    [SerializeField] private float stuckCheckTime = 0.25f;
    [SerializeField] private float minMoveDelta = 0.005f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private float ledgeCheckDistance = 0.45f;
    [SerializeField] private float ledgeCheckRadius = 0.08f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] enemyColliders;
    private float startX;
    private float lastX;
    private float stuckTimer;
    private int direction;
    private static PhysicsMaterial2D noFrictionMaterial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyColliders = GetComponents<Collider2D>();
        startX = transform.position.x;
        lastX = startX;
        direction = startsMovingRight ? 1 : -1;

        ApplyNoFrictionMaterial();
    }

    private void FixedUpdate()
    {
        if (!IsGrounded())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            ResetStuckCheck();
            return;
        }

        float offsetFromStart = transform.position.x - startX;

        if (offsetFromStart >= patrolDistance)
        {
            direction = -1;
        }
        else if (offsetFromStart <= -patrolDistance)
        {
            direction = 1;
        }

        if (!HasGroundAhead())
        {
            direction *= -1;
            ResetStuckCheck();
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        UpdateFacing();
        ReverseIfStuck();
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool HasGroundAhead()
    {
        if (groundCheck == null)
        {
            return false;
        }

        Vector2 ledgeCheckPosition = (Vector2)groundCheck.position + Vector2.right * direction * ledgeCheckDistance;
        return Physics2D.OverlapCircle(ledgeCheckPosition, ledgeCheckRadius, groundLayer);
    }

    private void UpdateFacing()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.flipX = direction > 0;
    }

    private void ReverseIfStuck()
    {
        float movedDistance = Mathf.Abs(transform.position.x - lastX);

        if (movedDistance < minMoveDelta)
        {
            stuckTimer += Time.fixedDeltaTime;

            if (stuckTimer >= stuckCheckTime)
            {
                direction *= -1;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastX = transform.position.x;
    }

    private void ResetStuckCheck()
    {
        stuckTimer = 0f;
        lastX = transform.position.x;
    }

    private void ApplyNoFrictionMaterial()
    {
        if (noFrictionMaterial == null)
        {
            noFrictionMaterial = new PhysicsMaterial2D("EnemyNoFriction")
            {
                friction = 0f,
                bounciness = 0f
            };
        }

        foreach (Collider2D enemyCollider in enemyColliders)
        {
            enemyCollider.sharedMaterial = noFrictionMaterial;
        }
    }

    private void OnDrawGizmosSelected()
    {
        float patrolCenterX = Application.isPlaying ? startX : transform.position.x;
        Vector3 leftPoint = new Vector3(patrolCenterX - patrolDistance, transform.position.y, transform.position.z);
        Vector3 rightPoint = new Vector3(patrolCenterX + patrolDistance, transform.position.y, transform.position.z);

        Gizmos.DrawLine(leftPoint, rightPoint);

        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

            Vector3 leftLedgePoint = groundCheck.position + Vector3.left * ledgeCheckDistance;
            Vector3 rightLedgePoint = groundCheck.position + Vector3.right * ledgeCheckDistance;

            Gizmos.DrawWireSphere(leftLedgePoint, ledgeCheckRadius);
            Gizmos.DrawWireSphere(rightLedgePoint, ledgeCheckRadius);
        }
    }
}
