
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [Header("Next Room")]
    [SerializeField] private string nextSceneName;

    [Header("Reward Preview")]
    [SerializeField] private TMP_Text rewardPreviewText;

    [SerializeField] private ResourceType rewardResource;
    [SerializeField, Min(1)] private int rewardAmount = 1;

    [Tooltip("Roll a new reward when this door initializes.")]
    [SerializeField] private bool randomizeReward = true;

    [Header("Door Visuals (Optional)")]
    [SerializeField] private GameObject lockedVisual;
    [SerializeField] private GameObject unlockedVisual;

    private bool hasExited;

    private void Start()
    {
        if (randomizeReward)
        {
            CleaningManager.RollRandomReward(
                out rewardResource, out rewardAmount);
        }

        UpdateRewardPreview();
        UpdateDoorVisuals();
    }

    private void Update()
    {
        UpdateDoorVisuals();
    }

    private void UpdateRewardPreview()
    {
        if (rewardPreviewText != null)
        {
            rewardPreviewText.text =
                rewardResource + " x" + rewardAmount;
        }
    }

    private void UpdateDoorVisuals()
    {
        bool unlocked =
            CleaningManager.Instance != null &&
            CleaningManager.Instance.RoomComplete;

        if (lockedVisual != null)
            lockedVisual.SetActive(!unlocked);

        if (unlockedVisual != null)
            unlockedVisual.SetActive(unlocked);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExited || !other.CompareTag("Player"))
            return;

        if (CleaningManager.Instance == null)
        {
            Debug.LogError("ExitDoor: CleaningManager is missing.", this);
            return;
        }

        if (!CleaningManager.Instance.RoomComplete)
        {
            Debug.Log("Clean the entire room before leaving!");
            return;
        }

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogWarning(
                "ExitDoor: Next Scene Name is not assigned.", this);
            return;
        }

        // The next room will award the resource shown on this door.
        CleaningManager.PrepareNextRoomReward(
            rewardResource, rewardAmount);

        hasExited = true;

        SceneManager.LoadScene(nextSceneName);
    }
}

