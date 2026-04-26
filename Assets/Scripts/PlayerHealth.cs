using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float damageCooldown = 0f;

    private int currentHealth;
    private float lastDamageTime;

    public int CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public bool TakeDamage(int damage)
    {

        if (Time.time < lastDamageTime + damageCooldown)
        {
            return false;
        }

        lastDamageTime = Time.time;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }

        return true;
    }

    private void Die()
    {
        Debug.Log("Player died");
    }
}
