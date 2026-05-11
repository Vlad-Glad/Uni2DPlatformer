using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelFlowBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void SetupActiveScene()
    {
        SetupScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SetupScene(scene);
    }

    private static void SetupScene(Scene scene)
    {
        if (IsNonGameplayScene(scene.name))
        {
            return;
        }

        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();

        if (player == null)
        {
            return;
        }

        LevelTimerManager timerManager = Object.FindFirstObjectByType<LevelTimerManager>();

        if (timerManager == null)
        {
            GameObject timerManagerObject = new GameObject("LevelTimerManager");
            timerManager = timerManagerObject.AddComponent<LevelTimerManager>();
        }

        LevelHUD levelHud = Object.FindFirstObjectByType<LevelHUD>();

        if (levelHud == null)
        {
            GameObject levelHudObject = new GameObject("LevelHUD");
            levelHud = levelHudObject.AddComponent<LevelHUD>();
        }

        levelHud.EnsureUi();
        timerManager.ConfigureHud(levelHud);
        timerManager.BindToPlayer(player);
    }

    private static bool IsNonGameplayScene(string sceneName)
    {
        return sceneName == "Menu" || sceneName == "DeathMenu";
    }
}
