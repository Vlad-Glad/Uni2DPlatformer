using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class LevelFlowBootstrap
{
    private const string TimerCanvasName = "LevelTimerCanvas";

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

        TMP_Text timerText;
        TMP_Text promptText;
        CreateTimerUi(out timerText, out promptText);

        timerManager.ConfigureUi(timerText, promptText);
        timerManager.BindToPlayer(player);
    }

    private static bool IsNonGameplayScene(string sceneName)
    {
        return sceneName == "Menu" || sceneName == "DeathMenu";
    }

    private static void CreateTimerUi(out TMP_Text timerText, out TMP_Text promptText)
    {
        GameObject existingCanvas = GameObject.Find(TimerCanvasName);

        if (existingCanvas != null)
        {
            timerText = existingCanvas.transform.Find("TimerText")?.GetComponent<TMP_Text>();
            promptText = existingCanvas.transform.Find("ContinuePromptText")?.GetComponent<TMP_Text>();

            if (timerText != null && promptText != null)
            {
                return;
            }

            Object.Destroy(existingCanvas);
        }

        GameObject canvasObject = new GameObject(TimerCanvasName);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        timerText = CreateText(
            "TimerText",
            canvasObject.transform,
            "Time: 00:00.00",
            34f,
            TextAlignmentOptions.Top,
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0f, -28f),
            new Vector2(360f, 70f)
        );

        promptText = CreateText(
            "ContinuePromptText",
            canvasObject.transform,
            "To start next level press E",
            40f,
            TextAlignmentOptions.Center,
            new Vector2(0.5f, 0.2f),
            new Vector2(0.5f, 0.2f),
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            new Vector2(560f, 90f)
        );
        promptText.gameObject.SetActive(false);
    }

    private static TMP_Text CreateText(
        string objectName,
        Transform parent,
        string text,
        float fontSize,
        TextAlignmentOptions alignment,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta
    )
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = Color.white;
        textComponent.enableAutoSizing = false;

        RectTransform rectTransform = textComponent.rectTransform;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;

        return textComponent;
    }
}
