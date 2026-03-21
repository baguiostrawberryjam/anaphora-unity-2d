using System.Collections;
using Unity.VisualScripting;
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

    // Interact prompt UI
    public GameObject interactPrompt;
    public Text interactPromptText;

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
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
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogueLines[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            zeroText();

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}
