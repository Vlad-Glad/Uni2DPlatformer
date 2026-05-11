using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class HouseLevelExit : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string nextLevelSceneName = "";
    [SerializeField] private string promptText = "To start next level press E";

    private LevelTimerManager timerManager;
    private bool playerInRange;

    private void Awake()
    {
        CircleCollider2D interactionCollider = GetComponent<CircleCollider2D>();
        interactionCollider.isTrigger = true;
    }

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        LevelTimerManager activeTimerManager = GetTimerManager();

        if (activeTimerManager == null || !activeTimerManager.FinishReached)
        {
            return;
        }

        activeTimerManager.ShowExitPrompt(promptText);

        if (Input.GetKeyDown(interactKey))
        {
            if (!string.IsNullOrWhiteSpace(nextLevelSceneName))
            {
                activeTimerManager.SetNextLevelSceneName(nextLevelSceneName);
            }

            activeTimerManager.LoadNextLevel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerInRange = false;

        LevelTimerManager activeTimerManager = GetTimerManager();
        if (activeTimerManager != null)
        {
            activeTimerManager.HideExitPrompt();
        }
    }

    private void OnDisable()
    {
        LevelTimerManager activeTimerManager = GetTimerManager();
        if (activeTimerManager != null)
        {
            activeTimerManager.HideExitPrompt();
        }
    }

    private LevelTimerManager GetTimerManager()
    {
        if (timerManager == null)
        {
            timerManager = LevelTimerManager.Instance;
        }

        if (timerManager == null)
        {
            timerManager = FindFirstObjectByType<LevelTimerManager>();
        }

        return timerManager;
    }

    private static bool IsPlayer(Collider2D other)
    {
        return other.CompareTag("Player") || other.GetComponentInParent<PlayerMovement>() != null;
    }
}
