using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    [Header("Item Panel")]
    [SerializeField] private GameObject itemsPanel;


    [Header("Random Items & Upgrades")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI objectOneName;
    [SerializeField] private TextMeshProUGUI objectTwoName;
    [SerializeField] private TextMeshProUGUI objectThreeName;
    [SerializeField] private TextMeshProUGUI objectOneDesc;
    [SerializeField] private TextMeshProUGUI objectTwoDesc;
    [SerializeField] private TextMeshProUGUI objectThreeDesc;
    [SerializeField] private Image objectOneImage;
    [SerializeField] private Image objectTwoImage;
    [SerializeField] private Image objectThreeImage;
    [SerializeField] private GameObject ItemOne;
    [SerializeField] private GameObject ItemTwo;
    [SerializeField] private GameObject ItemThree;
    private string[] itemsName = {"option 1", "option 2", "option 3", "option 4", "option 5", "option 6"};
    private string[] itemsDescription = {"option 1 description", "option 2 description", "option 3 description", "option 4 description", "option 5 description", "option 6 description"};
    private int randomNumberOne;
    private int randomNumberTwo;
    private int randomNumberThree;





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

    // Function to pick three random upgrades and fill them into the appropriate spot on the upgrade panel 
    private void RandomItemPicker()
    {

        // Sends three variables to become unique random numbers
        RandomizeMyNumbers(randomNumberOne, randomNumberTwo, randomNumberThree);



        upgradePanel.SetActive(true);






    }

    // Randomizes numbers for item selection
    private void RandomizeMyNumbers(int x, int y, int z)
    {

        // Randomizes the three optinons from 0 to the length of the array of items +1
        x = UnityEngine.Random.Range(0, (itemsName.Length + 1));
        y = UnityEngine.Random.Range(0, (itemsName.Length + 1));
        z = UnityEngine.Random.Range(0, (itemsName.Length + 1));

        return;
        // Checks to make sure none of the numbers are the same, then returns the values to RandomItemPicker
        /*
        if (x == y || x == z)
        {




        }
        else if (y == z)
        {
             


        }
        else
        {

            return;

        }

        */

    }

    // Sends the picked item from above and sends the information to the player items panel
    private void ItemSelected(int x)
    {




    }

    // Skips items selection
    public void ItemSkip()
    {

        upgradePanel.SetActive(false);


    }

    // Sends item one to inventory & closes menu
    public void ItemOneSelected()
    {

        ItemSelected(randomNumberOne);

        upgradePanel.SetActive(false);

    }

    // Sends item two to inventory & closes menu
    public void ItemTwoSelected()
    {

        ItemSelected(randomNumberTwo);

        upgradePanel.SetActive(false);

    }

    // Sends item three to inventory & closes menu
    public void ItemThreeSelected()
    {

        ItemSelected(randomNumberThree);

        upgradePanel.SetActive(false);

    }





}
