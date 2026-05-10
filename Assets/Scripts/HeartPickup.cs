using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HeartPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    private bool collected;

    private void Reset()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        collected = true;
        playerHealth.Heal(healAmount);
        Destroy(gameObject);
    }
}
