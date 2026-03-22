using System.Collections;
using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    public Transform target;
    public DoorTeleport pairedDoor;
    public Vector2 exitDirection = Vector2.up;
    public float exitOffset = 0.4f;
    public float cooldownSeconds = 0.15f;

    //Simple Transition during Teleportation
    public GameObject blackPanel;
    public float fadeDuration = 0.2f;

    bool isOnCooldown;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;
        if (!other.CompareTag("Player")) return;
        if (target == null) return;

        if (blackPanel != null)
        {
            StartCoroutine(TeleportWithBlack(other));
        }
    }
    private IEnumerator TeleportWithBlack(Collider2D other)
    {
        blackPanel.SetActive(true);
        yield return null;
        DoTeleport(other);
        yield return new WaitForSeconds(fadeDuration);
        blackPanel.SetActive(false);
    }

    private void DoTeleport(Collider2D other)
    {
        Vector2 dir = exitDirection.sqrMagnitude > 0.0001f ? exitDirection.normalized : Vector2.zero;
        Vector3 destination = target.position + (Vector3)(dir * exitOffset);
        destination.z = other.transform.position.z; 

        var playerRb = other.attachedRigidbody;

        if (playerRb != null)
        {
            playerRb.position = new Vector2(destination.x, destination.y);
            playerRb.linearVelocity = Vector2.zero;
        }
        else
        {
            other.transform.position = destination;
        }

        var animator = other.GetComponent<Animator>();
        if (animator != null && dir != Vector2.zero)
        {
            animator.SetFloat("FaceX", dir.x);
            animator.SetFloat("FaceY", dir.y);
            animator.SetFloat("Speed", 0f);
        }

        StartCoroutine(StartCooldown(cooldownSeconds));
        if (pairedDoor != null)
        {
            pairedDoor.StartCoroutine(pairedDoor.StartCooldown(cooldownSeconds));
        }
    }

    IEnumerator StartCooldown(float seconds)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(seconds);
        isOnCooldown = false;
    }
}