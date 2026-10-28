
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTreeUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject treePanel;
    public TMP_Text selectedUpgradeDetails;
    public TMP_Text resourceBalancesText;
    public Button purchaseButton;
    public Button closeButton;
    public UpgradeNodeUI[] nodes;

    [Header("Player Control")]
    [Tooltip("Drag the player's movement script here.")]
    public Behaviour playerMovement;

    [Tooltip("Drag the player's Rigidbody2D here.")]
    public Rigidbody2D playerBody;

    [Header("Mouse Cursor")]
    [Tooltip("Unlock and show the cursor while the tree is open.")]
    public bool unlockCursorWhileOpen = true;

    [Header("Input")]
    public KeyCode closeKey = KeyCode.Escape;

    private UpgradeDefinition selectedUpgrade;
    private HubUpgradeBoard openingBoard;
    private bool isOpen;

    private bool movementWasEnabled;
    private bool movementStateSaved;

    private RigidbodyConstraints2D originalConstraints;
    private bool bodyStateSaved;

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisibility;
    private bool cursorStateSaved;

    private void Awake()
    {
        if (treePanel != null)
            treePanel.SetActive(false);
        else
            Debug.LogError(
                "UpgradeTreeUI: Tree Panel is not assigned.", this);

        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(PurchaseSelectedUpgrade);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseTree);
    }

    private void Start()
    {
        RefreshAll();
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(closeKey))
            CloseTree();
    }

    private void OnEnable()
    {
        if (GameProgression.Instance != null)
            GameProgression.Instance.OnProgressionChanged += RefreshAll;
    }

    private void OnDisable()
    {
        if (GameProgression.Instance != null)
            GameProgression.Instance.OnProgressionChanged -= RefreshAll;

        RestorePlayerControl();
        isOpen = false;
    }

    private void OnDestroy()
    {
        if (purchaseButton != null)
            purchaseButton.onClick.RemoveListener(PurchaseSelectedUpgrade);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseTree);
    }

    public bool OpenFromBoard(HubUpgradeBoard board)
    {
        if (isOpen)
            return true;

        if (GameProgression.Instance == null)
        {
            Debug.LogError(
                "UpgradeTreeUI: GameProgression.Instance is missing.");
            return false;
        }

        if (treePanel == null)
        {
            Debug.LogError(
                "UpgradeTreeUI: Tree Panel is not assigned.", this);
            return false;
        }

        openingBoard = board;
        isOpen = true;

        StopAndDisablePlayer();
        UnlockMouseCursor();

        treePanel.SetActive(true);

        RefreshAll();

        Debug.Log(
            "Upgrade tree opened. Panel active in hierarchy: " +
            treePanel.activeInHierarchy, this);

        return true;
    }

    private void StopAndDisablePlayer()
    {
        if (playerMovement != null)
        {
            movementWasEnabled = playerMovement.enabled;
            movementStateSaved = true;
            playerMovement.enabled = false;
        }
        else
        {
            Debug.LogWarning(
                "UpgradeTreeUI: Player Movement is not assigned.", this);
        }

        if (playerBody != null)
        {
            originalConstraints = playerBody.constraints;
            bodyStateSaved = true;

            // Stop current movement and freeze physics while the menu is open.
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
            playerBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            Debug.LogWarning(
                "UpgradeTreeUI: Player Body is not assigned. " +
                "Assign the player's Rigidbody2D to stop physics movement.",
                this);
        }
    }

    private void UnlockMouseCursor()
    {
        if (!unlockCursorWhileOpen)
            return;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;
        cursorStateSaved = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void RestorePlayerControl()
    {
        if (bodyStateSaved && playerBody != null)
        {
            playerBody.constraints = originalConstraints;

            // Do not resume the movement that was happening before the menu.
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
        }

        bodyStateSaved = false;

        if (movementStateSaved && playerMovement != null)
            playerMovement.enabled = movementWasEnabled;

        movementStateSaved = false;

        if (cursorStateSaved)
        {
            Cursor.lockState = previousCursorLockState;
            Cursor.visible = previousCursorVisibility;
        }

        cursorStateSaved = false;
    }

    public void CloseTree()
    {
        if (treePanel != null)
            treePanel.SetActive(false);

        isOpen = false;
        selectedUpgrade = null;

        RestorePlayerControl();

        if (selectedUpgradeDetails != null)
            selectedUpgradeDetails.text =
                "Select an upgrade to view its details.";

        if (purchaseButton != null)
            purchaseButton.interactable = false;

        if (openingBoard != null)
        {
            openingBoard.OnUpgradeTreeClosed();
            openingBoard = null;
        }
    }

    public void SelectUpgrade(UpgradeDefinition definition)
    {
        selectedUpgrade = definition;
        RefreshSelectedUpgrade();
    }

    public void PurchaseSelectedUpgrade()
    {
        if (!isOpen ||
            selectedUpgrade == null ||
            GameProgression.Instance == null)
            return;

        UpgradeDefinition definition = selectedUpgrade;
        GameProgression progress = GameProgression.Instance;

        int currentRank = progress.GetUpgradeRank(definition.upgradeID);

        if (currentRank >= definition.maxRank)
        {
            RefreshAll();
            return;
        }

        if (!HasPrerequisites(definition))
        {
            RefreshAll();
            return;
        }

        ResourceCost[] costs = GetNextRankCosts(definition);

        if (costs == null || !progress.TrySpend(costs))
        {
            RefreshAll();
            return;
        }

        progress.SetUpgradeRank(
            definition.upgradeID, currentRank + 1);

        RefreshAll();
    }

    private ResourceCost[] GetNextRankCosts(
        UpgradeDefinition definition)
    {
        int rank = GameProgression.Instance.GetUpgradeRank(
            definition.upgradeID);

        if (rank >= definition.maxRank ||
            definition.rankCosts == null ||
            rank >= definition.rankCosts.Length ||
            definition.rankCosts[rank] == null)
            return null;

        return definition.rankCosts[rank].costs;
    }

    private bool HasPrerequisites(UpgradeDefinition definition)
    {
        GameProgression progress = GameProgression.Instance;

        if (definition.requiredUpgradeIDs != null)
        {
            foreach (string id in definition.requiredUpgradeIDs)
            {
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                if (progress.GetUpgradeRank(id) < 1)
                    return false;
            }
        }

        if (definition.requiredRepairIDs != null)
        {
            foreach (string id in definition.requiredRepairIDs)
            {
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                if (progress.GetRepairLevel(id) < 1)
                    return false;
            }
        }

        return true;
    }

    private void RefreshAll()
    {
        Debug.Log("UpgradeTreeUI: RefreshAll called.", this);
        RefreshAllNodes();
        RefreshResourceBalances();
        RefreshSelectedUpgrade();
    }

    public void RefreshAllNodes()
    {
        if (nodes == null)
            return;

        foreach (UpgradeNodeUI node in nodes)
        {
            if (node != null)
                node.Refresh();
        }
    }

    private void RefreshResourceBalances()
    {
        if (resourceBalancesText == null ||
            GameProgression.Instance == null)
            return;

        StringBuilder text = new StringBuilder();

        foreach (ResourceType type in
                 (ResourceType[])System.Enum.GetValues(
                     typeof(ResourceType)))
        {
            text.AppendLine(
                type + ": " +
                GameProgression.Instance.GetResource(type));
        }

        resourceBalancesText.text = text.ToString();
    }

    private void RefreshSelectedUpgrade()
    {
        if (selectedUpgradeDetails == null)
            return;

        if (selectedUpgrade == null ||
            GameProgression.Instance == null)
        {
            selectedUpgradeDetails.text =
                "Select an upgrade to view its details.";

            if (purchaseButton != null)
                purchaseButton.interactable = false;

            return;
        }

        GameProgression progress = GameProgression.Instance;
        UpgradeDefinition definition = selectedUpgrade;

        int rank = progress.GetUpgradeRank(definition.upgradeID);
        bool maxed = rank >= definition.maxRank;
        bool prerequisitesMet = HasPrerequisites(definition);

        ResourceCost[] costs = maxed
            ? null
            : GetNextRankCosts(definition);

        StringBuilder text = new StringBuilder();

        text.AppendLine(definition.displayName);
        text.AppendLine("Rank: " + rank + " / " + definition.maxRank);
        text.AppendLine();
        text.AppendLine(definition.description);
        text.AppendLine();

        if (maxed)
        {
            text.AppendLine("MAXIMUM RANK REACHED");
        }
        else
        {
            text.AppendLine("Next Rank Cost:");

            if (costs == null)
            {
                text.AppendLine("Upgrade costs are not configured.");
            }
            else
            {
                foreach (ResourceCost cost in costs)
                {
                    if (cost == null)
                        continue;

                    int owned = progress.GetResource(cost.resource);
                    bool affordable = owned >= cost.amount;

                    text.AppendLine(
                        cost.resource + ": " + cost.amount +
                        " (You have " + owned + ")" +
                        (affordable ? " [OK]" : " [NOT ENOUGH]"));
                }
            }

            text.AppendLine();
            text.AppendLine(prerequisitesMet
                ? "Prerequisites: Met"
                : "Prerequisites: Not met");
        }

        selectedUpgradeDetails.text = text.ToString();

        bool canAfford =
            costs != null && progress.HasResources(costs);

        if (purchaseButton != null)
        {
            purchaseButton.interactable =
                isOpen &&
                !maxed &&
                prerequisitesMet &&
                canAfford;
        }
    }
}
