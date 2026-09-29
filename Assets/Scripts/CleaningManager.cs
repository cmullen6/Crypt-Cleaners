using System.Collections.Generic;
using UnityEngine;

public class CleaningManager : MonoBehaviour
{
    public static CleaningManager Instance { get; private set; }

    [Header("Room Requirement")]
    [SerializeField, Range(0f, 1f)]
    private float requiredCleanPercentage = 0.80f;

    private List<CleaningSpot> cleaningSpots = new();

    private int totalSpots;
    private int cleanedSpots;

    public float CleaningPercentage
    {
        get
        {
            if (totalSpots == 0)
                return 0f;

            return (float)cleanedSpots / totalSpots;
        }
    }

    public bool RoomComplete => CleaningPercentage >= requiredCleanPercentage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Registers individual interactable cleaning objects (trash, spills, spots).
    public void RegisterCleaningSpot(CleaningSpot spot)
    {
        if (!cleaningSpots.Contains(spot))
        {
            cleaningSpots.Add(spot);
            totalSpots++;
        }
    }

    // Registers total initial goo tiles into the room quota.
    public void RegisterTileCount(int count)
    {
        totalSpots += count;
    }

    // Called when an interactable CleaningSpot is fully completed.
    public void SpotCleaned(CleaningSpot spot)
    {
        cleanedSpots++;
    }

    // Called when a single tilemap goo cell is fully erased.
    public void TileCleaned()
    {
        cleanedSpots++;
    }

    public int GetCleanedCount() => cleanedSpots;
    public int GetTotalCount() => totalSpots;
}
