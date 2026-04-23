using UnityEngine;
using TMPro; // Make sure you have TextMeshPro imported in your project

public class DialogueController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip nextLineSound;

    [Tooltip("Minimum pitch for the Animal Crossing effect")]
    public float minPitch = 0.9f;
    [Tooltip("Maximum pitch for the Animal Crossing effect")]
    public float maxPitch = 1.1f;

    private string[] currentDialogueLines;
    private int currentLineIndex = 0;

    // Call this from another script to begin a dialogue sequence
    public void StartDialogue(string[] lines)
    {
        currentDialogueLines = lines;
        currentLineIndex = 0;

        ShowCurrentLine();
    }

    // Hook this up to your "Next" UI Button's OnClick() event in the Inspector
    public void OnNextButtonClicked()
    {
        currentLineIndex++;

        // Check if we still have lines left to show
        if (currentLineIndex < currentDialogueLines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            // Dialogue is over, hide the panel
            gameObject.SetActive(false);
        }
    }

    private void ShowCurrentLine()
    {
        dialogueText.text = currentDialogueLines[currentLineIndex];

        if (audioSource != null && nextLineSound != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);

            audioSource.PlayOneShot(nextLineSound);
        }
    }
}