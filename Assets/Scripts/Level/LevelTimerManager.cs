using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTimerManager : MonoBehaviour
{
    public static LevelTimerManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text continuePromptText;

    [Header("Interaction")]
    [SerializeField] private string nextLevelSceneName = "";
    [SerializeField] private string continuePrompt = "To start next level press E";

    [Header("Bonus")]
    [SerializeField] private float bonusTimeLimit = 30f;
    [SerializeField] private int bonusMaxHealth = 1;
    [SerializeField] private int bonusCurrentHealth = 1;

    private PlayerMovement playerMovement;
    private float elapsedTime;
    private bool timerStarted;
    private bool timerRunning;
    private bool finishReached;
    private bool isExitPromptVisible;
    private bool bonusApplied;

    public float ElapsedTime => elapsedTime;
    public bool IsRunning => timerRunning;
    public bool FinishReached => finishReached;
    public bool BonusApplied => bonusApplied;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        BindToPlayer(FindFirstObjectByType<PlayerMovement>());
        UpdateTimerText();
        SetContinuePromptVisible(false);
    }

    private void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void OnDestroy()
    {
        if (playerMovement != null)
        {
            playerMovement.FirstGrounded -= StartTimer;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ConfigureUi(TMP_Text newTimerText, TMP_Text newContinuePromptText)
    {
        if (timerText == null)
        {
            timerText = newTimerText;
        }

        if (continuePromptText == null)
        {
            continuePromptText = newContinuePromptText;
        }

        UpdateTimerText();
        SetContinuePromptVisible(isExitPromptVisible);
    }

    public void BindToPlayer(PlayerMovement newPlayerMovement)
    {
        if (playerMovement == newPlayerMovement)
        {
            return;
        }

        if (playerMovement != null)
        {
            playerMovement.FirstGrounded -= StartTimer;
        }

        playerMovement = newPlayerMovement;

        if (playerMovement == null)
        {
            return;
        }

        playerMovement.FirstGrounded += StartTimer;

        if (playerMovement.HasTouchedGround)
        {
            StartTimer();
        }
    }

    public void SetNextLevelSceneName(string sceneName)
    {
        nextLevelSceneName = sceneName;
    }

    public void StartTimer()
    {
        if (timerStarted || finishReached)
        {
            return;
        }

        timerStarted = true;
        timerRunning = true;
        elapsedTime = 0f;
        UpdateTimerText();
    }

    public void CompleteLevel()
    {
        if (finishReached)
        {
            return;
        }

        finishReached = true;
        timerRunning = false;

        TryApplyTimeBonus();
        UpdateTimerText();
    }

    public void ShowExitPrompt(string promptText)
    {
        if (!finishReached)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(promptText))
        {
            continuePrompt = promptText;
        }

        isExitPromptVisible = true;
        SetContinuePromptVisible(true);
    }

    public void HideExitPrompt()
    {
        isExitPromptVisible = false;
        SetContinuePromptVisible(false);
    }

    public void LoadNextLevel()
    {
        string sceneName = ResolveNextLevelSceneName();

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("No next gameplay scene is configured or available in Build Settings.");
            return;
        }

        Time.timeScale = 1f;

        GameSession.GetOrCreate().SetCurrentLevel(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    private void TryApplyTimeBonus()
    {
        if (bonusApplied || !timerStarted || elapsedTime > bonusTimeLimit)
        {
            return;
        }

        bonusApplied = true;

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ApplyHealthBonus(bonusMaxHealth, bonusCurrentHealth);
            return;
        }

        GameSession.GetOrCreate().ApplyHealthBonus(bonusMaxHealth, bonusCurrentHealth);
    }

    private string ResolveNextLevelSceneName()
    {
        if (!string.IsNullOrWhiteSpace(nextLevelSceneName))
        {
            if (Application.CanStreamedLevelBeLoaded(nextLevelSceneName))
            {
                return nextLevelSceneName;
            }

            Debug.LogWarning("Configured next level scene is not in Build Settings: " + nextLevelSceneName);
        }

        Scene activeScene = SceneManager.GetActiveScene();
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount <= 0)
        {
            return "";
        }

        int startIndex = activeScene.buildIndex >= 0 ? activeScene.buildIndex : -1;

        for (int offset = 1; offset <= sceneCount; offset++)
        {
            int buildIndex = startIndex >= 0 ? (startIndex + offset) % sceneCount : offset - 1;
            string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            string candidateName = Path.GetFileNameWithoutExtension(scenePath);

            if (string.IsNullOrWhiteSpace(candidateName))
            {
                continue;
            }

            if (candidateName == activeScene.name || IsNonGameplayScene(candidateName))
            {
                continue;
            }

            return candidateName;
        }

        return "";
    }

    private static bool IsNonGameplayScene(string sceneName)
    {
        return sceneName == "Menu" || sceneName == "DeathMenu";
    }

    private void SetContinuePromptVisible(bool isVisible)
    {
        if (continuePromptText == null)
        {
            return;
        }

        continuePromptText.text = continuePrompt;
        continuePromptText.gameObject.SetActive(isVisible);
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int hundredths = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        timerText.text = string.Format("Time: {0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);
    }
}
