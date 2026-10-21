
using UnityEngine;

public class HubUpgradeBoard : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Tooltip("Assign the GameObject containing UpgradeTreeUI.")]
    public UpgradeTreeUI upgradeTree;

    [Header("Visual Feedback")]
    public SpriteRenderer outlineSprite;
    public GameObject interactionPrompt;

    private bool playerNearby;
    private bool interactionOpen;

    private void Start()
    {
        SetVisuals(false);

        if (upgradeTree == null)
        {
            Debug.LogError(
                "Upgrade Board: UpgradeTreeUI is not assigned.", this);
        }
    }

    private void Update()
    {
        if (!playerNearby || interactionOpen)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            if (upgradeTree == null)
            {
                Debug.LogError(
                    "Upgrade Board: Cannot open tree. " +
                    "Assign the UpgradeTreeUI reference.", this);
                return;
            }

            if (upgradeTree.OpenFromBoard(this))
            {
                interactionOpen = true;
                SetVisuals(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerNearby = true;

        if (!interactionOpen)
            SetVisuals(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerNearby = false;
        SetVisuals(false);
    }

    private void SetVisuals(bool visible)
    {
        if (outlineSprite != null)
            outlineSprite.enabled = visible;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(visible);
    }

    public void OnUpgradeTreeClosed()
    {
        interactionOpen = false;

        if (playerNearby)
            SetVisuals(true);
    }
}
