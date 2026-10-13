using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI recAText;
    [SerializeField] private TextMeshProUGUI recBText;
    [SerializeField] private TextMeshProUGUI recCText;
    [SerializeField] private TextMeshProUGUI recDText;
    [SerializeField] private Button testButton;

    private int playerGold;
    private int playerRecA;
    private int playerRecB;
    private int playerRecC;
    private int playerRecD;

    [Header("Item Panel")]
    [SerializeField] private GameObject itemsPanel;




    void Start()
    {

        // Turns off panel on game start
        itemsPanel.SetActive(false);

    }

    void Update()
    {

        // Updates variables with the instance version
        playerGold = ResourceManager.instance.gold;
        playerRecA = ResourceManager.instance.recA;
        playerRecB = ResourceManager.instance.recB;
        playerRecC = ResourceManager.instance.recC;
        playerRecD = ResourceManager.instance.recD;

        // Toggles player item inventory on tab hold
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {

            goldText.text = "Gold: " + playerGold;
            recAText.text = "Resource A: " + playerRecA;
            recBText.text = "Resource B: " + playerRecB;
            recCText.text = "Resource C: " + playerRecC;
            recDText.text = "Resource D: " + playerRecD;

            itemsPanel.SetActive(true);

        }

        if (Keyboard.current.tabKey.wasReleasedThisFrame)
        {

            itemsPanel.SetActive(false);

        }

        // Test to make sure item tracking works
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {

            ResourceManager.instance.gold += 1;
            ResourceManager.instance.recA += 1;

        }


        // Test to make sure you can remove value from the instance
        // Also test to make sure you can "lock" things behind a set value
        if (playerGold < 5)
        {

            testButton.interactable = false;

        }
        else
        {

            testButton.interactable = true;

        }


    }


    // Test button to make sure instance resource changing is working
    public void BuyButton()
    {

        ResourceManager.instance.recC += 10;
        ResourceManager.instance.gold -= 5;

    }


}