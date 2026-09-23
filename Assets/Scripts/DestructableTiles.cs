using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableTiles : MonoBehaviour
{
    public Tilemap destructableTilemap;

    private void Awake()
    {
        destructableTilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Get the player's feet bounds in World Space
            Bounds playerBounds = other.bounds;
            Vector3 feetWorldPos = new Vector3(playerBounds.center.x, playerBounds.min.y + 0.1f, 0f);

            // 2. Convert World Space -> Tilemap Local Space (handles Grid parent offsets)
            Vector3 localPos = destructableTilemap.transform.InverseTransformPoint(feetWorldPos);

            // 3. Convert Local Space -> Tilemap Cell Coordinate
            Vector3Int cellPosition = destructableTilemap.WorldToCell(localPos);

            // Force Z to 0 (2D Tilemaps strictly use Z = 0)
            cellPosition.z = 0;

            // 4. Erase primary tile under feet
            if (destructableTilemap.HasTile(cellPosition))
            {
                destructableTilemap.SetTile(cellPosition, null);
            }

            // 5. Secondary check: Sample slight left/right offsets to ensure continuous cleaning
            CheckAndErase(localPos + new Vector3(-0.2f, 0f, 0f));
            CheckAndErase(localPos + new Vector3(0.2f, 0f, 0f));
        }
    }

    private void CheckAndErase(Vector3 localPos)
    {
        Vector3Int cell = destructableTilemap.WorldToCell(localPos);
        cell.z = 0;

        if (destructableTilemap.HasTile(cell))
        {
            destructableTilemap.SetTile(cell, null);
        }
    }
}