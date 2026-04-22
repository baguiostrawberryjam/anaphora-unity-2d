using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;

public class InteractTrigger : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Button continueButton;
    public Button interactButton;
    public TMP_Text interactButtonText;

    [Header("On Dialogue Close Event")]
    public UnityEvent onDialogueClose;

    [Header("Interact Label")]
    public string interactLabel = "Interact";

    [TextArea(2, 5)]
    public string[] dialogues;

    public float typingSpeed = 0.03f;

    public PlayerController playerMovementScript;
    public JoystickController joystick;

    [Header("Player Bools To Set")]
    public bool setHasFlashlight = false;
    public bool setHasInteractedSwitch = false;
    public bool setHasKey = false;
    public bool setHasInteractedDrawer = false;
    public bool setHasInteractedRef = false;
    public bool setHasCheckedCaseFiles = false;

    private static List<InteractTrigger> nearbyTriggers = new List<InteractTrigger>();

    private bool isTyping;
    private int currentIndex;
    private Coroutine typingCoroutine;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        interactButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        nearbyTriggers.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!nearbyTriggers.Contains(this))
            nearbyTriggers.Add(this);

        interactButton.onClick.RemoveAllListeners();
        interactButton.onClick.AddListener(OpenNearestDialogue);

        RefreshInteractButton();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        nearbyTriggers.Remove(this);

        if (nearbyTriggers.Count == 0)
            interactButton.onClick.RemoveAllListeners();

        RefreshInteractButton();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            OpenNearestDialogue();
    }

    void OpenNearestDialogue()
    {
        if (dialoguePanel.activeSelf) return;

        InteractTrigger nearest = GetNearestTrigger();

        if (nearest != null)
            nearest.OpenDialogue();
    }

    InteractTrigger GetNearestTrigger()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) return null;

        float closest = Mathf.Infinity;
        InteractTrigger nearest = null;

        foreach (InteractTrigger trigger in nearbyTriggers)
        {
            float dist = Vector2.Distance(player.transform.position, trigger.transform.position);

            if (dist < closest)
            {
                closest = dist;
                nearest = trigger;
            }
        }

        return nearest;
    }

    void RefreshInteractButton()
    {
        bool hasNearby = nearbyTriggers.Count > 0 && !dialoguePanel.activeSelf;
        interactButton.gameObject.SetActive(hasNearby);

        if (hasNearby)
        {
            InteractTrigger nearest = GetNearestTrigger();
            if (nearest != null && interactButtonText != null)
                interactButtonText.text = nearest.interactLabel;
        }
    }

    void OpenDialogue()
    {
        currentIndex = 0;

        if (joystick != null)
            joystick.ForceReset();

        interactButton.gameObject.SetActive(false);

        dialoguePanel.SetActive(true);

        if (playerMovementScript != null)
            playerMovementScript.canMove = false;

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinuePressed);

        StartTyping(dialogues[currentIndex]);
    }

    void StartTyping(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeDialogue(line));
    }

    IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    void OnContinuePressed()
    {
        // If still typing, skip to full line
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

        // Typing done, go to next or close
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

    void CloseDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        if (playerMovementScript != null)
            playerMovementScript.canMove = true;

        SetPlayerBools();

        RefreshInteractButton();
        onDialogueClose?.Invoke();
    }

    void SetPlayerBools()
    {
        if (playerMovementScript == null) return;

        if (setHasFlashlight)
            PlayerController.hasFlashlight = true;

        if (setHasInteractedSwitch)
            PlayerController.hasInteractedSwitch = true;

        if (setHasKey)
            PlayerController.hasKey = true;

        if (setHasInteractedDrawer)
            PlayerController.hasInteractedDrawer = true;

        if (setHasInteractedRef)
            PlayerController.hasInteractedRef = true;

        if (setHasCheckedCaseFiles)
            PlayerController.hasCheckedCaseFiles = true;
    }
}