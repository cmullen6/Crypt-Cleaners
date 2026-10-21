
using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    [Header("Hitstop Settings")]
    [SerializeField, Min(0.01f)]
    private float defaultDuration = 0.06f;

    private Coroutine hitStopRoutine;
    private float requestedDuration;
    private float previousTimeScale = 1f;
    private bool isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StopTime()
    {
        StopTime(defaultDuration);
    }

    public void StopTime(float duration)
    {
        if (duration <= 0f || Time.timeScale <= 0f)
            return;

        requestedDuration = Mathf.Max(requestedDuration, duration);

        if (hitStopRoutine == null)
        {
            previousTimeScale = Time.timeScale;
            hitStopRoutine = StartCoroutine(HitStopCoroutine());
        }
    }

    private IEnumerator HitStopCoroutine()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Use real time so the freeze can end while gameplay is paused.
        yield return new WaitForSecondsRealtime(requestedDuration);

        requestedDuration = 0f;
        Time.timeScale = previousTimeScale;
        isPaused = false;
        hitStopRoutine = null;
    }

    public bool IsInHitStop => isPaused;
}
