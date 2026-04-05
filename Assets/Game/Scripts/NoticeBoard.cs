using UnityEngine;
using UnityEngine.UI;

public class NoticeBoard : MonoBehaviour
{
    public GameObject noticePanel;
    public GameObject interactPrompt;
    public Text interactPromptText;

    public bool playerInRange;
    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (noticePanel.activeInHierarchy)
            {
                noticePanel.SetActive(false);
                if (interactPrompt != null)
                    interactPrompt.SetActive(true);
            }
            else
            {
                noticePanel.SetActive(true);
                if (interactPrompt != null)
                    interactPrompt.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
            if (noticePanel.activeInHierarchy)
                noticePanel.SetActive(false);
        }
    }

}
