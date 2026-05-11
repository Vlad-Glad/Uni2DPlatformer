using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FinishPlatform : MonoBehaviour
{
    [SerializeField] private string nextLevelSceneName = "";

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCompleteLevel(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (TryCompleteLevel(collision.collider))
        {
            return;
        }

        TryCompleteLevel(collision.otherCollider);
    }

    private bool TryCompleteLevel(Collider2D other)
    {
        if (triggered || other == null || !IsPlayer(other))
        {
            return false;
        }

        LevelTimerManager timerManager = LevelTimerManager.Instance;

        if (timerManager == null)
        {
            timerManager = FindFirstObjectByType<LevelTimerManager>();
        }

        if (timerManager == null)
        {
            Debug.LogWarning("Finish reached, but no LevelTimerManager exists in the scene.");
            return false;
        }

        triggered = true;

        if (!string.IsNullOrWhiteSpace(nextLevelSceneName))
        {
            timerManager.SetNextLevelSceneName(nextLevelSceneName);
        }

        timerManager.CompleteLevel();
        return true;
    }

    private static bool IsPlayer(Collider2D other)
    {
        return other.CompareTag("Player") || other.GetComponentInParent<PlayerMovement>() != null;
    }
}
