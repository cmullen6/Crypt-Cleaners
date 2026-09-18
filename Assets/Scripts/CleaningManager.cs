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

    public bool RoomComplete =>
        CleaningPercentage >= requiredCleanPercentage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        totalSpots = cleaningSpots.Count;
    }

    public void RegisterCleaningSpot(CleaningSpot spot)
    {
        if (!cleaningSpots.Contains(spot))
        {
            cleaningSpots.Add(spot);
            totalSpots = cleaningSpots.Count;
        }
    }

    public void SpotCleaned(CleaningSpot spot)
    {
        cleanedSpots++;
    }

    public int GetCleanedCount()
    {
        return cleanedSpots;
    }

    public int GetTotalCount()
    {
        return totalSpots;
    }
}
