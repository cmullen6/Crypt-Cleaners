using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrototypeUI : MonoBehaviour
{
    [Header("Cleaning")]
    [SerializeField] private Slider cleaningSlider;
    [SerializeField] private TMP_Text cleaningText;

    [Header("Health")]
    [SerializeField] private TMP_Text healthText;

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (cleaningSlider != null)
        {
            cleaningSlider.minValue = 0f;
            cleaningSlider.maxValue = 1f;
        }
    }

    private void Update()
    {
        UpdateCleaningUI();
        UpdateHealthUI();
    }

    private void UpdateCleaningUI()
    {
        if (CleaningManager.Instance == null)
            return;

        float percentage =
            CleaningManager.Instance.CleaningPercentage;

        if (cleaningSlider != null)
            cleaningSlider.value = percentage;

        if (cleaningText != null)
        {
            int percent = Mathf.RoundToInt(percentage * 100f);

            cleaningText.text =
                $"CLEAN: {percent}%";
        }
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        if (healthText != null)
        {
            healthText.text =
                $"LIVES: {playerHealth.CurrentHealth}";
        }
    }
}
