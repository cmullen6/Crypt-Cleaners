
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNodeUI : MonoBehaviour
{
    [Header("Upgrade")]
    public UpgradeDefinition definition;

    [Header("UI References")]
    public Button button;
    public TMP_Text nodeLabel;
    public UpgradeTreeUI tree;

    private void Start()
    {
        if (button != null)
            button.onClick.AddListener(OnClicked);

        Refresh();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClicked);
    }

    private void OnClicked()
    {
        if (tree == null || definition == null)
            return;

        tree.SelectUpgrade(definition);
        tree.TryPurchase(definition);
    }

    public void Refresh()
    {
        if (definition == null || GameProgression.Instance == null)
            return;

        int rank = GameProgression.Instance.GetUpgradeRank(
            definition.upgradeID);

        if (nodeLabel != null)
        {
            nodeLabel.text = definition.displayName +
                "\n" + rank + " / " + definition.maxRank;
        }

        if (button != null)
        {
            button.interactable = rank < definition.maxRank;
        }
    }
}
