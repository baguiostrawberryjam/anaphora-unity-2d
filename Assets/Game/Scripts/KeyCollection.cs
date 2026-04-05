using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeyCollection : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogueLines;
    private int index;

    public float wordSpeed;

    // state flags
    private bool pickedUp;
    private bool destroyOnClose;

    private Button continueBtnComponent;

    private void Start()
    {
        // auto-wire the UI Button if present so clicks reliably call nextLine
        if (continueButton != null)
        {
            continueBtnComponent = continueButton.GetComponent<Button>();
            if (continueBtnComponent != null)
            {
                continueBtnComponent.onClick.AddListener(nextLine);
            }
            else
            {
                Debug.LogWarning($"{name}: continueButton GameObject has no Button component.");
            }

            // hide continue by default; Typing will show it
            continueButton.SetActive(false);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void nextLine()
    {
        Debug.Log("KeyCollection.nextLine invoked");

        if (continueButton != null) continueButton.SetActive(false);

        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            zeroText();
            return;
        }

        if (index < dialogueLines.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            destroyOnClose = true;
            zeroText();
        }
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (destroyOnClose)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Typing()
    {
        if (dialogueLines == null || dialogueLines.Length == 0) yield break;

        dialogueText.text = "";
        foreach (char letter in dialogueLines[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        // Show the continue button when the line is fully typed
        if (continueButton != null)
            continueButton.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pickedUp) return;

        GameObject hitRoot = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;

        if (!hitRoot.CompareTag("Player")) return;

        pickedUp = true;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;
        else
        {
            var renderer = GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;
        }

        PlayerController player = hitRoot.GetComponent<PlayerController>() ?? collision.GetComponentInParent<PlayerController>();
        if (player != null) player.isKeyCollected = true;

        if (dialogueLines != null && dialogueLines.Length > 0 && dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "";
            index = 0;
            StartCoroutine(Typing());
        }
        else
        {
            Destroy(gameObject);
        }
    }
}