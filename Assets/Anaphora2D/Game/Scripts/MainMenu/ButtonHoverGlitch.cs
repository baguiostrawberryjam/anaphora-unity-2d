using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverGlitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("What to Glitch")]
    [Tooltip("Drag the child Text object here")]
    public RectTransform textToGlitch;

    [Header("Glitch Settings")]
    public float maxHorizontalTear = 10f; // Max pixels it jumps left/right
    public float maxVerticalTear = 2f;    // Max pixels it jumps up/down
    public int glitchFramerate = 15;      // Lower = chunkier retro feel

    private Vector3 originalPosition;
    private Coroutine glitchRoutine;

    void Start()
    {
        // Remember exactly where the text is supposed to sit
        if (textToGlitch != null)
        {
            originalPosition = textToGlitch.localPosition;
        }
    }

    // Triggered the exact frame the mouse enters the invisible hitbox
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textToGlitch != null && glitchRoutine == null)
        {
            glitchRoutine = StartCoroutine(DoGlitch());
        }
    }

    // Triggered the exact frame the mouse leaves the invisible hitbox
    public void OnPointerExit(PointerEventData eventData)
    {
        StopGlitching();
    }

    // Failsafe: If the menu changes scenes while you are hovering, reset it
    void OnDisable()
    {
        StopGlitching();
    }

    private void StopGlitching()
    {
        if (glitchRoutine != null)
        {
            StopCoroutine(glitchRoutine);
            glitchRoutine = null;
        }

        // Immediately snap the text perfectly back to center
        if (textToGlitch != null)
        {
            textToGlitch.localPosition = originalPosition;
        }
    }

    private IEnumerator DoGlitch()
    {
        float chunkyFrameTime = 1f / glitchFramerate;

        while (true)
        {
            // Calculate a random violent shift
            float xOffset = Mathf.Round(Random.Range(-maxHorizontalTear, maxHorizontalTear));
            float yOffset = Mathf.Round(Random.Range(-maxVerticalTear, maxVerticalTear));

            // Apply it to the text
            textToGlitch.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);

            // Wait for our chunky retro framerate before twitching again
            yield return new WaitForSeconds(chunkyFrameTime);
        }
    }
}