using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu UI Panels")]
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {

        // Set panels to off on game start
        controlsPanel.SetActive(false);
        settingsPanel.SetActive(false);

    }


    // BUTTONS FOR MENUS & PANEL INTERACTIONS
    // ---------------------------------------

    // On button click, starts the game
    public void StartGame()
    {

       // SceneManager.LoadScene();

    }

    // On button click, quick starts the game - sends directly into a run with previously selected weapon
    public void QuickStartGame()
    {

       // SceneManager.LoadScene();

    }

    // On button click, closes application
    public void Quit()
    {

        Application.Quit();

    }

    // On button click, opens settings panel
    public void Settings()
    {

        settingsPanel.SetActive(true);

    }

    // On button click, opens controls panel
    public void Controls()
    {

        controlsPanel.SetActive(true);

    }

    // On button click, returns back from panel to the main menu
    public void Back()
    {

        if (controlsPanel == true)
        {

            controlsPanel.SetActive(false);

        }

        if (settingsPanel == true)
        {

            settingsPanel.SetActive(false);

        }

    }

}