using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Management")]
#if UNITY_EDITOR
    public UnityEditor.SceneAsset gameSceneAsset;
#endif

    [SerializeField, HideInInspector]
    private string gameSceneName;
    private string spawnPointName = "DefaultSpawn";

    [Header("UI & Audio Sources")]
    public CanvasGroup fadeGroup;
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Sound Effect Clips")]
    public AudioClip genericClickSound;
    public AudioClip startGameSound;

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

    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }

    void Update()
    {
        bool clicked = false;

#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            clicked = true;
        else if (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            clicked = true;
#else
        // Check for Mouse OR at least one touch starting this frame
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            clicked = true;
#endif

        if (clicked && sfxSource != null && genericClickSound != null && !isTransitioning)
        {
            sfxSource.PlayOneShot(genericClickSound);
        }
    }

    public void StartGame()
    {
        if (isTransitioning) return;

        // Wipe old save data so the player starts fresh!
        SaveSystem.DeleteSave();

        PlayerPrefs.SetString("SpawnPoint", spawnPointName);
        // One-Time Session Triggers
        PlayerPrefs.SetString("savedDialogueTriggers", string.Join(",", DialogueTrigger.sessionTriggers));
        PlayerPrefs.SetString("savedDoorTeleports", string.Join(",", DoorTeleport.triggeredDoors));
        PlayerPrefs.SetString("savedSceneTeleports", string.Join(",", SceneTeleport.passedConditions));
        PlayerPrefs.Save();

        // Pass the default starting scene
        StartCoroutine(TransitionToGame(gameSceneName));
    }

    public void ContinueGame()
    {
        if (isTransitioning) return;

        // Check if a save actually exists
        if (!PlayerPrefs.HasKey("savedScene") || string.IsNullOrEmpty(PlayerPrefs.GetString("savedScene")))
        {
            Debug.LogWarning("No save data found! Start a new game instead.");
            // Optional: Play an error sound here if you want
            return;
        }

        // Grab the scene the player was last in
        string sceneToLoad = PlayerPrefs.GetString("savedScene");

        // Start the transition into the saved scene
        StartCoroutine(TransitionToGame(sceneToLoad));
    }

    public void OpenSettings()
    {
        if (isTransitioning) return;
        Debug.Log("Opening Settings...");
    }

    // Notice I added a string parameter here so it knows WHICH scene to load
    private IEnumerator TransitionToGame(string targetScene)
    {
        isTransitioning = true;

        float sfxLength = 0f;

        if (sfxSource != null && startGameSound != null)
        {
            sfxSource.PlayOneShot(startGameSound);
            sfxLength = startGameSound.length;
        }

        if (fadeGroup != null)
        {
            fadeGroup.blocksRaycasts = true;
        }

        float elapsedTime = 0f;
        float startVolume = (bgmSource != null) ? bgmSource.volume : 0f;

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

        if (sfxLength > transitionDuration)
        {
            float remainingTime = sfxLength - transitionDuration;
            yield return new WaitForSeconds(remainingTime);
        }

        Debug.Log($"Loading scene '{targetScene}'");
        SceneManager.LoadScene(targetScene);
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