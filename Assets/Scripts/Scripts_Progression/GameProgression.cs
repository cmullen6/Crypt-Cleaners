using System;
using System.Collections.Generic;
using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public static GameProgression Instance { get; private set; }

    [Header("Starting Banked Resources")]
    [Min(0)] public int startingGold;
    [Min(0)] public int startingResourceA;
    [Min(0)] public int startingResourceB;
    [Min(0)] public int startingResourceC;
    [Min(0)] public int startingResourceD;

    [Header("Testing")]
    public bool loadSavedData = true;

    [Serializable]
    private class SavedEntry
    {
        public string id;
        public int value;
    }

    [Serializable]
    private class SaveData
    {
        public List<SavedEntry> resources = new List<SavedEntry>();
        public List<SavedEntry> upgrades = new List<SavedEntry>();
        public List<SavedEntry> repairs = new List<SavedEntry>();
    }

    private readonly Dictionary<ResourceType, int> resources =
        new Dictionary<ResourceType, int>();

    private readonly Dictionary<string, int> upgrades =
        new Dictionary<string, int>();

    private readonly Dictionary<string, int> repairs =
        new Dictionary<string, int>();

    private const string SaveKey = "CryptCleaners_Progression";
    private bool initialized;

    public event Action OnProgressionChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeResources();

        if (loadSavedData)
        {
            LoadProgress();
        }

        initialized = true;
        OnProgressionChanged?.Invoke();
    }

    private void InitializeResources()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
        }

        resources[ResourceType.Gold] = startingGold;
        resources[ResourceType.ResourceA] = startingResourceA;
        resources[ResourceType.ResourceB] = startingResourceB;
        resources[ResourceType.ResourceC] = startingResourceC;
        resources[ResourceType.ResourceD] = startingResourceD;
    }

    public int GetResource(ResourceType type)
    {
        return resources.TryGetValue(type, out int amount) ? amount : 0;
    }

    public int GetUpgradeRank(string id)
    {
        return upgrades.TryGetValue(id, out int rank) ? rank : 0;
    }

    public int GetRepairLevel(string id)
    {
        return repairs.TryGetValue(id, out int level) ? level : 0;
    }

    public bool HasResources(ResourceCost[] costs)
    {
        if (costs == null)
            return true;

        foreach (ResourceCost cost in costs)
        {
            if (cost == null || cost.amount <= 0)
                continue;

            if (GetResource(cost.resource) < cost.amount)
                return false;
        }

        return true;
    }

    public bool TrySpend(ResourceCost[] costs)
    {
        if (!HasResources(costs))
            return false;

        if (costs != null)
        {
            foreach (ResourceCost cost in costs)
            {
                if (cost == null || cost.amount <= 0)
                    continue;

                resources[cost.resource] -= cost.amount;
            }
        }

        NotifyChanged();
        return true;
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        resources[type] = GetResource(type) + amount;
        NotifyChanged();
    }

    public void SetUpgradeRank(string id, int rank)
    {
        if (string.IsNullOrWhiteSpace(id))
            return;

        upgrades[id] = Mathf.Max(0, rank);
        NotifyChanged();
    }

    public void SetRepairLevel(string id, int level)
    {
        if (string.IsNullOrWhiteSpace(id))
            return;

        repairs[id] = Mathf.Max(0, level);
        NotifyChanged();
    }

    public void NotifyChanged()
    {
        OnProgressionChanged?.Invoke();

        if (initialized)
            SaveProgress();
    }

    public void SaveProgress()
    {
        SaveData data = new SaveData();

        foreach (var pair in resources)
        {
            data.resources.Add(new SavedEntry
            {
                id = pair.Key.ToString(),
                value = pair.Value
            });
        }

        foreach (var pair in upgrades)
        {
            data.upgrades.Add(new SavedEntry
            {
                id = pair.Key,
                value = pair.Value
            });
        }

        foreach (var pair in repairs)
        {
            data.repairs.Add(new SavedEntry
            {
                id = pair.Key,
                value = pair.Value
            });
        }

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return;

        SaveData data = JsonUtility.FromJson<SaveData>(
            PlayerPrefs.GetString(SaveKey));

        if (data == null)
            return;

        if (data.resources != null)
        {
            foreach (SavedEntry entry in data.resources)
            {
                if (Enum.TryParse(entry.id, out ResourceType type))
                    resources[type] = Mathf.Max(0, entry.value);
            }
        }

        if (data.upgrades != null)
        {
            foreach (SavedEntry entry in data.upgrades)
                upgrades[entry.id] = Mathf.Max(0, entry.value);
        }

        if (data.repairs != null)
        {
            foreach (SavedEntry entry in data.repairs)
                repairs[entry.id] = Mathf.Max(0, entry.value);
        }
    }

    [ContextMenu("TEST ONLY: Add 1000 of Every Resource")]
    private void AddTestResources()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            resources[type] = GetResource(type) + 1000;

        NotifyChanged();
    }

    public void ResetProgressForTesting()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();

        upgrades.Clear();
        repairs.Clear();
        InitializeResources();
        NotifyChanged();
    }
}
