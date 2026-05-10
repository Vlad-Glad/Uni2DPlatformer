using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("UI")]
    [SerializeField] private HealthUI healthUI;

    [Header("Hit Flash")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color flashColor = Color.red;

    private int currentHealth;
    private float lastDamageTime;
    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    private Coroutine flashCoroutine;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Start()
    {
        if (healthUI == null)
        {
            healthUI = HealthUI.Instance;
        }

        UpdateHealthUI();
    }

    public bool TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return false;
        }

        if (Time.time < lastDamageTime + damageCooldown)
        {
            return false;
        }

        lastDamageTime = Time.time;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
        PlayHitFlash();

        if (currentHealth <= 0)
        {
            Die();
        }

        return true;
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    public void AddMaxHealth(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        maxHealth += amount;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthUI == null)
        {
            healthUI = HealthUI.Instance;
        }

        if (healthUI == null)
        {
            return;
        }

        healthUI.SetHealth(currentHealth, maxHealth);
    }

    private void PlayHitFlash()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private void Die()
    {
        Debug.Log("Player died.");
    }
}
