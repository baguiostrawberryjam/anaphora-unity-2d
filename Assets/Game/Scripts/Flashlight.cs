using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    private bool isPlayerNearby;
    public Button interactButton;
    private bool isInteractVisible;

    private void Start()
    {
        if (interactButton == null)
        {
            Debug.LogWarning($"{name}: interactButton reference is null.");
        }
        else
        {
            interactButton.onClick.AddListener(OnInteractButtonPressed);

            interactButton.gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
            interactButton.gameObject.SetActive(false);

            isInteractVisible = false;
        }
    }

    private void OnDestroy()
    {
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(OnInteractButtonPressed);
        }
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

        Debug.Log("Interact Button should appear here");
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

        Debug.Log("Interact Button should disappear here");
    }

    private void Update()
    {
        if (!isPlayerNearby) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            PickUp();
        }
    }

    private void OnInteractButtonPressed()
    {
        if (!isPlayerNearby) return;
        PickUp();
    }

    private void PickUp()
    {
        Debug.Log("Flashlight picked up successfully!");

        if (interactButton != null && isInteractVisible)
        {
            interactButton.gameObject.SetActive(false);
            isInteractVisible = false;
        }

        Destroy(gameObject);
    }
}