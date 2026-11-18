using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject itemsPanel;


    private bool paused = false;

    private void Start()
    {

        // Set panels to off on game start
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false); 
        settingsPanel.SetActive(false);
        itemsPanel.SetActive(false);
        paused = false;

    }
 

    private void Update()
    {

        // Pauses the game and turns on pause panel
        // If game is paused then acts as a quick unpause by hitting espace again
        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {

            if (!paused)
            {

                pausePanel.SetActive(true);
                Cursor.visible = true;
                paused = true;


                Time.timeScale = 0f;


            }
            else if (paused)
            {

                pausePanel.SetActive(false);
                controlsPanel.SetActive(false);
                settingsPanel.SetActive(false);
                Cursor.visible = false;
                paused = false;


                Time.timeScale = 1f;


            }

        }

        // turns on inventory on tab down
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {

            itemsPanel.SetActive(true);

        }

        // turns off inventory on tab release
        if (Keyboard.current.tabKey.wasReleasedThisFrame)
        {

            itemsPanel.SetActive(false);

        }

    }


    // BUTTONS FOR MENUS & PANEL INTERACTIONS
    // ---------------------------------------

    // Unpauses game and turns off pause panel
    public void Unpause()
    {

        pausePanel.SetActive(false);
        Cursor.visible = false;
        paused = false;


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

    // On button click, returns to hub
    public void ReturnToHub()
    {

        SceneManager.LoadScene(1);

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

}