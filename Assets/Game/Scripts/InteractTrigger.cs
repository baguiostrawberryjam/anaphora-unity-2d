using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractTrigger : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Button continueButton;
    public Button interactButton;

    [TextArea(2, 5)]
    public string[] dialogues;

    public float typingSpeed = 0.03f;
    public PlayerController playerMovementScript;

    [Header("Player Bools to Set on Interact")]
    public bool setHasFlashlight = false;
    public bool setHasInteractedSwitch = false;
    public bool setHasKey = false;

    private bool playerInside;
    private bool dialogueOpen;
    private bool isTyping;

    private int currentIndex;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        interactButton.gameObject.SetActive(false);

        interactButton.onClick.AddListener(OpenDialogue);
    }

    private void OnDestroy()
    {
        interactButton.onClick.RemoveListener(OpenDialogue);
        continueButton.onClick.RemoveListener(OnContinuePressed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
        if (!dialogueOpen)
            interactButton.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        interactButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerInside && !dialogueOpen && Input.GetKeyDown(KeyCode.F))
            OpenDialogue();
    }

    void OpenDialogue()
    {
        if (dialogueOpen) return;

        dialogueOpen = true;
        currentIndex = 0;

        interactButton.gameObject.SetActive(false);
        dialoguePanel.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinuePressed);

        SetPlayerBools();
        LockPlayer();
        StartCoroutine(TypeDialogue(dialogues[currentIndex]));
    }

    void SetPlayerBools()
    {
        if (playerMovementScript == null) return;

        if (setHasFlashlight)
            playerMovementScript.hasFlashlight = true;

        if (setHasInteractedSwitch)
            playerMovementScript.hasInteractedSwitch = true;

        if (setHasKey)
            playerMovementScript.hasKey = true;
    }

    IEnumerator TypeDialogue(string message)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void OnContinuePressed()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogues[currentIndex];
            isTyping = false;
            return;
        }

        currentIndex++;

        if (currentIndex < dialogues.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeDialogue(dialogues[currentIndex]));
        }
        else
        {
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        StopAllCoroutines();
        continueButton.onClick.RemoveAllListeners();

        dialoguePanel.SetActive(false);
        dialogueOpen = false;
        isTyping = false;

        UnlockPlayer();

        if (playerInside)
            interactButton.gameObject.SetActive(true);
    }

    void LockPlayer()
    {
        if (playerMovementScript != null)
            playerMovementScript.canMove = false;
    }

    void UnlockPlayer()
    {
        if (playerMovementScript != null)
            playerMovementScript.canMove = true;
    }
}