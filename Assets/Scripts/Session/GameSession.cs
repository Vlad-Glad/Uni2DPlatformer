using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [Header("Initial Run State")]
    [SerializeField] private int startingCurrentHealth = 3;
    [SerializeField] private int startingMaxHealth = 3;

    private int currentHealth;
    private int maxHealth;
    private string currentLevelName = "";

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public string CurrentLevelName => currentLevelName;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateSessionOnGameStart()
    {
        GetOrCreate();
    }

    public static GameSession GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameSession existingSession = FindFirstObjectByType<GameSession>();
        if (existingSession != null)
        {
            Instance = existingSession;
            return Instance;
        }

        GameObject sessionObject = new GameObject("GameSession");
        return sessionObject.AddComponent<GameSession>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetToInitialState();
    }

    public void StartNewRun(string firstLevelName)
    {
        ResetToInitialState();
        SetCurrentLevel(firstLevelName);
    }

    public void ResetToInitialState()
    {
        maxHealth = Mathf.Max(1, startingMaxHealth);
        currentHealth = Mathf.Clamp(startingCurrentHealth, 1, maxHealth);
        currentLevelName = "";
    }

    public void SetCurrentLevel(string levelName)
    {
        if (string.IsNullOrWhiteSpace(levelName))
        {
            return;
        }

        currentLevelName = levelName;
    }

    public void SetHealth(int newCurrentHealth, int newMaxHealth)
    {
        maxHealth = Mathf.Max(1, newMaxHealth);
        currentHealth = Mathf.Clamp(newCurrentHealth, 0, maxHealth);
    }

    public void ApplyHealingItem(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (currentHealth >= maxHealth)
        {
            maxHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        }
    }
}
