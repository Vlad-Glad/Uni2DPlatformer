using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public static HealthUI Instance { get; private set; }

    [Header("Heart UI")]
    [SerializeField] private Transform heartContainer;
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private int maxVisibleHearts = 5;
    [SerializeField] private float heartSpacing = 54f;

    [Header("Text UI")]
    [SerializeField] private TMP_Text healthText;

    private readonly List<Image> hearts = new List<Image>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        int visibleHeartCount = Mathf.Clamp(maxHealth, 0, maxVisibleHearts);

        EnsureHeartCount(visibleHeartCount);
        UpdateHeartPositions();
        UpdateHeartIcons(currentHealth);
        UpdateHealthText(currentHealth);
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    private void EnsureHeartCount(int requiredCount)
    {
        if (heartContainer == null || heartPrefab == null)
        {
            return;
        }

        while (hearts.Count < requiredCount)
        {
            Image newHeart = Instantiate(heartPrefab, heartContainer);
            newHeart.gameObject.name = "HeartIcon_" + (hearts.Count + 1);
            newHeart.gameObject.SetActive(true);
            hearts.Add(newHeart);
        }

        while (hearts.Count > requiredCount)
        {
            Image lastHeart = hearts[hearts.Count - 1];
            hearts.RemoveAt(hearts.Count - 1);
            Destroy(lastHeart.gameObject);
        }
    }

    private void UpdateHeartPositions()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            RectTransform heartRect = hearts[i].rectTransform;
            heartRect.anchorMin = new Vector2(0f, 1f);
            heartRect.anchorMax = new Vector2(0f, 1f);
            heartRect.pivot = new Vector2(0f, 1f);
            heartRect.anchoredPosition = new Vector2(i * heartSpacing, 0f);
        }
    }

    private void UpdateHeartIcons(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            bool isFull = i < currentHealth;
            hearts[i].gameObject.SetActive(isFull);
            hearts[i].sprite = fullHeartSprite;
            hearts[i].color = Color.white;
        }
    }

    private void UpdateHealthText(int currentHealth)
    {
        if (healthText == null)
        {
            return;
        }

        healthText.text = "Healths: " + currentHealth;
    }
}
