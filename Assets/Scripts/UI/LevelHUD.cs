using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelHUD : MonoBehaviour
{
    [Header("Bonus Settings")]
    [SerializeField] private float bonusTimeLimit = 30f;
    [SerializeField] private int bonusHealthAmount = 1;

    private TMP_Text timerText;
    private TMP_Text bonusTargetText;
    private GameObject resultPanel;
    private TMP_Text resultTitleText;
    private TMP_Text resultTimeText;
    private TMP_Text resultBonusTargetText;
    private TMP_Text resultBonusText;
    private TMP_Text resultContinueText;

    public float BonusTimeLimit => Mathf.Max(0f, bonusTimeLimit);
    public int BonusHealthAmount => Mathf.Max(0, bonusHealthAmount);

    private void Awake()
    {
        EnsureUi();
        HideResult();
    }

    public void EnsureUi()
    {
        Transform canvasTransform = EnsureCanvasTransform();
        BindExistingReferences(canvasTransform);

        if (timerText == null)
        {
            timerText = CreateText(
                "TimerText",
                canvasTransform,
                "Time: 00:00.00",
                34f,
                TextAlignmentOptions.Top,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -28f),
                new Vector2(360f, 48f)
            );
        }

        if (bonusTargetText == null)
        {
            bonusTargetText = CreateText(
                "BonusTargetText",
                canvasTransform,
                GetBonusRewardLabel() + ": under 00:30.00",
                24f,
                TextAlignmentOptions.Top,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -72f),
                new Vector2(460f, 42f)
            );
        }

        if (resultPanel == null)
        {
            resultPanel = CreateResultPanel(canvasTransform);
        }

        EnsureResultTextReferences();
    }

    public void SetTimerText(string formattedTime)
    {
        EnsureUi();

        if (timerText != null)
        {
            timerText.text = "Time: " + formattedTime;
        }
    }

    public void SetBonusTargetText(string formattedTargetTime)
    {
        EnsureUi();

        if (bonusTargetText != null)
        {
            bonusTargetText.text = GetBonusRewardLabel() + ": under " + formattedTargetTime;
        }
    }

    public void ShowResult(string formattedTime, string formattedTargetTime, bool bonusReceived, string continuePrompt)
    {
        EnsureUi();

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultTitleText != null)
        {
            resultTitleText.text = "Level completed";
        }

        if (resultTimeText != null)
        {
            resultTimeText.text = "Time: " + formattedTime;
        }

        if (resultBonusTargetText != null)
        {
            resultBonusTargetText.text = GetBonusRewardLabel() + " target: " + formattedTargetTime;
        }

        if (resultBonusText != null)
        {
            resultBonusText.text = bonusReceived
                ? GetBonusRewardLabel() + ": received"
                : GetBonusRewardLabel() + ": not received";
        }

        if (resultContinueText != null)
        {
            resultContinueText.text = continuePrompt;
        }
    }

    public void HideResult()
    {
        EnsureUi();

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    private Transform EnsureCanvasTransform()
    {
        Canvas existingCanvas = GetComponentInChildren<Canvas>(true);

        if (existingCanvas != null)
        {
            return existingCanvas.transform;
        }

        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        return canvasObject.transform;
    }

    private void BindExistingReferences(Transform canvasTransform)
    {
        if (timerText == null)
        {
            timerText = canvasTransform.Find("TimerText")?.GetComponent<TMP_Text>();
        }

        if (bonusTargetText == null)
        {
            bonusTargetText = canvasTransform.Find("BonusTargetText")?.GetComponent<TMP_Text>();
        }

        if (resultPanel == null)
        {
            Transform resultPanelTransform = canvasTransform.Find("LevelResultPanel");
            resultPanel = resultPanelTransform != null ? resultPanelTransform.gameObject : null;
        }
    }

    private void EnsureResultTextReferences()
    {
        if (resultPanel == null)
        {
            return;
        }

        Transform resultPanelTransform = resultPanel.transform;

        if (resultTitleText == null)
        {
            resultTitleText = resultPanelTransform.Find("ResultTitleText")?.GetComponent<TMP_Text>();
        }

        if (resultTimeText == null)
        {
            resultTimeText = resultPanelTransform.Find("ResultTimeText")?.GetComponent<TMP_Text>();
        }

        if (resultBonusTargetText == null)
        {
            resultBonusTargetText = resultPanelTransform.Find("ResultBonusTargetText")?.GetComponent<TMP_Text>();
        }

        if (resultBonusText == null)
        {
            resultBonusText = resultPanelTransform.Find("ResultBonusText")?.GetComponent<TMP_Text>();
        }

        if (resultContinueText == null)
        {
            resultContinueText = resultPanelTransform.Find("ResultContinueText")?.GetComponent<TMP_Text>();
        }

        if (resultTitleText == null)
        {
            resultTitleText = CreateText(
                "ResultTitleText",
                resultPanelTransform,
                "Level completed",
                46f,
                TextAlignmentOptions.Center,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -52f),
                new Vector2(0f, 64f)
            );
        }

        if (resultTimeText == null)
        {
            resultTimeText = CreateText(
                "ResultTimeText",
                resultPanelTransform,
                "Time: 00:00.00",
                32f,
                TextAlignmentOptions.Center,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -118f),
                new Vector2(0f, 48f)
            );
        }

        if (resultBonusTargetText == null)
        {
            resultBonusTargetText = CreateText(
                "ResultBonusTargetText",
                resultPanelTransform,
                "Bonus target: 00:30.00",
                28f,
                TextAlignmentOptions.Center,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -173f),
                new Vector2(0f, 44f)
            );
        }

        if (resultBonusText == null)
        {
            resultBonusText = CreateText(
                "ResultBonusText",
                resultPanelTransform,
                GetBonusRewardLabel() + ": not received",
                28f,
                TextAlignmentOptions.Center,
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -226f),
                new Vector2(0f, 44f)
            );
        }

        if (resultContinueText == null)
        {
            resultContinueText = CreateText(
                "ResultContinueText",
                resultPanelTransform,
                "Press E to continue",
                32f,
                TextAlignmentOptions.Center,
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 48f),
                new Vector2(0f, 56f)
            );
        }
    }

    private static GameObject CreateResultPanel(Transform parent)
    {
        GameObject panelObject = new GameObject("LevelResultPanel", typeof(RectTransform));
        panelObject.transform.SetParent(parent, false);

        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.72f);

        RectTransform rectTransform = panelImage.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(680f, 390f);

        return panelObject;
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
        GameObject textObject = new GameObject(objectName, typeof(RectTransform));
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

    private string GetBonusRewardLabel()
    {
        int amount = BonusHealthAmount;
        string unit = amount == 1 ? "heart" : "hearts";
        return "Bonus +" + amount + " " + unit;
    }
}
