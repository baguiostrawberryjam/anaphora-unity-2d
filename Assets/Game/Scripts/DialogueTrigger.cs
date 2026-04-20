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
    private Coroutine typingCoroutine;

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
        if (!isRepeatable && hasTriggered) return;
        if (isTyping || dialoguePanel.activeSelf) return;

        hasTriggered = true;
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        currentIndex = 0;
        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinuePressed);

        LockPlayer();

        StartTyping(dialogues[currentIndex]);
    }

    void StartTyping(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;

        yield return new WaitForEndOfFrame();

        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    void OnContinuePressed()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            dialogueText.text = dialogues[currentIndex];
            isTyping = false;
            return;
        }

        currentIndex++;

        if (currentIndex < dialogues.Length)
        {
            StartTyping(dialogues[currentIndex]);
        }
        else
        {
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        continueButton.onClick.RemoveAllListeners();
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
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