
using System.Text;
using TMPro;
using UnityEngine;

public class UpgradeTreeUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject treePanel;
    public TMP_Text selectedUpgradeDetails;
    public UpgradeNodeUI[] nodes;

    [Header("World Rules")]
    [Tooltip("Enable only when this UI is used in the hub scene.")]
    public bool purchasesAllowedInThisScene;

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.Q;

    private UpgradeDefinition selectedUpgrade;
    private bool visible;

    private void Start()
    {
        if (treePanel != null)
            treePanel.SetActive(false);

        visible = false;
        RefreshAllNodes();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleTree();
    }

    public void ToggleTree()
    {
        SetTreeVisible(!visible);
    }

    public void SetTreeVisible(bool show)
    {
        visible = show;

        if (treePanel != null)
            treePanel.SetActive(visible);

        if (visible)
        {
            RefreshAllNodes();

            if (selectedUpgrade != null)
                SelectUpgrade(selectedUpgrade);
        }
    }

    public void SelectUpgrade(UpgradeDefinition definition)
    {
        selectedUpgrade = definition;

        if (selectedUpgradeDetails == null || definition == null)
            return;

        int rank = GameProgression.Instance.GetUpgradeRank(
            definition.upgradeID);

        StringBuilder text = new StringBuilder();
        text.AppendLine(definition.displayName);
        text.AppendLine("Rank: " + rank + " / " + definition.maxRank);
        text.AppendLine();
        text.AppendLine(definition.description);
        text.AppendLine();

        if (rank >= definition.maxRank)
        {
            text.AppendLine("MAXIMUM RANK REACHED");
        }
        else
        {
            ResourceCost[] costs = GetNextRankCosts(definition);

            text.AppendLine("Next Rank Cost:");

            if (costs != null)
            {
                foreach (ResourceCost cost in costs)
                {
                    if (cost == null)
                        continue;

                    text.AppendLine(
                        cost.resource + ": " + cost.amount +
                        " (You have " +
                        GameProgression.Instance.GetResource(cost.resource) +
                        ")");
                }
            }

            bool prerequisitesMet = HasPrerequisites(definition);
            text.AppendLine();
            text.AppendLine(prerequisitesMet
                ? "Prerequisites: Met"
                : "Prerequisites: Not met");

            text.AppendLine(purchasesAllowedInThisScene
                ? "Purchases available in this hub."
                : "Viewing only: return to the hub to purchase.");
        }

        selectedUpgradeDetails.text = text.ToString();
    }

    public bool TryPurchase(UpgradeDefinition definition)
    {
        if (definition == null || GameProgression.Instance == null)
            return false;

        if (!purchasesAllowedInThisScene)
        {
            SelectUpgrade(definition);
            return false;
        }

        int rank = GameProgression.Instance.GetUpgradeRank(
            definition.upgradeID);

        if (rank >= definition.maxRank)
            return false;

        if (!HasPrerequisites(definition))
        {
            SelectUpgrade(definition);
            return false;
        }

        ResourceCost[] costs = GetNextRankCosts(definition);

        if (costs == null ||
            !GameProgression.Instance.TrySpend(costs))
        {
            SelectUpgrade(definition);
            return false;
        }

        GameProgression.Instance.SetUpgradeRank(
            definition.upgradeID, rank + 1);

        RefreshAllNodes();
        SelectUpgrade(definition);
        return true;
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
        {
            return null;
        }

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

                if (progress.GetUpgradeRank(id) <= 0)
                    return false;
            }
        }

        if (definition.requiredRepairIDs != null)
        {
            foreach (string id in definition.requiredRepairIDs)
            {
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                if (progress.GetRepairLevel(id) <= 0)
                    return false;
            }
        }

        return true;
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

        if (selectedUpgrade != null && visible)
            SelectUpgrade(selectedUpgrade);
    }
}
