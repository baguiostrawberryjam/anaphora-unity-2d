using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Button continueButton;
    public Text dialogueText;

    public string dialogue = "";

    public float delayBeforeShow = 1f;
    public float typingSpeed = 0.03f;

    public bool useAutoTrigger = true;
    private bool hasTriggered = false;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(CloseDialogue);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useAutoTrigger) return;
        if (!other.CompareTag("Player")) return;

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
    }
}