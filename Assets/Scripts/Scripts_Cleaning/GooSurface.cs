using System.Collections.Generic;
using UnityEngine;

public class GooSurface : MonoBehaviour
{
    [Header("Goo Rendering")]
    [SerializeField] private SpriteRenderer gooRenderer;
    [SerializeField] private Shader paintShader;

    [SerializeField, Min(64)]
    private int maskResolution = 1024;

    [Header("Brush")]
    [SerializeField, Range(0f, 1f)]
    private float brushHardness = 0.35f;

    [SerializeField, Range(0f, 1f)]
    private float brushStrength = 1f;

    [Header("Room Cleaning Progress")]
    [SerializeField, Min(1)]
    private int progressColumns = 40;

    [SerializeField, Min(1)]
    private int progressRows = 24;

    private Material gooMaterial;
    private Material paintMaterial;

    private RenderTexture maskA;
    private RenderTexture maskB;

    private RenderTexture currentMask;

    private bool[,] cleanedCells;

    private int totalCells;
    private int cleanedCellCount;

    private Bounds surfaceBounds;

    private bool registeredWithManager;

    public float CleaningPercentage
    {
        get
        {
            if (totalCells == 0)
                return 0f;

            return cleanedCellCount / (float)totalCells;
        }
    }

    private void Awake()
    {
        if (gooRenderer == null)
            gooRenderer = GetComponent<SpriteRenderer>();

        if (gooRenderer == null)
        {
            Debug.LogError(
                "GooSurface requires a SpriteRenderer.", this);
            enabled = false;
            return;
        }

        if (paintShader == null)
            paintShader = Shader.Find("Hidden/CryptCleaners/GooMaskPaint");

        if (paintShader == null)
        {
            Debug.LogError(
                "Could not find GooMaskPaint shader.", this);
            enabled = false;
            return;
        }

        surfaceBounds = gooRenderer.bounds;

        CreateMaskTextures();
        CreateMaterials();
        InitializeProgressGrid();
    }

    private void Start()
    {
        RegisterWithCleaningManager();
    }

    private void CreateMaskTextures()
    {
        maskA = CreateRenderTexture();
        maskB = CreateRenderTexture();

        // White means the entire goo surface is dirty.
        ClearTexture(maskA, Color.white);
        ClearTexture(maskB, Color.white);

        currentMask = maskA;
    }

    private RenderTexture CreateRenderTexture()
    {
        RenderTexture texture = new RenderTexture(
            maskResolution,
            maskResolution,
            0,
            RenderTextureFormat.ARGB32
        );

        texture.name = "Goo Dirt Mask";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Create();

        return texture;
    }

    private void ClearTexture(RenderTexture texture, Color color)
    {
        RenderTexture previous = RenderTexture.active;

        RenderTexture.active = texture;

        GL.Clear(true, true, color);

        RenderTexture.active = previous;
    }

    private void CreateMaterials()
    {
        gooMaterial = new Material(gooRenderer.sharedMaterial);

        paintMaterial = new Material(paintShader);

        gooMaterial.SetTexture("_DirtMask", currentMask);

        gooRenderer.material = gooMaterial;
    }

    private void InitializeProgressGrid()
    {
        totalCells = progressColumns * progressRows;

        cleanedCells = new bool[
            progressColumns,
            progressRows
        ];
    }

    private void RegisterWithCleaningManager()
    {
        if (registeredWithManager)
            return;

        if (CleaningManager.Instance == null)
        {
            Debug.LogWarning(
                "GooSurface: CleaningManager not found.", this);
            return;
        }

        CleaningManager.Instance.RegisterTileCount(totalCells);

        registeredWithManager = true;
    }

    public void CleanAtWorldPosition(
        Vector3 worldPosition,
        float brushRadius)
    {
        if (currentMask == null)
            return;

        if (!surfaceBounds.Contains(worldPosition))
            return;

        Vector2 uv = WorldToUV(worldPosition);

        PaintMask(uv, brushRadius);

        MarkProgressCells(worldPosition, brushRadius);
    }

    private Vector2 WorldToUV(Vector3 worldPosition)
    {
        float u = Mathf.InverseLerp(
            surfaceBounds.min.x,
            surfaceBounds.max.x,
            worldPosition.x
        );

        float v = Mathf.InverseLerp(
            surfaceBounds.min.y,
            surfaceBounds.max.y,
            worldPosition.y
        );

        return new Vector2(u, v);
    }

    private void PaintMask(Vector2 uv, float brushRadius)
    {
        RenderTexture destination =
            currentMask == maskA ? maskB : maskA;

        paintMaterial.SetVector(
            "_BrushUV",
            new Vector4(uv.x, uv.y, 0f, 0f)
        );

        paintMaterial.SetFloat(
            "_BrushRadius",
            brushRadius
        );

        paintMaterial.SetFloat(
            "_BrushHardness",
            brushHardness
        );

        paintMaterial.SetFloat(
            "_BrushStrength",
            brushStrength
        );

        paintMaterial.SetVector(
            "_SurfaceSize",
            new Vector4(
                surfaceBounds.size.x,
                surfaceBounds.size.y,
                0f,
                0f
            )
        );

        Graphics.Blit(
            currentMask,
            destination,
            paintMaterial
        );

        currentMask = destination;

        gooMaterial.SetTexture(
            "_DirtMask",
            currentMask
        );
    }

    private void MarkProgressCells(
        Vector3 worldPosition,
        float brushRadius)
    {
        float cellWidth =
            surfaceBounds.size.x / progressColumns;

        float cellHeight =
            surfaceBounds.size.y / progressRows;

        for (int x = 0; x < progressColumns; x++)
        {
            for (int y = 0; y < progressRows; y++)
            {
                if (cleanedCells[x, y])
                    continue;

                Vector3 cellCenter = new Vector3(
                    surfaceBounds.min.x +
                    (x + 0.5f) * cellWidth,

                    surfaceBounds.min.y +
                    (y + 0.5f) * cellHeight,

                    worldPosition.z
                );

                // Only mark a cell when the brush reaches it.
                if (Vector2.Distance(
                    worldPosition,
                    cellCenter) <= brushRadius)
                {
                    cleanedCells[x, y] = true;
                    cleanedCellCount++;

                    if (CleaningManager.Instance != null)
                    {
                        CleaningManager.Instance.TileCleaned();
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (gooMaterial != null)
            Destroy(gooMaterial);

        if (paintMaterial != null)
            Destroy(paintMaterial);

        if (maskA != null)
        {
            maskA.Release();
            Destroy(maskA);
        }

        if (maskB != null)
        {
            maskB.Release();
            Destroy(maskB);
        }
    }
}
