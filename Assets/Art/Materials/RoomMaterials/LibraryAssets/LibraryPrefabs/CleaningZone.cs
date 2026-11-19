using UnityEngine;
using UnityEngine.UI;

public class CleaningZone : MonoBehaviour
{
    public GameObject cleaningCanvas;
    public Slider cleaningSlider;
    public SpriteRenderer assetSprite;

    private ParticleSystem dirtyParticles;
    private ParticleSystem bubbleParticles;
    private ParticleSystem.EmissionModule dirtEmission;
    private float initialDirtRate = 15f;
    private bool isCleaned = false;
    private float lastSliderValue;

    void Awake()
    {
        GameObject dirtObj = new GameObject("DirtyParticles");
        dirtObj.transform.SetParent(transform);
        dirtObj.transform.localPosition = Vector3.zero;
        dirtObj.transform.localRotation = Quaternion.identity;
        dirtObj.transform.localScale = Vector3.one;

        dirtyParticles = dirtObj.AddComponent<ParticleSystem>();
        var dirtMain = dirtyParticles.main;
        dirtMain.startColor = new Color(0.8f, 0.55f, 1f, 0.85f);
        dirtMain.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.24f);
        dirtMain.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        dirtMain.startLifetime = new ParticleSystem.MinMaxCurve(1f, 2f);
        dirtMain.gravityModifier = -0.1f;
        dirtMain.simulationSpace = ParticleSystemSimulationSpace.World;

        dirtEmission = dirtyParticles.emission;
        dirtEmission.rateOverTime = initialDirtRate;

        var dirtShape = dirtyParticles.shape;
        if (assetSprite != null)
        {
            dirtShape.shapeType = ParticleSystemShapeType.Sprite;
            dirtShape.sprite = assetSprite.sprite;
            dirtShape.spriteRenderer = assetSprite;
        }

        var dirtRenderer = dirtyParticles.GetComponent<ParticleSystemRenderer>();
        dirtRenderer.material = new Material(Shader.Find("Sprites/Default"));
        dirtRenderer.sortingOrder = assetSprite != null ? assetSprite.sortingOrder + 1 : 5;

        GameObject bubbleObj = new GameObject("BubbleParticles");
        bubbleObj.transform.SetParent(transform);
        bubbleObj.transform.localPosition = Vector3.zero;
        bubbleObj.transform.localRotation = Quaternion.identity;
        bubbleObj.transform.localScale = Vector3.one;

        bubbleParticles = bubbleObj.AddComponent<ParticleSystem>();
        var bubbleMain = bubbleParticles.main;
        bubbleMain.startColor = new Color(0.75f, 0.92f, 1f, 0.75f);
        bubbleMain.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.24f);
        bubbleMain.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        bubbleMain.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 1.2f);
        bubbleMain.gravityModifier = -0.15f;
        bubbleMain.simulationSpace = ParticleSystemSimulationSpace.World;

        var bubbleEmission = bubbleParticles.emission;
        bubbleEmission.rateOverTime = initialDirtRate;

        var bubbleShape = bubbleParticles.shape;
        if (assetSprite != null)
        {
            bubbleShape.shapeType = ParticleSystemShapeType.Sprite;
            bubbleShape.sprite = assetSprite.sprite;
            bubbleShape.spriteRenderer = assetSprite;
        }

        var bubbleRenderer = bubbleParticles.GetComponent<ParticleSystemRenderer>();
        bubbleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        bubbleRenderer.sortingOrder = assetSprite != null ? assetSprite.sortingOrder + 2 : 6;
    }

    void Start()
    {
        cleaningCanvas.SetActive(false);
        dirtyParticles.Play();
        bubbleParticles.Stop();
        lastSliderValue = cleaningSlider.value;
    }

    void Update()
    {
        if (isCleaned) return;

        float progress = Mathf.InverseLerp(cleaningSlider.minValue, cleaningSlider.maxValue, cleaningSlider.value);
        dirtEmission.rateOverTime = Mathf.Lerp(initialDirtRate, 0f, progress);

        bool isActivelyCleaning = cleaningSlider.value > lastSliderValue;
        lastSliderValue = cleaningSlider.value;

        if (isActivelyCleaning)
        {
            if (!bubbleParticles.isPlaying) bubbleParticles.Play();
        }
        else
        {
            if (bubbleParticles.isPlaying) bubbleParticles.Stop();
        }

        if (cleaningSlider.value >= cleaningSlider.maxValue)
        {
            isCleaned = true;
            cleaningCanvas.SetActive(false);
            dirtyParticles.Stop();
            bubbleParticles.Stop();
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
            bubbleParticles.Stop();
        }
    }
}
