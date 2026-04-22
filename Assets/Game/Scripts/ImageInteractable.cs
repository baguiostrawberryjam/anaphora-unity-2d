using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ImageInteractable : MonoBehaviour
{
    [Header("Dialogue Panel")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public Button continueButton;

    [Header("Zoom Image Panel")]
    public GameObject zoomImagePanel;
    public Image zoomImage;

    [Header("Interact Button")]
    public Button interactButton;
    public TMP_Text interactButtonText;

    [Header("Notice Board Settings")]
    public string interactLabel = "Read";
    public Sprite noticeBoardSprite;

    [TextArea(2, 5)]
    public string[] dialogues;

    public float typingSpeed = 0.03f;

    public PlayerController playerMovementScript;
    public JoystickController joystick;

    [Header("Player Bools To Set")]
    public bool setHasCheckedBulletinBoard = false;

    private static List<ImageInteractable> nearbyObjects = new List<ImageInteractable>();

    private bool isTyping = false;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;

    private void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (zoomImagePanel != null) zoomImagePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        nearbyObjects.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!nearbyObjects.Contains(this))
            nearbyObjects.Add(this);

        // Claim the shared interact button for ImageInteractable while player is nearby
        if (interactButton != null)
        {
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(OpenNearestObject);
        }

        RefreshInteractButton();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        nearbyObjects.Remove(this);

        // Release the listener so NPCInteract can re-register when needed
        if (interactButton != null && nearbyObjects.Count == 0)
            interactButton.onClick.RemoveAllListeners();

        RefreshInteractButton();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            OpenNearestObject();
    }

    void OpenNearestObject()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf) return;

        ImageInteractable nearest = GetNearestObject();

        if (nearest != null)
            nearest.OpenObject();
    }

    ImageInteractable GetNearestObject()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return null;

        float closest = Mathf.Infinity;
        ImageInteractable nearest = null;

        foreach (ImageInteractable obj in nearbyObjects)
        {
            float dist = Vector2.Distance(player.transform.position, obj.transform.position);

            if (dist < closest)
            {
                closest = dist;
                nearest = obj;
            }
        }

        return nearest;
    }

    void RefreshInteractButton()
    {
        if (interactButton == null) return;

        bool hasNearby = nearbyObjects.Count > 0 && (dialoguePanel == null || !dialoguePanel.activeSelf);
        interactButton.gameObject.SetActive(hasNearby);

        if (hasNearby)
        {
            ImageInteractable nearest = GetNearestObject();
            if (nearest != null && interactButtonText != null)
                interactButtonText.text = nearest.interactLabel;
        }
    }

    void SetPlayerBools()
    {
        if (setHasCheckedBulletinBoard)
            PlayerController.hasCheckedBulletinBoard = true;
    }

    void OpenObject()
    {
        currentIndex = 0;

        if (joystick != null)
            joystick.ForceReset();

        if (interactButton != null)
            interactButton.gameObject.SetActive(false);

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (zoomImagePanel != null) zoomImagePanel.SetActive(true);

        if (zoomImage != null)
        {
            zoomImage.sprite = noticeBoardSprite;
            zoomImage.preserveAspect = true;
        }

        if (playerMovementScript != null)
            playerMovementScript.canMove = false;

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinuePressed);
        }

        SetPlayerBools();

        if (dialogues != null && dialogues.Length > 0)
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
        if (dialogueText != null) dialogueText.text = "";

        foreach (char c in line)
        {
            if (dialogueText != null) dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
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

            if (dialogueText != null && dialogues != null)
                dialogueText.text = dialogues[currentIndex];

            isTyping = false;
            return;
        }

        currentIndex++;

        if (dialogues != null && currentIndex < dialogues.Length)
        {
            StartTyping(dialogues[currentIndex]);
        }
        else
        {
            CloseObject();
        }
    }

    void CloseObject()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (zoomImagePanel != null) zoomImagePanel.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

        isTyping = false;

        if (playerMovementScript != null)
            playerMovementScript.canMove = true;

        RefreshInteractButton();
    }
}