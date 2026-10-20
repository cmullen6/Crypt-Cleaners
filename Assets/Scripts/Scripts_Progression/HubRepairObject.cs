
using TMPro;
using UnityEngine;

public class HubRepairObject : MonoBehaviour
{
    [Header("Repair Definition")]
    public RepairDefinition definition;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";
    public RepairPanelUI repairPanel;

    [Header("World Display")]
    public TMP_Text worldLabel;
    public GameObject interactionHint;

    [Header("Outline")]
    [Tooltip("Assign a child SpriteRenderer showing the white silhouette.")]
    public SpriteRenderer outlineSprite;

    private bool playerNearby;
    private bool panelOpen;

    private void Start()
    {
        SetOutline(false);

        if (interactionHint != null)
            interactionHint.SetActive(false);

        RefreshWorldDisplay();
    }

    private void Update()
    {
        if (!playerNearby || panelOpen)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            if (repairPanel == null || definition == null)
                return;

            panelOpen = true;
            repairPanel.Open(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerNearby = true;
        SetOutline(true);

        if (interactionHint != null)
            interactionHint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerNearby = false;
        SetOutline(false);

        if (interactionHint != null)
            interactionHint.SetActive(false);
    }

    private void SetOutline(bool enabledState)
    {
        if (outlineSprite != null)
            outlineSprite.enabled = enabledState;
    }

    public void RefreshWorldDisplay()
    {
        if (definition == null || worldLabel == null)
            return;

        int level = GameProgression.Instance != null
            ? GameProgression.Instance.GetRepairLevel(definition.repairID)
            : 0;

        worldLabel.text = definition.displayName +
            " " + level + "/" + definition.maxLevel;
    }

    public bool TryRepair()
    {
        if (definition == null || GameProgression.Instance == null)
            return false;

        int level = GameProgression.Instance.GetRepairLevel(
            definition.repairID);

        if (level >= definition.maxLevel)
            return false;

        if (definition.levelCosts == null ||
            level >= definition.levelCosts.Length ||
            definition.levelCosts[level] == null)
            return false;

        ResourceCost[] costs = definition.levelCosts[level].costs;

        if (!GameProgression.Instance.TrySpend(costs))
            return false;

        GameProgression.Instance.SetRepairLevel(
            definition.repairID, level + 1);

        RefreshWorldDisplay();
        return true;
    }

    public void CloseInteraction()
    {
        panelOpen = false;
        RefreshWorldDisplay();
    }

    public RepairDefinition GetDefinition()
    {
        return definition;
    }

    public int GetCurrentLevel()
    {
        if (definition == null || GameProgression.Instance == null)
            return 0;

        return GameProgression.Instance.GetRepairLevel(
            definition.repairID);
    }
}
