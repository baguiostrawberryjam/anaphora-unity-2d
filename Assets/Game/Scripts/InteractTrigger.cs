using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InteractTrigger : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Button continueButton;
    public Button interactButton;
    public TMP_Text interactButtonText;

    [Header("Interact Label")]
    public string interactLabel = "Interact"; // set this per object in Inspector

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

    private static List<InteractTrigger> nearbyTriggers = new List<InteractTrigger>();

    private bool isTyping;

    private int currentIndex;
    private Coroutine typingCoroutine;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        interactButton.gameObject.SetActive(false);

        interactButton.onClick.RemoveAllListeners();
        interactButton.onClick.AddListener(OpenNearestDialogue);
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

        RefreshInteractButton();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        nearbyTriggers.Remove(this);

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

        SetPlayerBools();

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
    }

    void OnContinuePressed()
    {
        if (isTyping) return;

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

        RefreshInteractButton();
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

        if (setHasInteractedDrawer)
            playerMovementScript.hasInteractedDrawer = true;

        if(setHasInteractedRef)
            playerMovementScript.hasInteractedRef = true;
    }
}