
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
        if (button != null)
            button.onClick.AddListener(OnClicked);

        if (GameProgression.Instance != null)
            GameProgression.Instance.OnProgressionChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClicked);

        if (GameProgression.Instance != null)
            GameProgression.Instance.OnProgressionChanged -= Refresh;
    }

    private void OnClicked()
    {
        if (tree == null || definition == null)
            return;

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
