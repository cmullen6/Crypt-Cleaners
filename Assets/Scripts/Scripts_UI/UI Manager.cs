using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Player Death")]
    [SerializeField] private int resurrectMe = 3;
    [SerializeField] private int zoomTimer;
    [SerializeField] private Camera camera;

    private PlayerHealth isDead;
   

    private void Start()
    {

        // Set panels to off on game start
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false); 
        settingsPanel.SetActive(false);
        losePanel.SetActive(false);

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
        if (isDead == true)
        {

            // Zooms in on player to show death animation


            // Turns on lose panel
            losePanel.SetActive(true);

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

        if (resurrectMe < 0)
        {




        }
        else
        {




        }


    }

}