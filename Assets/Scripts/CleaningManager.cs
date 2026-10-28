
using System.Collections.Generic;
using UnityEngine;

public class CleaningManager : MonoBehaviour
{
    public static CleaningManager Instance { get; private set; }

    // The next room receives its reward selection from the door entered.
    private static bool hasPendingReward;
    private static ResourceType pendingResource;
    private static int pendingAmount;

    [Header("Room Requirement")]
    [SerializeField, Range(0f, 1f)]
    private float requiredCleanPercentage = 1f;

    [Header("Room Reward")]
    [SerializeField] private GameObject rewardPrefab;

    [SerializeField]
    private ResourceType[] possibleRewards =
    {
        ResourceType.Gold,
        ResourceType.ResourceA,
        ResourceType.ResourceB,
        ResourceType.ResourceC,
        ResourceType.ResourceD
    };

    [SerializeField, Min(1)] private int minimumRewardAmount = 1;
    [SerializeField, Min(1)] private int maximumRewardAmount = 5;

    [SerializeField]
    private ResourcePickup.RewardDestination rewardDestination =
        ResourcePickup.RewardDestination.CurrentRun;

    [SerializeField]
    private Vector2 rewardSpawnOffset =
        new Vector2(1f, 0.25f);

    [SerializeField] private string playerTag = "Player";

    private readonly List<CleaningSpot> cleaningSpots = new();
    private readonly HashSet<CleaningSpot> cleanedSpots = new();

    private int totalSpots;
    private int cleanedSpotsCount;
    private int initialTileCount;

    private bool roomComplete;
    private bool rewardSpawned;

    private ResourceType roomRewardResource;
    private int roomRewardAmount;

    public float CleaningPercentage =>
        totalSpots <= 0
            ? 0f
            : Mathf.Clamp01((float)cleanedSpotsCount / totalSpots);

    public bool RoomComplete => roomComplete;
    public ResourceType RoomRewardResource => roomRewardResource;
    public int RoomRewardAmount => roomRewardAmount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SelectRoomReward();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void SelectRoomReward()
    {
        if (hasPendingReward)
        {
            roomRewardResource = pendingResource;
            roomRewardAmount = pendingAmount;
            hasPendingReward = false;
            return;
        }

        RollRandomReward(out roomRewardResource, out roomRewardAmount);
    }

    public static void PrepareNextRoomReward(
        ResourceType resource, int amount)
    {
        pendingResource = resource;
        pendingAmount = Mathf.Max(1, amount);
        hasPendingReward = true;
    }

    public static void RollRandomReward(
        out ResourceType resource, out int amount)
    {
        ResourceType[] available =
        {
            ResourceType.Gold,
            ResourceType.ResourceA,
            ResourceType.ResourceB,
            ResourceType.ResourceC,
            ResourceType.ResourceD
        };

        resource = available[Random.Range(0, available.Length)];
        amount = Random.Range(1, 6);
    }

    public void RegisterCleaningSpot(CleaningSpot spot)
    {
        if (spot == null || cleaningSpots.Contains(spot))
            return;

        cleaningSpots.Add(spot);
        totalSpots++;

        CheckRoomCompletion();
    }

    public void RegisterTileCount(int count)
    {
        if (count <= 0)
            return;

        initialTileCount += count;
        totalSpots += count;

        CheckRoomCompletion();
    }

    public void SpotCleaned(CleaningSpot spot)
    {
        if (spot == null || !cleaningSpots.Contains(spot))
            return;

        if (!cleanedSpots.Add(spot))
            return;

        cleanedSpotsCount++;
        CheckRoomCompletion();
    }

    public void TileCleaned()
    {
        int cleanedTiles = cleanedSpotsCount - cleanedSpots.Count;

        if (cleanedTiles >= initialTileCount)
            return;

        cleanedSpotsCount++;
        CheckRoomCompletion();
    }

    private void CheckRoomCompletion()
    {
        if (roomComplete || totalSpots <= 0)
            return;

        if (CleaningPercentage < requiredCleanPercentage)
            return;

        roomComplete = true;

        Debug.Log(
            "Room complete! Reward: " +
            roomRewardResource + " x" + roomRewardAmount);

        SpawnRoomReward();
    }

    private void SpawnRoomReward()
    {
        if (rewardSpawned)
            return;

        if (rewardPrefab == null)
        {
            Debug.LogError(
                "CleaningManager: Assign a Reward Prefab.", this);
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
        {
            Debug.LogError(
                "CleaningManager: Player not found.", this);
            return;
        }

        Vector3 position = player.transform.position +
            new Vector3(rewardSpawnOffset.x, rewardSpawnOffset.y, 0f);

        GameObject reward = Instantiate(
            rewardPrefab, position, Quaternion.identity);

        ResourcePickup pickup = reward.GetComponent<ResourcePickup>();

        if (pickup == null)
        {
            Debug.LogError(
                "Reward Prefab needs a ResourcePickup component.", reward);
            Destroy(reward);
            return;
        }

        pickup.resourceType = roomRewardResource;
        pickup.amount = roomRewardAmount;
        pickup.destination = rewardDestination;
        pickup.playerTag = playerTag;

        rewardSpawned = true;
    }

    public int GetCleanedCount() => cleanedSpotsCount;
    public int GetTotalCount() => totalSpots;
}
