using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Management")]
#if UNITY_EDITOR
    public UnityEditor.SceneAsset gameSceneAsset;
#endif
    private string gameSceneName;
    private string spawnPointName = "DefaultSpawn";

    [Header("UI & Audio Sources")]
    public CanvasGroup fadeGroup;
    public AudioSource bgmSource;  // Your looping music speaker
    public AudioSource sfxSource;  // Your sound effect speaker

    [Header("Sound Effect Clips")]
    public AudioClip genericClickSound; // The quiet tick when clicking anywhere
    public AudioClip startGameSound;    // The heavy thud when clicking New/Continue

    public float transitionDuration = 1.5f;

    private bool isTransitioning = false;

    void OnValidate()
    {
#if UNITY_EDITOR
        if (gameSceneAsset != null)
        {
            gameSceneName = gameSceneAsset.name;
        }
#endif
    }

    void Update()
    {
        // 1. GLOBAL CLICK DETECTOR
        bool clicked = false;

#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            clicked = true;
#else
        if (Input.GetMouseButtonDown(0))
            clicked = true;
#endif

        // 2. Play the generic click sound (only if we aren't transitioning to the game)
        if (clicked && sfxSource != null && genericClickSound != null && !isTransitioning)
        {
            sfxSource.PlayOneShot(genericClickSound);
        }
    }

    public void StartGame()
    {
        if (isTransitioning) return;

        PlayerPrefs.SetString("SpawnPoint", spawnPointName);
        PlayerPrefs.Save();

        StartCoroutine(TransitionToGame());
    }

    public void OpenSettings()
    {
        if (isTransitioning) return;
        Debug.Log("Opening Settings...");
    }

    private IEnumerator TransitionToGame()
    {
        isTransitioning = true;

        // Variable to remember how long our sound effect is
        float sfxLength = 0f;

        // 1. Play the massive "Start Game" sound
        if (sfxSource != null && startGameSound != null)
        {
            sfxSource.PlayOneShot(startGameSound);
            sfxLength = startGameSound.length; // Grab the exact duration of the audio clip in seconds
        }

        // 2. Lock the screen
        if (fadeGroup != null)
        {
            fadeGroup.blocksRaycasts = true;
        }

        float elapsedTime = 0f;
        float startVolume = (bgmSource != null) ? bgmSource.volume : 0f;

        // 3. Fade out the looping BGM and fade in the black screen simultaneously
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float timeRatio = elapsedTime / transitionDuration;

            if (fadeGroup != null)
            {
                fadeGroup.alpha = timeRatio;
            }

            if (bgmSource != null)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, timeRatio);
            }

            yield return null;
        }

        // 4. THE FIX: If the sound effect is longer than the visual fade, wait for it to finish!
        if (sfxLength > transitionDuration)
        {
            float remainingTime = sfxLength - transitionDuration;
            yield return new WaitForSeconds(remainingTime);
        }

        // 5. Sound is done, screen is black. Load the game!
        Debug.Log($"Starting game, loading scene '{gameSceneName}', spawn point '{spawnPointName}'");
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        if (isTransitioning) return;

        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}