using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableTiles : MonoBehaviour
{
    private Tilemap destructableTilemap;

    private void Awake()
    {
        destructableTilemap = GetComponent<Tilemap>();
    }


    // Erases goo tiles within a given radius around a world position.
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

                // CRITICAL PERF FIX: Check HasTile FIRST so SetTile is only called 
                // when there is actually a tile to destroy!
                if (destructableTilemap.HasTile(targetCell))
                {
                    destructableTilemap.SetTile(targetCell, null);
                }
            }
        }
    }
}
