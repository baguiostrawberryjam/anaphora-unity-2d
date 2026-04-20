using UnityEngine;
using System.Collections;

public class SplashFade : MonoBehaviour
{
    public float displayTime = 3f;
    public float fadeDuration = 1.5f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        // Stay visible
        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;

        // Fade out
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1 - (elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}