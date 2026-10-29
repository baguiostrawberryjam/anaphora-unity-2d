using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIGlitchEffect : MonoBehaviour
{
    [Header("Glitch Timing")]
    [Tooltip("Minimum seconds between glitch bursts.")]
    public float minWaitTime = 2f;
    [Tooltip("Maximum seconds between glitch bursts.")]
    public float maxWaitTime = 6f;
    [Tooltip("How long the actual glitch lasts when it happens.")]
    public float glitchDuration = 0.25f;

    [Header("Artifact Intensity")]
    [Tooltip("How far the object tears horizontally (in pixels).")]
    public float maxHorizontalTear = 25f;
    [Tooltip("Frames per second of the glitch (lower = chunkier).")]
    public int glitchFramerate = 12;
    public bool flashAlpha = true;

    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private float chunkyFrameTime;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = transform.localPosition;
        chunkyFrameTime = 1f / glitchFramerate;

        StartCoroutine(GlitchRoutine());
    }

    private IEnumerator GlitchRoutine()
    {
        while (true)
        {
            // 1. Wait peacefully for a random amount of time
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 2. The Glitch triggers!
            float timer = 0f;
            while (timer < glitchDuration)
            {
                // Randomly shift the X position to simulate a digital tear
                // Mathf.Round ensures the tear snaps to whole pixels for that retro look
                float tearAmount = Mathf.Round(Random.Range(-maxHorizontalTear, maxHorizontalTear));
                transform.localPosition = originalPosition + new Vector3(tearAmount, 0, 0);

                // Randomly drop the alpha so it looks like it's failing to render
                if (flashAlpha)
                {
                    canvasGroup.alpha = Random.Range(0.1f, 1f);
                }

                timer += chunkyFrameTime;

                // Wait for our chunky frame duration before twitching again
                yield return new WaitForSeconds(chunkyFrameTime);
            }

            // 3. Glitch over, snap exactly back to normal
            transform.localPosition = originalPosition;
            canvasGroup.alpha = 1f;
        }
    }
}