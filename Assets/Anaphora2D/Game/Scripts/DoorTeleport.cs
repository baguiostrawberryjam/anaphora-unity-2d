using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public DoorTeleport linkedDoor;
    public Vector2 spawnOffset = new Vector2(0f, 1f);
    public float cooldown = 1.5f;

    [Header("Conditions (leave unchecked to ignore)")]
    public bool requireFlashlight = false;
    public bool requireInteractedSwitch = false;
    public bool requireKey = false;
    public bool requiredInteractedDrawer = false;
    public bool requiredInteractedRef = false;
    public bool requireCheckedBulletinBoard = false;
    public bool requireTalkedToBien = false;
    public bool requireTalkedToJam = false;

    [Header("Blocked Dialogue")]
    public DialogueTrigger blockedDialogueTrigger;

    [Header("Warning Collider (optional)")]
    public Collider2D warningCollider;

    [Header("Session Trigger")]
    public bool triggerOncePerSession = false;
    public string triggerID = "";

    public static HashSet<string> triggeredDoors = new HashSet<string>();

    private bool isOnCooldown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isOnCooldown) return;

        if (triggerOncePerSession &&
            !string.IsNullOrEmpty(triggerID) &&
            triggeredDoors.Contains(triggerID))
        {
            return;
        }

        if (linkedDoor == null)
        {
            Debug.LogWarning($"[DoorTeleport] '{gameObject.name}' has no Linked Door assigned!", this);
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();

        if (!MeetsConditions(player))
        {
            if (blockedDialogueTrigger != null)
                blockedDialogueTrigger.TriggerDialogue();
            return;
        }

        if (triggerOncePerSession && !string.IsNullOrEmpty(triggerID))
            triggeredDoors.Add(triggerID);

        TeleportPlayer(other.gameObject);
    }

    private bool MeetsConditions(PlayerController player)
    {
        if (player == null) return false;

        Debug.Log($"hasFlashlight: {PlayerController.hasFlashlight} | hasInteractedSwitch: {PlayerController.hasInteractedSwitch}");

        if (requireFlashlight && !PlayerController.hasFlashlight) return false;
        if (requireInteractedSwitch && !PlayerController.hasInteractedSwitch) return false;
        if (requireKey && !PlayerController.hasKey) return false;
        if (requiredInteractedDrawer && !PlayerController.hasInteractedDrawer) return false;
        if (requiredInteractedRef && !PlayerController.hasInteractedRef) return false;
        if (requireTalkedToBien && !NPCInteract.hasTalkedBien) return false;
        if (requireTalkedToJam && !NPCInteract.hasTalkedJam) return false;

        return true;
    }

    private void TeleportPlayer(GameObject playerObj)
    {
        Vector2 destination = (Vector2)linkedDoor.transform.position + spawnOffset;

        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = destination;
        }
        else
        {
            playerObj.transform.position = new Vector3(destination.x, destination.y, playerObj.transform.position.z);
        }

        Cinemachine.CinemachineVirtualCamera vcam = Object.FindFirstObjectByType<Cinemachine.CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.OnTargetObjectWarped(playerObj.transform, (Vector3)destination - playerObj.transform.position);
        }

        linkedDoor.StartCooldown();
        StartCooldown();
    }

    public void StartCooldown()
    {
        if (!isOnCooldown)
            StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    // Call this from other scripts to enable/disable the warning collider
    public void SetWarningCollider(bool active)
    {
        if (warningCollider != null)
            warningCollider.enabled = active;
    }

    private void OnDrawGizmosSelected()
    {
        if (linkedDoor == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, linkedDoor.transform.position);
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        Gizmos.color = Color.green;
        Vector3 spawnPoint = linkedDoor.transform.position + new Vector3(spawnOffset.x, spawnOffset.y, 0f);
        Gizmos.DrawWireSphere(spawnPoint, 0.25f);
    }
}