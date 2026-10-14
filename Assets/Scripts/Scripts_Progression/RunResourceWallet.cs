
using System;
using System.Collections.Generic;
using UnityEngine;

public class RunResourceWallet : MonoBehaviour
{
    private readonly Dictionary<ResourceType, int> runResources =
        new Dictionary<ResourceType, int>();

    public event Action OnRunResourcesChanged;

    private void Awake()
    {
        ClearRunResources();
    }

    public int GetResource(ResourceType type)
    {
        return runResources.TryGetValue(type, out int amount)
            ? amount : 0;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        runResources[type] = GetResource(type) + amount;
        OnRunResourcesChanged?.Invoke();
    }

    public void BankRunResources()
    {
        if (GameProgression.Instance == null)
            return;

        foreach (ResourceType type in
                 (ResourceType[])Enum.GetValues(typeof(ResourceType)))
        {
            int amount = GetResource(type);

            if (amount > 0)
                GameProgression.Instance.AddResource(type, amount);
        }

        ClearRunResources();
    }

    public void ClearRunResources()
    {
        foreach (ResourceType type in
                 (ResourceType[])Enum.GetValues(typeof(ResourceType)))
        {
            runResources[type] = 0;
        }

        OnRunResourcesChanged?.Invoke();
    }
}

