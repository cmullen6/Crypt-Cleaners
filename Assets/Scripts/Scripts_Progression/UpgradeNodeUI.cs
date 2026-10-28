
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNodeUI : MonoBehaviour
{
    [Header("Upgrade Definition")]
    public UpgradeDefinition definition;

    [Header("UI References")]
    public Button button;
    public TMP_Text nodeLabel;
    public UpgradeTreeUI tree;

    private void OnEnable()
    {
        if (GameProgression.Instance != null)
            GameProgression.Instance.OnProgressionChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (GameProgression.Instance != null)
            GameProgression.Instance.OnProgressionChanged -= Refresh;
    }

    // This public method can be assigned directly in the Button Inspector.
    public void SelectThisUpgrade()
    {
        Debug.Log("UpgradeNodeUI: SelectThisUpgrade called.", this);

        if (tree == null)
        {
            Debug.LogError("UpgradeNodeUI: Tree reference is missing.", this);
            return;
        }

        if (definition == null)
        {
            Debug.LogError("UpgradeNodeUI: Upgrade Definition is missing.", this);
            return;
        }

        tree.SelectUpgrade(definition);
    }

    public void Refresh()
    {
        if (definition == null ||
            GameProgression.Instance == null)
            return;

        int rank = GameProgression.Instance.GetUpgradeRank(
            definition.upgradeID);

        if (nodeLabel != null)
        {
            nodeLabel.text =
                definition.displayName + "\n" +
                rank + " / " + definition.maxRank;
        }

        if (button != null)
        {
            button.interactable = rank < definition.maxRank;
        }
    }
}
