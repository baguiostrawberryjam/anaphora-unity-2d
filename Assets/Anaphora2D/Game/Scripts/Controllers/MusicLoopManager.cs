using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicLoopManager : MonoBehaviour
{
    [Header("Music Tracks")]
    [Tooltip("This track will always play first.")]
    public AudioClip trackA;
    [Tooltip("This track will play second.")]
    public AudioClip trackB;

    [Header("Settings")]
    [Tooltip("Seconds of silence between each track.")]
    public float silenceDuration = 15f;
    public bool playOnAwake = true;

    private AudioSource audioSource;

    private void Awake()
    {
        // Grab the AudioSource on this GameObject automatically
        audioSource = GetComponent<AudioSource>();

        // We disable the built-in loop and playOnAwake so our script can control the exact timing
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (playOnAwake)
        {
            StartCoroutine(PlayMusicSequence());
        }
    }

    // You can call this from another script if you ever want to start the loop manually
    public void StartMusicLoop()
    {
        StopAllCoroutines(); // Prevents multiple sequences from running at the same time
        StartCoroutine(PlayMusicSequence());
    }

    private IEnumerator PlayMusicSequence()
    {
        // We use a boolean to keep track of whose turn it is. 
        // True = Track A's turn. False = Track B's turn.
        bool isTrackATurn = true;

        // This creates an infinite loop that runs as long as this GameObject is active
        while (true)
        {
            // Pick the correct song based on whose turn it is
            AudioClip currentSong = isTrackATurn ? trackA : trackB;

            if (currentSong != null)
            {
                // Assign the clip and play it
                audioSource.clip = currentSong;
                audioSource.Play();

                // Tell the Coroutine to pause right here until the song finishes playing
                yield return new WaitForSeconds(currentSong.length);
            }
            else
            {
                Debug.LogWarning("A music track is missing in the MusicLoopManager!");
                yield return null; // Failsafe to prevent Unity from freezing if a track is unassigned
            }

            // The song has finished! Now, wait for the 15 seconds of silence
            yield return new WaitForSeconds(silenceDuration);

            // Flip the boolean so the OTHER track plays on the next loop
            isTrackATurn = !isTrackATurn;
        }
    }
}