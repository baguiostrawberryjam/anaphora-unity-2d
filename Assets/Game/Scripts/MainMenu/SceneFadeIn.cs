using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SceneFadeIn : MonoBehaviour
{
    [Tooltip("Should match the fadeDuration in your EnterMainMenu script.")]
    public float fadeDuration = 1.5f;

    private CanvasGroup fadeGroup;

    void Start()
    {
        fadeGroup = GetComponent<CanvasGroup>();

        // Ensure the panel starts completely black and blocks clicks
        fadeGroup.alpha = 1f;
        fadeGroup.blocksRaycasts = true;

        // Start the fade transition immediately when the scene loads
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // 1f minus the progress reverses the math, going from 1 down to 0
            fadeGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        // Failsafe: Ensure it ends exactly at 0 and turns off the invisible shield
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }
}