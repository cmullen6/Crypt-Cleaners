using UnityEngine;
using TMPro;

public class NPCDialogue2D : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("NPC Dialogue")]
    [TextArea(3, 6)]
    public string npcMessage = "Hello! Welcome to the village.";

    private bool playerInRange = false;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDialogue();
        }
    }

    private void ToggleDialogue()
    {
        bool isCurrentlyOpen = dialoguePanel.activeSelf;
        dialoguePanel.SetActive(!isCurrentlyOpen);

        if (!isCurrentlyOpen && dialogueText != null)
        {
            dialogueText.text = npcMessage;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
    }
}
