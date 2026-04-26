using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text heartsText;

    private void Start()
    {
        UpdateHearts();
    }

    private void Update()
    {
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        if (playerHealth == null || heartsText == null)
        {
            return;
        }

        heartsText.text = "Hearts: " + playerHealth.CurrentHealth + "/" + playerHealth.MaxHealth;
    }
}
