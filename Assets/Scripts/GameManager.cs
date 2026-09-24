using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    [Header("Item Panel")]
    [SerializeField] private GameObject itemsPanel;


    [Header("Random Items & Upgrades")]
    [SerializeField] private GameObject upgradePanel;
    private string[] itemsName = {"option 1", "option 2", "option 3", "option 4", "option 5", "option 6"};
    private string[] itemsDescription = { "option 1 description", "option 2 description", "option 3 description", "option 4 description", "option 5 description", "option 6 description" };





    void Start()
    {

        // Turns off panel on game start
        itemsPanel.SetActive(false);
        upgradePanel.SetActive(false);
        
    }

    void Update()
    {

        // Toggles player item inventory on tab hold
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {

            itemsPanel.SetActive(true);

        }

        if (Keyboard.current.tabKey.wasReleasedThisFrame)
        {

            itemsPanel.SetActive(false);

        }

    }


    // Logic to pick three random upgrades and fill them into the appropriate spot on the upgrade panel 
    private void RandomItemPicker()
    {





        upgradePanel.SetActive(true);







    }

    // Sends the picked item from above and sends the information to the player items panel
    private void ItemSelected()
    {




    }

    // Skips items selection
    public void ItemSkip()
    {

        upgradePanel.SetActive(false);

    }









}
