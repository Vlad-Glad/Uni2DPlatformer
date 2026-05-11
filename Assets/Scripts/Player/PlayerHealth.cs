using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float damageCooldown = 0.5f;
    [SerializeField] private string deathMenuSceneName = "DeathMenu";

    [Header("Fall Death")]
    [SerializeField] private float fallDeathY = -8f;

    [Header("Hit Flash")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color flashColor = Color.red;

    private int currentHealth;
    private HealthUI healthUI;
    private float lastDamageTime;
    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    private Coroutine flashCoroutine;
    private bool isDead;
    private GameSession gameSession;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        gameSession = GameSession.GetOrCreate();
        ApplySessionHealth();

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
        if (gameSession == null)
        {
            gameSession = GameSession.GetOrCreate();
        }

        gameSession.SetCurrentLevel(SceneManager.GetActiveScene().name);
        ApplySessionHealth();

        if (healthUI == null)
        {
            healthUI = HealthUI.Instance;
        }

        UpdateHealthUI();
    }

    private void Update()
    {
        if (!isDead && transform.position.y <= fallDeathY)
        {
            Die();
        }
    }

    public bool TakeDamage(int damage)
    {
        if (isDead)
        {
            return false;
        }

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

        SaveHealthToSession();
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
        if (isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        SaveHealthToSession();
        UpdateHealthUI();
    }

    public void AddMaxHealth(int amount)
    {
        if (isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        maxHealth += amount;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        SaveHealthToSession();
        UpdateHealthUI();
    }

    public void ApplyHealingItem(int amount)
    {
        if (isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        if (currentHealth >= maxHealth)
        {
            AddMaxHealth(amount);
        }
        else
        {
            Heal(amount);
        }
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

    private void ApplySessionHealth()
    {
        if (gameSession == null)
        {
            gameSession = GameSession.GetOrCreate();
        }

        maxHealth = gameSession.MaxHealth;
        currentHealth = gameSession.CurrentHealth;
    }

    private void SaveHealthToSession()
    {
        if (gameSession == null)
        {
            gameSession = GameSession.GetOrCreate();
        }

        gameSession.SetHealth(currentHealth, maxHealth);
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
        if (isDead)
        {
            return;
        }

        isDead = true;
        currentHealth = 0;
        UpdateHealthUI();

        PlayerPrefs.SetString("LastPlayedLevel", SceneManager.GetActiveScene().name);

        if (gameSession == null)
        {
            gameSession = GameSession.GetOrCreate();
        }

        gameSession.ResetToInitialState();
        SceneManager.LoadScene(deathMenuSceneName);
    }
}
