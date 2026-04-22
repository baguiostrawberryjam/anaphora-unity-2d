using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cutsceneCanvas;
    public VideoPlayer videoPlayer;

    [Header("Cutscene Text")]
    public Text cutsceneText; 

    private System.Action onFinished;
    private bool videoEnded;

    public void PlayCutscene(System.Action callback)
    {
        onFinished = callback;
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        cutsceneCanvas.SetActive(true);

        videoEnded = false;

        videoPlayer.Stop();
        videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.loopPointReached += OnVideoEnded;

        videoPlayer.Play();
        StartCoroutine(PlayCutsceneText());

        IEnumerator PlayCutsceneText()
        {
            if (cutsceneText == null) yield break;

            cutsceneText.gameObject.SetActive(true);

            // Line 1
            cutsceneText.text = "You have survived an encounter with the entity..";
            yield return new WaitForSeconds(3f);

            // Line 2
            cutsceneText.text = "You win...";
            yield return new WaitForSeconds(2f);

            // Line 3
            cutsceneText.text = "FOR NOW!";
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitUntil(() =>
            videoEnded || !videoPlayer.isPlaying
        );

        videoPlayer.loopPointReached -= OnVideoEnded;

        if (cutsceneText != null)
            cutsceneText.gameObject.SetActive(false);

        cutsceneCanvas.SetActive(false);

        onFinished?.Invoke();
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        videoEnded = true;
    }
}