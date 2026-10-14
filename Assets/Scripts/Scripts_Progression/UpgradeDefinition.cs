
using System;
using UnityEngine;

[Serializable]
public class UpgradeRankCost
{
    public ResourceCost[] costs;
}

[CreateAssetMenu(
    fileName = "NewUpgrade",
    menuName = "Crypt Cleaners/Progression/Upgrade")]
public class UpgradeDefinition : ScriptableObject
{
    [Header("Identity")]
    public string upgradeID = "upgrade_new";
    public string displayName = "New Upgrade";

    [TextArea(3, 8)]
    public string description = "Describe what this upgrade does.";

    [Header("Ranks")]
    [Min(1)] public int maxRank = 1;

    [Tooltip("Element 0 is the cost to buy rank 1, element 1 buys rank 2, etc.")]
    public UpgradeRankCost[] rankCosts;

    [Header("Prerequisites")]
    public string[] requiredUpgradeIDs;
    public string[] requiredRepairIDs;

    [Header("Example Gameplay Effect")]
    public float valuePerRank = 1f;
}
