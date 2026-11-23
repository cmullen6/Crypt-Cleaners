
using UnityEngine;

public class CleaningMopController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform mopVisual;
    [SerializeField] private CleaningTool cleaningTool;
    [SerializeField] private Animator mopAnimator;

    [Header("Mop Movement")]
    [Tooltip("Maximum distance the mop can move from the player.")]
    [SerializeField, Min(0.1f)] private float mopDistance = 2.5f;

    [Tooltip("Rotation offset if the mop sprite points in the wrong direction.")]
    [SerializeField] private float rotationOffset = 0f;

    [Header("Input")]
    [SerializeField] private int mouseButton = 0;

    [Header("Animation")]
    [Tooltip("Animator bool parameter used to control the cleaning animation.")]
    [SerializeField] private string cleaningParameter = "IsCleaning";

    private int cleaningParameterHash;
    private bool isCleaning;

    public bool IsCleaning => isCleaning;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Automatically find the Animator if one wasn't assigned.
        if (mopAnimator == null && mopVisual != null)
        {
            mopAnimator = mopVisual.GetComponent<Animator>();

            if (mopAnimator == null)
            {
                mopAnimator = mopVisual.GetComponentInChildren<Animator>(true);
            }
        }

        cleaningParameterHash = Animator.StringToHash(cleaningParameter);

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

        // Start cleaning when the mouse button is pressed.
        if (Input.GetMouseButtonDown(mouseButton))
        {
            StartCleaning();
        }

        // Move the mop and clean while the button is held.
        if (Input.GetMouseButton(mouseButton) && isCleaning)
        {
            UpdateMopPosition();

            cleaningTool.ExecuteClean(
                transform.position,
                Time.deltaTime
            );
        }

        // Stop cleaning when the mouse button is released.
        if (Input.GetMouseButtonUp(mouseButton))
        {
            StopCleaning();
        }
    }

    private void StartCleaning()
    {
        if (isCleaning)
            return;

        isCleaning = true;

        // Set the animation parameter before activating the visual.
        if (mopAnimator != null)
            mopAnimator.SetBool(cleaningParameterHash, true);

        mopVisual.gameObject.SetActive(true);

        UpdateMopPosition();

        // Reset the cleaning sweep at the mop's current position.
        cleaningTool.BeginCleaning();
    }

    private void StopCleaning()
    {
        if (!isCleaning)
            return;

        isCleaning = false;

        if (mopAnimator != null)
            mopAnimator.SetBool(cleaningParameterHash, false);

        mopVisual.gameObject.SetActive(false);

        cleaningTool.StopCleaning();
    }

    private void UpdateMopPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Find the distance from the camera to the player's Z plane.
        mouseScreenPosition.z = Mathf.Abs(
            transform.position.z - mainCamera.transform.position.z
        );

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        mouseWorldPosition.z = transform.position.z;

        // Find the direction from the player toward the cursor.
        Vector2 direction =
            (Vector2)(mouseWorldPosition - transform.position);

        float distanceToMouse = direction.magnitude;

        // Prevent division by zero when the cursor is exactly on the player.
        if (distanceToMouse > 0.001f)
            direction /= distanceToMouse;
        else
            direction = Vector2.zero;

        // Clamp the mop's position to its maximum reach.
        float clampedDistance = Mathf.Min(distanceToMouse, mopDistance);

        Vector3 targetPosition =
            transform.position + (Vector3)(direction * clampedDistance);

        targetPosition.z = transform.position.z;

        mopVisual.position = targetPosition;

        // Rotate the mop toward the cursor.
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            mopVisual.rotation =
                Quaternion.Euler(0f, 0f, angle + rotationOffset);
        }
    }

    private void OnDisable()
    {
        // Prevent the mop from remaining in a cleaning state
        // if this controller gets disabled.
        if (isCleaning)
        {
            isCleaning = false;

            if (mopAnimator != null)
                mopAnimator.SetBool(cleaningParameterHash, false);

            if (cleaningTool != null)
                cleaningTool.StopCleaning();

            if (mopVisual != null)
                mopVisual.gameObject.SetActive(false);
        }
    }
}

