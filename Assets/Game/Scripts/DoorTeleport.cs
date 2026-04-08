using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    public Transform target;
    public DoorTeleport pairedDoor;
    public Vector2 exitDirection = Vector2.up;
    public float exitOffset = 0.4f;
    public float cooldownSeconds = 0.15f;

    public Animator fadeOutAnimator;
    public Animator fadeInAnimator;

    // Fallback durations if clip detection fails.
    public float fadeOutDuration = 0.2f;
    public float fadeInDuration = 0.2f;

    // Cinemachine virtual camera to temporarily disable during teleport
    public CinemachineCamera cam;

    bool isOnCooldown;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;
        if (!other.CompareTag("Player")) return;
        if (target == null) return;

        // If any animator is assigned, use the animated transition path.
        if (fadeOutAnimator != null || fadeInAnimator != null)
        {
            StartCoroutine(TeleportWithFade(other));
        }
        else
        {
            // Immediate teleport fallback if no animators are assigned.
            DoTeleport(other);
        }
    }

    private IEnumerator TeleportWithFade(Collider2D other)
    {
        // Disable the cine camera so it doesn't try to follow while we teleport
        if (cam != null)
            cam.gameObject.SetActive(false);

        // Play FadeOut (canvas should be visible and animate to black)
        if (fadeOutAnimator != null)
        {
            fadeOutAnimator.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty("FadeOut"))
                fadeOutAnimator.SetTrigger("FadeOut");

            float wait = GetClipLengthOrFallback(fadeOutAnimator, "FadeOut", fadeOutDuration);
            if (wait > 0f)
                yield return new WaitForSeconds(wait);
            else
                yield return null;
        }
        else
        {
            // ensure at least one frame for camera updates if no fadeOut animator
            yield return null;
        }

        DoTeleport(other);

        // Play FadeIn (canvas should animate from black to transparent)
        if (fadeInAnimator != null)
        {
            fadeInAnimator.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty("FadeIn"))
                fadeInAnimator.SetTrigger("FadeIn");

            float wait = GetClipLengthOrFallback(fadeInAnimator, "FadeIn", fadeInDuration);
            if (wait > 0f)
                yield return new WaitForSeconds(wait);
            else
                yield return null;
        }
        else
        {
            yield return null;
        }

        // Optionally hide canvases after animation completes
        if (fadeOutAnimator != null)
            fadeOutAnimator.gameObject.SetActive(false);
        if (fadeInAnimator != null)
            fadeInAnimator.gameObject.SetActive(false);

        // Re-enable the cine camera after teleport
        if (cam != null)
        {
            cam.gameObject.SetActive(true);
            // Optionally wait a frame to let the camera update:
            yield return null;
        }
    }

    private float GetClipLengthOrFallback(Animator animator, string clipNameFragment, float fallback)
    {
        if (animator == null || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(clipNameFragment))
            return fallback;

        var clips = animator.runtimeAnimatorController.animationClips;
        if (clips == null || clips.Length == 0)
            return fallback;

        string fragmentLower = clipNameFragment.ToLowerInvariant();
        foreach (var clip in clips)
        {
            if (clip == null) continue;
            if (clip.name.ToLowerInvariant().Contains(fragmentLower))
            {
                // Respect animator speed (if you modify speed at runtime).
                float speed = Mathf.Approximately(animator.speed, 0f) ? 1f : animator.speed;
                return clip.length / speed;
            }
        }

        // No matching clip found — return fallback.
        return fallback;
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