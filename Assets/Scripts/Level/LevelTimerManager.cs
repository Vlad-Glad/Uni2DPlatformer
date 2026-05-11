using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTimerManager : MonoBehaviour
{
    public static LevelTimerManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private LevelHUD levelHud;

    [Header("Interaction")]
    [SerializeField] private KeyCode continueKey = KeyCode.E;
    [SerializeField] private string nextLevelSceneName = "";
    [SerializeField] private string continuePrompt = "Press E to continue";

    private PlayerMovement playerMovement;
    private float elapsedTime;
    private bool timerStarted;
    private bool timerRunning;
    private bool finishReached;
    private bool bonusApplied;
    private bool isLoadingNextLevel;

    public float ElapsedTime => elapsedTime;
    public bool IsRunning => timerRunning;
    public bool FinishReached => finishReached;
    public bool BonusApplied => bonusApplied;
    private float BonusTimeLimit => levelHud != null ? levelHud.BonusTimeLimit : 30f;
    private int BonusHealthAmount => levelHud != null ? levelHud.BonusHealthAmount : 1;

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
        if (levelHud == null)
        {
            ConfigureHud(FindFirstObjectByType<LevelHUD>());
        }

        BindToPlayer(FindFirstObjectByType<PlayerMovement>());
        UpdateTimerText();
        UpdateBonusTargetText();
        HideResultPanel();
    }

    private void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }

        if (finishReached && !isLoadingNextLevel && Input.GetKeyDown(continueKey))
        {
            LoadNextLevel();
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

    public void ConfigureHud(LevelHUD newLevelHud)
    {
        if (newLevelHud == null)
        {
            return;
        }

        levelHud = newLevelHud;
        levelHud.EnsureUi();
        UpdateTimerText();
        UpdateBonusTargetText();

        if (finishReached)
        {
            ShowResultPanel();
        }
        else
        {
            HideResultPanel();
        }
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
        PauseGameplayAfterFinish();
        UpdateTimerText();
        ShowResultPanel();
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

        ShowResultPanel();
    }

    public void HideExitPrompt()
    {
    }

    public void LoadNextLevel()
    {
        isLoadingNextLevel = true;

        string sceneName = ResolveNextLevelSceneName();

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            isLoadingNextLevel = false;
            Debug.LogWarning("No next gameplay scene is configured or available in Build Settings.");
            return;
        }

        Time.timeScale = 1f;

        GameSession.GetOrCreate().SetCurrentLevel(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    private void TryApplyTimeBonus()
    {
        if (bonusApplied || !timerStarted || elapsedTime > BonusTimeLimit)
        {
            return;
        }

        int bonusHealthAmount = BonusHealthAmount;

        if (bonusHealthAmount <= 0)
        {
            return;
        }

        bonusApplied = true;

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ApplyHealingItem(bonusHealthAmount);
            return;
        }

        GameSession.GetOrCreate().ApplyHealingItem(bonusHealthAmount);
    }

    private void PauseGameplayAfterFinish()
    {
        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
        }

        if (playerMovement != null)
        {
            playerMovement.SetInputEnabled(false);
        }

        Time.timeScale = 0f;
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

    private void ShowResultPanel()
    {
        if (levelHud == null)
        {
            return;
        }

        levelHud.ShowResult(
            FormatTime(elapsedTime),
            FormatTime(BonusTimeLimit),
            bonusApplied,
            continuePrompt
        );
    }

    private void HideResultPanel()
    {
        if (levelHud != null)
        {
            levelHud.HideResult();
        }
    }

    private void UpdateTimerText()
    {
        if (levelHud == null)
        {
            return;
        }

        levelHud.SetTimerText(FormatTime(elapsedTime));
    }

    private void UpdateBonusTargetText()
    {
        if (levelHud == null)
        {
            return;
        }

        levelHud.SetBonusTargetText(FormatTime(BonusTimeLimit));
    }

    private static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int hundredths = Mathf.FloorToInt((time * 100f) % 100f);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);
    }
}
