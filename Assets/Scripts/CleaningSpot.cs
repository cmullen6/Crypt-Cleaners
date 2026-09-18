using UnityEngine;
using UnityEngine.UI;

public class CleaningSpot : MonoBehaviour
{
    [Header("Cleaning")]
    [SerializeField] private float cleaningRequired = 100f;

    [Header("Progress Bar")]
    [SerializeField] private Slider progressBar;

    private float cleaningProgress;
    private bool isCleaned;

    public float Progress01
    {
        get
        {
            if (cleaningRequired <= 0f)
                return 1f;

            return Mathf.Clamp01(cleaningProgress / cleaningRequired);
        }
    }

    private void Start()
    {
        cleaningProgress = 0f;
        isCleaned = false;

        UpdateProgressBar();

        if (CleaningManager.Instance != null)
        {
            CleaningManager.Instance.RegisterCleaningSpot(this);
        }
    }

    public void Clean(float amount)
    {
        if (isCleaned)
            return;

        cleaningProgress += amount;
        cleaningProgress = Mathf.Clamp(cleaningProgress, 0f, cleaningRequired);

        UpdateProgressBar();

        if (cleaningProgress >= cleaningRequired)
        {
            CompleteCleaning();
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            progressBar.value = Progress01;
        }
    }

    private void CompleteCleaning()
    {
        if (isCleaned)
            return;

        isCleaned = true;

        if (CleaningManager.Instance != null)
        {
            CleaningManager.Instance.SpotCleaned(this);
        }

        gameObject.SetActive(false);
    }
}
