using System.Runtime.CompilerServices;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Player Death")]
    [SerializeField] private int resurrectMe = 3;
    [SerializeField] private float zoomTimer = 5f;
    [SerializeField] private Camera camera;
    [SerializeField] private TextMeshProUGUI resurrectText;

    private PlayerHealth playerHealth;

   

    private void Start()
    {

        // Set panels to off on game start
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false); 
        settingsPanel.SetActive(false);
        losePanel.SetActive(false);

        // Grabs needed variables & functions from PlayerHealth script
        playerHealth = GetComponent<PlayerHealth>();

    }


    private void Update()
    {

        // Pauses the game and turns on pause panel
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {

            pausePanel.SetActive(true);

            Time.timeScale = 0f;

        }


        // Checks if player has died
        if (playerHealth.isDead == true)
        {

            // Zooms in on player to show death animation





            zoomTimer -= Time.deltaTime;

            // Turns on lose panel once zoom is over
            if (zoomTimer < 0f)
            {

                resurrectText.text = "resurrections Left: " + resurrectMe;

                losePanel.SetActive(true);

            }

        }


    }


    // BUTTONS FOR MENUS & PANEL INTERACTIONS
    // ---------------------------------------

    // Unpauses game and turns off pause panel
    public void Unpause()
    {

        pausePanel.SetActive(false);

        Time.timeScale = 1f;

    }

    // On button click, sends player back to main menu
    public void MainMenu()
    {

        SceneManager.LoadScene(0);

    }

    // On button click, closes application
    public void Quit()
    {

        Application.Quit();

    }

    // On button click, opens settings panel
    public void Settings()
    {

        pausePanel.SetActive(false);

        settingsPanel.SetActive(true);

    }

    // On button click, opens controls panel
    public void Controls()
    {

        pausePanel.SetActive(false);

        controlsPanel.SetActive(true);

    }

    // On button click, returns to pause panel
    public void Back()
    {

        if (controlsPanel == true)
        {

            controlsPanel.SetActive(false);

            pausePanel.SetActive(true);

        }

        if (settingsPanel == true)
        {

            settingsPanel.SetActive(false);

            pausePanel.SetActive(true);

        }

    }

    // This respawns the player, only allowed 3
    public void Resurrect()
    {

        resurrectMe -= 1;
        zoomTimer = 5;

        // If respawnable, this will zoom out the camera and respawn the player
        if (resurrectMe >= 0)
        {





           playerHealth.Respawn();

        }
        else
        {

            return;

        }


    }

}