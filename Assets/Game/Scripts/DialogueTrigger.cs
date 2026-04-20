using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Button continueButton;
    public Text dialogueText;
    public PlayerController playerMovementScript;

    [TextArea(2, 5)]
    public string[] dialogues;

    public float delayBeforeShow = 1f;
    public float typingSpeed = 0.03f;
    public bool useAutoTrigger = true;
    public bool isRepeatable = false;

    private bool hasTriggered = false;
    private bool isTyping = false;
    private int currentIndex = 0;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useAutoTrigger) return;
        if (!other.CompareTag("Player")) return;
        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        // If not repeatable, block after first trigger
        if (!isRepeatable && hasTriggered) return;

        // Prevent overlapping if already open
        if (isTyping || dialoguePanel.activeSelf) return;

        hasTriggered = true;
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        currentIndex = 0;
        dialoguePanel.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinuePressed);

        LockPlayer();

        // Wait a frame before typing to avoid jumbled first line
        yield return null;

        StartCoroutine(TypeLine(dialogues[currentIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line)
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
            StartCoroutine(TypeLine(dialogues[currentIndex]));
        }
        else
        {
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        StopAllCoroutines();
        continueButton.onClick.RemoveAllListeners();
        dialoguePanel.SetActive(false);
        isTyping = false;

        UnlockPlayer();
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