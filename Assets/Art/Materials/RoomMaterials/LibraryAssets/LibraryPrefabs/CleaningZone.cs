using UnityEngine;
using UnityEngine.UI;

public class CleaningZone : MonoBehaviour
{
    public GameObject cleaningCanvas;
    public Slider cleaningSlider;

    private bool isCleaned = false;

    void Start()
    {
        cleaningCanvas.SetActive(false);
    }

    void Update()
    {
        if (!isCleaned && cleaningSlider.value >= cleaningSlider.maxValue)
        {
            isCleaned = true;
            cleaningCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCleaned)
        {
            cleaningCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cleaningCanvas.SetActive(false);
        }
    }
}
