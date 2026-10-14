
using System;
using UnityEngine;

public enum ResourceType
{
    Gold,
    ResourceA,
    ResourceB,
    ResourceC,
    ResourceD
}

[Serializable]
public class ResourceCost
{
    public ResourceType resource;

    [Min(0)]
    public int amount;
}
