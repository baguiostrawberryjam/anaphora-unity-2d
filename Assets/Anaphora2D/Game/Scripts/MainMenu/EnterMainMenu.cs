using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EnterMainMenu : MonoBehaviour, IPointerDownHandler
{
    public string mainMenuSceneName = "MainMenu";
    public CanvasGroup fadeGroup; // We will plug your new panel into this
    public float fadeDuration = 1.5f;

    private bool isTransitioning = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        // If we are already fading, ignore extra clicks
        if (isTransitioning) return;

        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        isTransitioning = true;
        float elapsedTime = 0f;

        // Make the fade panel block clicks so the user can't click anything else while fading
        fadeGroup.blocksRaycasts = true;

        // Gradually increase the alpha from 0 (transparent) to 1 (solid black)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}