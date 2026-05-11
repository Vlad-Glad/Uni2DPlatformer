using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string startMenuSceneName = "Menu";
    [SerializeField] private string firstLevelSceneName = "Level1_Layout";
    [SerializeField] private string deathMenuSceneName = "DeathMenu";

    [Header("Menu UI")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_Text menuTitleText;

    [Header("Buttons")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject exitToStartButton;

    private EventSystem persistentEventSystem;
    private InputSystemUIInputModule persistentInputModule;
    private GameObject hudCanvas;
    private bool isInGame;
    private bool isPaused;

    private void Awake()
    {
        if (transform.parent != null)
        {
            Debug.LogWarning("GameUIRoot should be a root GameObject. Move it to the top level of the Hierarchy.");
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        persistentEventSystem = GetComponentInChildren<EventSystem>(true);
        persistentInputModule = GetComponentInChildren<InputSystemUIInputModule>(true);
        hudCanvas = transform.Find("HUDCanvas")?.gameObject;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == startMenuSceneName)
        {
            ShowStartMenu();
        }
        else if (currentSceneName == deathMenuSceneName)
        {
            ShowExternalMenuScene();
        }
        else
        {
            EnterGameMode();
        }
    }

    private void Update()
    {
        if (!isInGame)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                ShowPauseMenu();
            }
        }
    }

    public void StartGame()
    {
        isInGame = true;
        isPaused = false;

        SetPersistentUiInputEnabled(false);
        SetHudVisible(true);
        menuPanel.SetActive(false);

        Time.timeScale = 1f;

        GameSession.GetOrCreate().StartNewRun(firstLevelSceneName);
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void ResumeGame()
    {
        if (!isInGame)
        {
            return;
        }

        isPaused = false;

        SetPersistentUiInputEnabled(false);
        SetHudVisible(true);
        menuPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void ShowPauseMenu()
    {
        if (!isInGame)
        {
            return;
        }

        isPaused = true;

        SetPersistentUiInputEnabled(true);
        menuPanel.SetActive(true);

        if (menuTitleText != null)
        {
            menuTitleText.text = "Paused";
        }

        startButton.SetActive(false);
        resumeButton.SetActive(true);
        exitToStartButton.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ReturnToStartMenu()
    {
        isInGame = false;
        isPaused = false;

        Time.timeScale = 1f;

        SceneManager.LoadScene(startMenuSceneName);
    }

    private void ShowStartMenu()
    {
        isInGame = false;
        isPaused = true;

        SetPersistentUiInputEnabled(true);
        SetHudVisible(false);
        menuPanel.SetActive(true);

        if (menuTitleText != null)
        {
            menuTitleText.text = "Island Run";
        }

        startButton.SetActive(true);
        resumeButton.SetActive(false);
        exitToStartButton.SetActive(false);

        Time.timeScale = 0f;
    }

    private void EnterGameMode()
    {
        isInGame = true;
        isPaused = false;

        SetPersistentUiInputEnabled(false);
        SetHudVisible(true);
        menuPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == startMenuSceneName)
        {
            ShowStartMenu();
        }
        else if (scene.name == deathMenuSceneName)
        {
            ShowExternalMenuScene();
        }
        else
        {
            EnterGameMode();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void SetPersistentUiInputEnabled(bool enabled)
    {
        if (persistentInputModule != null)
        {
            persistentInputModule.enabled = enabled;
        }

        if (persistentEventSystem != null)
        {
            persistentEventSystem.enabled = enabled;
        }
    }

    private void SetHudVisible(bool visible)
    {
        if (hudCanvas != null)
        {
            hudCanvas.SetActive(visible);
        }
    }

    private void ShowExternalMenuScene()
    {
        isInGame = false;
        isPaused = true;

        SetPersistentUiInputEnabled(false);
        SetHudVisible(false);
        menuPanel.SetActive(false);

        Time.timeScale = 0f;
    }
}
