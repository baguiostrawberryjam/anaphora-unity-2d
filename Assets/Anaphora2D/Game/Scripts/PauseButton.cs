using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [Header("References")]
    public GameObject pausePanel;
    public Button pauseButton;
    public GameObject pauseButton1;
    public Button continueButton;
    public Button returnToMenuButton;
    public Button exitButton;

    public PlayerController playerController;
    public JoystickController joystick;

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        if (continueButton != null)
            continueButton.onClick.AddListener(Resume);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMenu);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (pauseButton != null)
            pauseButton.gameObject.SetActive(false);

        if (pauseButton1 != null)
            pauseButton1.SetActive(false);

        if (playerController != null)
            playerController.canMove = false;

        if (joystick != null)
            joystick.ForceReset();

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.gameObject.SetActive(true);

        if (pauseButton1 != null)
            pauseButton1.SetActive(true);

        if (playerController != null)
            playerController.canMove = true;

        Time.timeScale = 1f;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;

        if (SaveSystem.instance != null)
        {
            SaveSystem.instance.SaveGame();
            Debug.Log("Game saved before returning to main menu.");
        }
        else
        {
            Debug.LogWarning("SaveSystem instance not found! Could not save.");
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}