using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public GameObject cutsceneCanvas;
    public VideoPlayer videoPlayer;

    private System.Action onFinished;

    public void PlayCutscene(System.Action callback)
    {
        onFinished = callback;
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        cutsceneCanvas.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.Play();

        yield return new WaitUntil(() => videoPlayer.isPlaying);
        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        cutsceneCanvas.SetActive(false);

        onFinished?.Invoke();
    }
}