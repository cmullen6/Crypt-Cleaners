
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairPanelUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Text")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text levelText;
    public TMP_Text costText;
    public TMP_Text unlockText;

    [Header("Buttons")]
    public Button repairButton;
    public Button closeButton;

    private HubRepairObject currentObject;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        if (repairButton != null)
            repairButton.onClick.AddListener(RepairCurrentObject);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    private void OnDestroy()
    {
        if (repairButton != null)
            repairButton.onClick.RemoveListener(RepairCurrentObject);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }

    public void Open(HubRepairObject repairObject)
    {
        currentObject = repairObject;

        if (panel != null)
            panel.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        if (currentObject != null)
            currentObject.CloseInteraction();

        currentObject = null;
    }

    public void Refresh()
    {
        if (currentObject == null || GameProgression.Instance == null)
            return;

        RepairDefinition definition = currentObject.GetDefinition();

        if (definition == null)
            return;

        int level = currentObject.GetCurrentLevel();

        if (titleText != null)
            titleText.text = definition.displayName;

        if (descriptionText != null)
            descriptionText.text = definition.description;

        if (levelText != null)
        {
            levelText.text = "Level " + level +
                " / " + definition.maxLevel;
        }

        bool isMaxLevel = level >= definition.maxLevel;
        bool hasCost = !isMaxLevel &&
            definition.levelCosts != null &&
            level < definition.levelCosts.Length &&
            definition.levelCosts[level] != null;

        if (costText != null)
        {
            if (isMaxLevel)
            {
                costText.text = "Fully repaired!";
            }
            else if (!hasCost)
            {
                costText.text = "Repair cost not configured.";
            }
            else
            {
                costText.text = BuildCostText(
                    definition.levelCosts[level].costs);
            }
        }

        if (unlockText != null)
            unlockText.text = definition.unlockDescription;

        if (repairButton != null)
            repairButton.interactable = !isMaxLevel && hasCost;
    }

    private string BuildCostText(ResourceCost[] costs)
    {
        StringBuilder text = new StringBuilder();

        text.AppendLine("Required Resources:");

        if (costs != null)
        {
            foreach (ResourceCost cost in costs)
            {
                if (cost == null)
                    continue;

                int available = GameProgression.Instance.GetResource(
                    cost.resource);

                text.AppendLine(
                    cost.resource + ": " + cost.amount +
                    " (You have " + available + ")");
            }
        }

        return text.ToString();
    }

    private void RepairCurrentObject()
    {
        if (currentObject == null)
            return;

        if (currentObject.TryRepair())
            Refresh();
        else
            Refresh();
    }
}
