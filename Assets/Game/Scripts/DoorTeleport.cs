using System.Collections;
using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    [Tooltip("Where the player should appear when using this door.")]
    public Transform target;

    [Tooltip("Optional: the matching door on the other side so it won't immediately teleport back.")]
    public DoorTeleport pairedDoor;

    [Tooltip("Direction the player should face after teleport. Use (0,1) for up, (0,-1) for down, (1,0) right, (-1,0) left.")]
    public Vector2 exitDirection = Vector2.up;

    [Tooltip("How far from the target position the player will be placed along exitDirection to avoid getting stuck.")]
    public float exitOffset = 0.4f;

    [Tooltip("Seconds to ignore triggers after a teleport (prevents immediate re-trigger).")]
    public float cooldownSeconds = 0.15f;

    bool isOnCooldown;

    private void Start()
    {
        var col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogWarning($"{name}: DoorTeleport requires a Collider2D (e.g. BoxCollider2D).");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"{name}: Collider2D should be set to Is Trigger for DoorTeleport to work properly.");
        }

        if (target == null)
        {
            Debug.LogWarning($"{name}: target Transform is not set on DoorTeleport.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;
        if (!other.CompareTag("Player")) return;
        if (target == null) return;

        // Compute normalized direction and destination with offset so the player is placed outside the door collider
        Vector2 dir = exitDirection.sqrMagnitude > 0.0001f ? exitDirection.normalized : Vector2.zero;
        Vector3 destination = target.position + (Vector3)(dir * exitOffset);
        destination.z = other.transform.position.z; // preserve original Z

        var playerRb = other.attachedRigidbody;

        if (playerRb != null)
        {
            // Move Rigidbody2D (recommended) and zero velocity to prevent physics drift
            playerRb.position = new Vector2(destination.x, destination.y);
            playerRb.linearVelocity = Vector2.zero;
        }
        else
        {
            other.transform.position = destination;
        }

        // Update animator facing so player's idle face points in the right direction
        var animator = other.GetComponent<Animator>();
        if (animator != null && dir != Vector2.zero)
        {
            animator.SetFloat("FaceX", dir.x);
            animator.SetFloat("FaceY", dir.y);
            animator.SetFloat("Speed", 0f);
        }

        // Start cooldown on both doors to avoid immediate back-and-forth
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