
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrototypeUI : MonoBehaviour
{
    [Header("Cleaning")]
    [SerializeField] private Slider cleaningSlider;
    [SerializeField] private TMP_Text cleaningText;

    [Header("Health Icons")]
    [SerializeField] private Transform healthContainer;
    [SerializeField] private Image healthIconPrefab;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    private PlayerHealth playerHealth;

    private Image[] healthIcons;

    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (cleaningSlider != null)
        {
            cleaningSlider.minValue = 0f;
            cleaningSlider.maxValue = 1f;
        }

        SetupHealthIcons();
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

            cleaningText.text = $"CLEAN: {percent}%";
        }
    }

    private void SetupHealthIcons()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("PrototypeUI: PlayerHealth was not found.");
            return;
        }

        if (healthContainer == null || healthIconPrefab == null)
        {
            Debug.LogWarning("PrototypeUI: Health container or icon prefab is missing.");
            return;
        }

        // Create one icon for every maximum health point.
        healthIcons = new Image[playerHealth.MaxHealth];

        for (int i = 0; i < healthIcons.Length; i++)
        {
            Image icon = Instantiate(
                healthIconPrefab,
                healthContainer
            );

            icon.sprite = fullHeartSprite;
            icon.gameObject.SetActive(true);

            healthIcons[i] = icon;
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null || healthIcons == null)
            return;

        int currentHealth = playerHealth.CurrentHealth;

        for (int i = 0; i < healthIcons.Length; i++)
        {
            if (healthIcons[i] == null)
                continue;

            if (i < currentHealth)
            {
                // This health point is still available.
                healthIcons[i].sprite = fullHeartSprite;
            }
            else
            {
                // This health point has been lost.
                healthIcons[i].sprite = emptyHeartSprite;
            }
        }
    }
}
