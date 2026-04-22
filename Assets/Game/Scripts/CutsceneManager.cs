using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR 
using UnityEditor;
#endif

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cutsceneCanvas;
    public CanvasGroup cutsceneCanvasGroup;
    public VideoPlayer videoPlayer;

    [Header("Settings")]
    public float fadeDuration = 1.5f;

    [Header("Scene Transition")]
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif
    [SerializeField, HideInInspector]
    private string sceneToLoad;

    private System.Action onFinished;
    private bool videoEnded;

    void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
            sceneToLoad = sceneAsset.name;
#endif
    }

    private void Awake()
    {
        // 1. Hide the canvas when the game starts
        if (cutsceneCanvas != null)
            cutsceneCanvas.SetActive(false);

        if (cutsceneCanvasGroup != null)
            cutsceneCanvasGroup.alpha = 0f;
    }

    public void PlayCutscene(System.Action callback)
    {
        onFinished = callback;
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        cutsceneCanvas.SetActive(true);
        if (cutsceneCanvasGroup != null)
            cutsceneCanvasGroup.alpha = 0f;

        // Ensure the video player is active 
        videoPlayer.gameObject.SetActive(true);
        videoEnded = false;

        videoPlayer.Stop();
        videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.loopPointReached += OnVideoEnded;

        // 2. Play the video
        videoPlayer.Play();

        // 3. Fade in
        if (cutsceneCanvasGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                cutsceneCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                yield return null;
            }
            cutsceneCanvasGroup.alpha = 1f;
        }

        // 4. Wait for the video to finish playing
        yield return new WaitUntil(() => videoEnded || !videoPlayer.isPlaying);

        // 5. Clean up
        videoPlayer.loopPointReached -= OnVideoEnded;
        onFinished?.Invoke();

        // 6. Load the next scene immediately!
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene to load is empty! Please assign a SceneAsset in the CutsceneManager inspector.");
        }
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        videoEnded = true;
    }
}