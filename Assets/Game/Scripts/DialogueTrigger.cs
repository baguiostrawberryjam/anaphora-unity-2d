using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Trigger Once Per Session (Even on Scene Changes)")]
    public bool saveTriggered = false;
    public string triggerID = "";

    // Added two small floats so you can tweak the voice pitch directly, but no drag-and-drop required!
    [Header("Audio Settings")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    public static HashSet<string> sessionTriggers = new HashSet<string>();

    private bool hasTriggered = false;
    private bool isTyping = false;

    private int currentIndex = 0;
    private Coroutine typingCoroutine;

    // Automatically grabbed from your dialoguePanel
    private AudioSource dialogueAudio;

    private void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);

            // Automatically grab the Audio Source attached to the dialogue panel!
            dialogueAudio = dialoguePanel.GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useAutoTrigger) return;
        if (!other.CompareTag("Player")) return;

        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (isTyping || dialoguePanel.activeSelf) return;

        if (!isRepeatable)
        {
            if (saveTriggered)
            {
                if (string.IsNullOrEmpty(triggerID)) return;

                if (sessionTriggers.Contains(triggerID))
                    return;

                sessionTriggers.Add(triggerID);
            }
            else
            {
                if (hasTriggered)
                    return;

                hasTriggered = true;
            }
        }

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
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;

            // --- ANIMAL CROSSING AUDIO EFFECT ---
            // We check if the letter is not a space so we don't play sounds on blank spaces
            if (dialogueAudio != null && letter != ' ')
            {
                dialogueAudio.pitch = Random.Range(minPitch, maxPitch);
                // Using .Play() instead of PlayOneShot stops the previous letter's sound instantly, 
                // preventing a chaotic overlapping mess of audio when typing very fast!
                dialogueAudio.Play();
            }

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

            // Stop the audio if the player fast-forwards the text
            if (dialogueAudio != null) dialogueAudio.Stop();

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