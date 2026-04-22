using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NPCDialogueEntry
{
    public string characterName;
    public Sprite characterImage;

    [TextArea(2, 5)]
    public string[] lines;
}

public class NPCInteract : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public Text characterNameText;
    public Text dialogueText;
    public Image characterImage;
    public Button continueButton;
    public Button interactButton;
    public TMP_Text interactButtonText;

    [Header("Talk State")]
    public bool setHasTalkedMarie = false;
    public bool setHasTalkedSimon = false;
    public bool setHasTalkedJohnuelle = false;
    public bool setHasTalkedJam = false;
    public bool setHasTalkedBien = false;

    public static bool hasTalkedMarie = false;
    public static bool hasTalkedSimon = false;
    public static bool hasTalkedJohnuelle = false;
    public static bool hasTalkedJam = false;
    public static bool hasTalkedBien = false;

    [Header("NPC Settings")]
    public string interactLabel = "Talk";
    public NPCDialogueEntry[] dialogueEntries;

    public float typingSpeed = 0.03f;
    public PlayerController playerMovementScript;
    public JoystickController joystick;

    private static NPCInteract activeNPC = null;
    private static List<NPCInteract> nearbyNPCs = new List<NPCInteract>();

    private int entryIndex = 0;
    private int lineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        interactButton.gameObject.SetActive(false);

        // FIXED: removed RemoveAllListeners()
        interactButton.onClick.AddListener(OpenNearestDialogue);
    }

    private void OnDestroy()
    {
        nearbyNPCs.Remove(this);

        if (interactButton != null)
            interactButton.onClick.RemoveListener(OpenNearestDialogue);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (interactButton == null || dialoguePanel == null) return;

        if (!nearbyNPCs.Contains(this))
            nearbyNPCs.Add(this);

        RefreshInteractButton();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        nearbyNPCs.Remove(this);

        if (interactButton == null || dialoguePanel == null) return;

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

        NPCInteract nearest = GetNearestNPC();

        if (nearest != null)
            nearest.OpenDialogue();
    }

    NPCInteract GetNearestNPC()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return null;

        float closest = Mathf.Infinity;
        NPCInteract nearest = null;

        foreach (NPCInteract npc in nearbyNPCs)
        {
            float dist = Vector2.Distance(player.transform.position, npc.transform.position);

            if (dist < closest)
            {
                closest = dist;
                nearest = npc;
            }
        }

        return nearest;
    }

    void RefreshInteractButton()
    {
        if (interactButton == null || dialoguePanel == null) return;

        bool hasNearby = nearbyNPCs.Count > 0 && !dialoguePanel.activeSelf;

        interactButton.gameObject.SetActive(hasNearby);

        if (hasNearby)
        {
            NPCInteract nearest = GetNearestNPC();

            if (nearest != null && interactButtonText != null)
                interactButtonText.text = nearest.interactLabel;
        }
    }

    void OpenDialogue()
    {
        if (activeNPC != null && activeNPC != this) return;

        activeNPC = this;

        entryIndex = 0;
        lineIndex = 0;

        if (joystick != null)
            joystick.ForceReset();

        interactButton.gameObject.SetActive(false);
        dialoguePanel.SetActive(true);

        continueButton.onClick.RemoveListener(OnContinuePressed);
        continueButton.onClick.AddListener(OnContinuePressed);

        LockPlayer();
        ShowCurrentEntry();
    }

    void ShowCurrentEntry()
    {
        if (entryIndex >= dialogueEntries.Length)
        {
            CloseDialogue();
            return;
        }

        NPCDialogueEntry entry = dialogueEntries[entryIndex];

        characterNameText.text = entry.characterName;

        if (characterImage != null)
        {
            characterImage.sprite = entry.characterImage;
            characterImage.preserveAspect = true;
        }

        StartTyping(entry.lines[lineIndex]);
    }

    void StartTyping(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        yield return new WaitForEndOfFrame();
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
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            dialogueText.text = dialogueEntries[entryIndex].lines[lineIndex];
            isTyping = false;
            return;
        }

        lineIndex++;

        if (lineIndex < dialogueEntries[entryIndex].lines.Length)
        {
            StartTyping(dialogueEntries[entryIndex].lines[lineIndex]);
            return;
        }

        entryIndex++;
        lineIndex = 0;

        if (entryIndex < dialogueEntries.Length)
        {
            ShowCurrentEntry();
        }
        else
        {
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (setHasTalkedMarie) hasTalkedMarie = true;
        if (setHasTalkedSimon) hasTalkedSimon = true;
        if (setHasTalkedJohnuelle) hasTalkedJohnuelle = true;
        if (setHasTalkedJam) hasTalkedJam = true;
        if (setHasTalkedBien) hasTalkedBien = true;

        continueButton.onClick.RemoveListener(OnContinuePressed);

        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        isTyping = false;
        activeNPC = null;

        UnlockPlayer();
        RefreshInteractButton();
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