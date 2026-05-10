using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        DealDamage(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamage(collision.gameObject);
    }

    private void DealDamage(GameObject target)
    {
        PlayerHealth playerHealth = target.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        playerHealth.TakeDamage(damage);
    }
}
