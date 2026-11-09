
using UnityEngine;

public class CleaningMopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform mopVisual;
    [SerializeField] private CleaningTool cleaningTool;

    [Header("Mop Position")]
    [Tooltip("Distance from the player to the mop.")]
    [SerializeField] private float mopDistance = 0.8f;

    [Tooltip("Rotation offset if the mop sprite points in the wrong direction.")]
    [SerializeField] private float rotationOffset = 0f;

    [Header("Input")]
    [SerializeField] private int mouseButton = 0;

    private bool isCleaning;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mopVisual != null)
            mopVisual.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (mainCamera == null ||
            mopVisual == null ||
            cleaningTool == null)
        {
            return;
        }

        // Start cleaning.
        if (Input.GetMouseButtonDown(mouseButton))
        {
            StartCleaning();
        }

        // Continue cleaning while the mouse is held.
        if (Input.GetMouseButton(mouseButton) && isCleaning)
        {
            UpdateMopPosition();

            cleaningTool.ExecuteClean(
                transform.position,
                Time.deltaTime
            );
        }

        // Stop cleaning when the mouse is released.
        if (Input.GetMouseButtonUp(mouseButton))
        {
            StopCleaning();
        }
    }

    private void StartCleaning()
    {
        isCleaning = true;

        mopVisual.gameObject.SetActive(true);

        UpdateMopPosition();

        cleaningTool.BeginCleaning();
    }

    private void StopCleaning()
    {
        isCleaning = false;

        mopVisual.gameObject.SetActive(false);

        cleaningTool.StopCleaning();
    }

    private void UpdateMopPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.z =
            Mathf.Abs(mainCamera.transform.position.z);

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        mouseWorldPosition.z = transform.position.z;

        Vector2 direction =
            (mouseWorldPosition - transform.position).normalized;

        if (direction.sqrMagnitude < 0.001f)
            return;

        // Place the mop around the player in the mouse direction.
        mopVisual.position =
            transform.position + (Vector3)(direction * mopDistance);

        // Rotate the mop toward the mouse.
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        mopVisual.rotation =
            Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

    public bool IsCleaning => isCleaning;
}
