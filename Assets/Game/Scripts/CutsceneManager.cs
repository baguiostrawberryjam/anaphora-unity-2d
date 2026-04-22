using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cutsceneCanvas;
    public VideoPlayer videoPlayer;

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
        videoPlayer.loopPointReached += OnVideoEnded;

        videoPlayer.Stop();
        videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.Play();

        yield return new WaitUntil(() => videoEnded);

        videoPlayer.loopPointReached -= OnVideoEnded;
        cutsceneCanvas.SetActive(false);

        onFinished?.Invoke();
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        videoEnded = true;
    }
}