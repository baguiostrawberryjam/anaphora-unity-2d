using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class NPC : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogueLines;
    private int index;

    public float wordSpeed;
    public bool playerInRange;

    public GameObject interactPrompt;
    public Text interactPromptText;

    void Start()
    {
        // Initialize the interact prompt UI state when the NPC is first created
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        // Handle player input for starting/closing dialogue and manage continue button visibility
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialoguePanel.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                if (interactPrompt != null)
                    interactPrompt.SetActive(false);

                StartCoroutine(Typing());
            }
        }

        if (dialogueLines != null && dialogueLines.Length > 0 && dialogueText != null)
        {
            if (dialogueText.text == dialogueLines[index])
            {
                if (continueButton != null) continueButton.SetActive(true);
            }
            else
            {
                if (continueButton != null) continueButton.SetActive(false);
            }
        }
    }

    public void nextLine()
    {
        // Advance to the next dialogue line or close the dialogue if at the end
        if (continueButton != null) continueButton.SetActive(false);

        if (index < dialogueLines.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    public void zeroText()
    {
        // Reset dialogue state and hide the dialogue panel
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);
    }

    IEnumerator Typing()
    {
        // Type out the active dialogue line character-by-character with a small delay
        foreach (char letter in dialogueLines[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect when the player enters the NPC's trigger area and show the interact prompt
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPrompt != null)
            {
                if (interactPromptText != null) 
                    interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Detect when the player leaves the NPC's trigger area and reset dialogue state
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            zeroText();

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}
