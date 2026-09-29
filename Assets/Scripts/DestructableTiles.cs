using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableTiles : MonoBehaviour
{
    [Header("Tile Durability")]
    [SerializeField] private int minHitsToClean = 3;
    [SerializeField] private int maxHitsToClean = 5;

    private Tilemap destructableTilemap;

    private struct TileHealthData
    {
        public int currentHealth;
        public int maxHealth;
    }

    private Dictionary<Vector3Int, TileHealthData> tileHealthMap = new();

    private void Awake()
    {
        destructableTilemap = GetComponent<Tilemap>();
    }

    private void Start()
    {
        RegisterInitialTileCount();
    }

    // Scans the tilemap at launch and registers total placed tiles with the CleaningManager.
    private void RegisterInitialTileCount()
    {
        if (destructableTilemap == null) return;

        int initialTileCount = 0;

        foreach (Vector3Int pos in destructableTilemap.cellBounds.allPositionsWithin)
        {
            if (destructableTilemap.HasTile(pos))
            {
                initialTileCount++;
            }
        }

        if (CleaningManager.Instance != null && initialTileCount > 0)
        {
            CleaningManager.Instance.RegisterTileCount(initialTileCount);
        }
    }

    // Reduces tile health and opacity within a sweep radius, erasing when depleted.
    public void EraseTilesAt(Vector3 worldPosition, float radius)
    {
        if (destructableTilemap == null) return;

        Vector3 localPos = destructableTilemap.transform.InverseTransformPoint(worldPosition);
        Vector3Int centerCell = destructableTilemap.WorldToCell(localPos);
        int cellRange = Mathf.CeilToInt(radius);

        for (int x = -cellRange; x <= cellRange; x++)
        {
            for (int y = -cellRange; y <= cellRange; y++)
            {
                Vector3Int targetCell = centerCell + new Vector3Int(x, y, 0);
                targetCell.z = 0;

                if (!destructableTilemap.HasTile(targetCell))
                    continue;

                // 1. Initialize durability on first sweep hit
                if (!tileHealthMap.TryGetValue(targetCell, out TileHealthData healthData))
                {
                    int randomMax = Random.Range(minHitsToClean, maxHitsToClean + 1);
                    healthData = new TileHealthData
                    {
                        maxHealth = randomMax,
                        currentHealth = randomMax
                    };
                }

                // 2. Reduce health on each tick
                healthData.currentHealth--;

                // 3. Tile fully cleaned
                if (healthData.currentHealth <= 0)
                {
                    destructableTilemap.SetTile(targetCell, null);
                    tileHealthMap.Remove(targetCell);

                    if (CleaningManager.Instance != null)
                    {
                        CleaningManager.Instance.TileCleaned();
                    }
                }
                // 4. Tile damaged -> lower alpha opacity
                else
                {
                    tileHealthMap[targetCell] = healthData;

                    destructableTilemap.RemoveTileFlags(targetCell, TileFlags.LockColor);

                    float alpha = (float)healthData.currentHealth / healthData.maxHealth;
                    Color currentColor = destructableTilemap.GetColor(targetCell);
                    currentColor.a = alpha;
                    destructableTilemap.SetColor(targetCell, currentColor);
                }
            }
        }
    }
}
