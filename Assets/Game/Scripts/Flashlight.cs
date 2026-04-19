using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Flashlight : MonoBehaviour
{
    private bool isPlayerNearby;
    private bool isInteractVisible;

    public Button interactButton;

    // Dialogue fields (from DialogueTrigger)
    public GameObject dialoguePanel;
    public Button continueButton;
    public Text dialogueText;

    public string dialogue = "";

    public float delayBeforeShow = 1f;
    public float typingSpeed = 0.03f;

    private bool hasTriggered = false;

    private void Start()
    {
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(OnInteractButtonPressed);
            interactButton.gameObject.SetActive(false);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (interactButton != null)
            interactButton.onClick.RemoveListener(OnInteractButtonPressed);

        if (continueButton != null)
            continueButton.onClick.RemoveListener(CloseDialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        isPlayerNearby = true;

        if (interactButton != null && !isInteractVisible)
        {
            interactButton.gameObject.SetActive(true);
            isInteractVisible = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        isPlayerNearby = false;

        if (interactButton != null && isInteractVisible)
        {
            interactButton.gameObject.SetActive(false);
            isInteractVisible = false;
        }
    }

    private void Update()
    {
        if (!isPlayerNearby) return;

        if (Input.GetKeyDown(KeyCode.F))
            PickUp();
    }

    private void OnInteractButtonPressed()
    {
        if (!isPlayerNearby) return;

        PickUp();
    }

    private void PickUp()
    {
        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(false);
            isInteractVisible = false;
        }
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (continueButton != null)
            continueButton.onClick.AddListener(CloseDialogue);

        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (hasTriggered) return;

        hasTriggered = true;

        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        foreach (char letter in dialogue)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void CloseDialogue()
    {
        StopAllCoroutines();
        dialoguePanel.SetActive(false);

        Destroy(gameObject);
    }
}