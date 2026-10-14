
using System;
using UnityEngine;

[Serializable]
public class RepairLevelCost
{
    public ResourceCost[] costs;
}

[CreateAssetMenu(
    fileName = "NewRepair",
    menuName = "Crypt Cleaners/Progression/Repair")]
public class RepairDefinition : ScriptableObject
{
    [Header("Identity")]
    public string repairID = "repair_new";
    public string displayName = "Repair Object";

    [TextArea(3, 6)]
    public string description = "Describe what this repair restores.";

    [Header("Levels")]
    [Min(1)] public int maxLevel = 3;

    [Tooltip("Element 0 is the cost to reach level 1, element 1 reaches level 2, etc.")]
    public RepairLevelCost[] levelCosts;

    [Header("Optional Unlock Information")]
    [TextArea(2, 4)]
    public string unlockDescription =
        "Describe the upgrade paths this repair unlocks.";
}
