using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;

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
    [SerializeField] private Image objectOneSprite;
    [SerializeField] private Image objectTwoSprite;
    [SerializeField] private Image objectThreeSprite;
    private int randomNumberOne;
    private int randomNumberTwo;
    private int randomNumberThree;


    [Header("Sprites for upgrades")]
    [SerializeField] private Sprite[] itemSprites;


    // Genuinely every single item in the game, sorry but also not sorry really. just ignore it or something
    private string[] itemsName = { "option 1", "option 2", "option 3", "option 4", "option 5", "option 6" };
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


        // TESTING BUTTON TO TEST THE WORK ... :)
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {

            // Randomizes item selection
            RandomItemPicker();

            // Turns on panel
            upgradePanel.SetActive(true);

        }

    }

    // Function to pick three random upgrades and fill them into the appropriate spot on the upgrade panel 
    private void RandomItemPicker()
    {

        // Randomizes three variables into unique numbers for random item selection
        randomNumberOne = UnityEngine.Random.Range(0, (itemsName.Length + 1));
        randomNumberTwo = UnityEngine.Random.Range(0, (itemsName.Length + 1));
        randomNumberThree = UnityEngine.Random.Range(0, (itemsName.Length + 1));


        // Creates item on UI Panel. Name, Image & Description
        // Name
        objectOneName.SetText(itemsName[randomNumberOne]);
        objectTwoName.SetText(itemsName[randomNumberTwo]);
        objectThreeName.SetText(itemsName[randomNumberThree]);

        // Image
        objectOneSprite.sprite = itemSprites[randomNumberOne];
        objectTwoSprite.sprite = itemSprites[randomNumberTwo];
        objectThreeSprite.sprite = itemSprites[randomNumberThree];

        // Description
        objectOneDesc.SetText(itemsDescription[randomNumberOne]);
        objectTwoDesc.SetText(itemsDescription[randomNumberTwo]);
        objectThreeDesc.SetText(itemsDescription[randomNumberTwo]);

    }

    // Sends the picked item from above and sends the information to the player items panel
    private void ItemSelected(int x)
    {

        // int x is the upgrade item the player selected
        

        // Send item to player inventory








        // Make gameplay modification from item happen








    }

    // Skips items selection
    public void ItemSkip()
    {

        upgradePanel.SetActive(false);

    }

    // Sends item one to inventory & closes menu
    public void ItemOneSelected()
    {

        // Sends item to inventory
        ItemSelected(randomNumberOne);

        // Removes item from total array item pool
        Remove(itemsName, randomNumberOne);
        Remove(itemSprites, randomNumberOne);
        Remove(itemsDescription, randomNumberOne);

        // Turns off panel
        upgradePanel.SetActive(false);

    }

    // Sends item two to inventory & closes menu
    public void ItemTwoSelected()
    {

        // Sends item to inventory
        ItemSelected(randomNumberTwo);

        // Removes item from total array item pool
        Remove(itemsName, randomNumberTwo);
        Remove(itemSprites, randomNumberTwo);
        Remove(itemsDescription, randomNumberTwo);

        // Turns off panel
        upgradePanel.SetActive(false);

    }

    // Sends item three to inventory & closes menu
    public void ItemThreeSelected()
    {

        // Sends item to inventory
        ItemSelected(randomNumberThree);

        // Removes item from total array item pool
        Remove(itemsName, randomNumberThree);
        Remove(itemSprites, randomNumberThree);
        Remove(itemsDescription, randomNumberThree);

        // Turns off panel
        upgradePanel.SetActive(false);

    }

    private static void Remove(string[] array, int index)
    {




    }

    private static void Remove(Sprite[] array, int index)
    {




    }

}