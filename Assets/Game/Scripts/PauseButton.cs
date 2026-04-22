using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [Header("References")]
    public GameObject pausePanel;
    public Button pauseButton;

    public PlayerController playerController;
    public JoystickController joystick;

    private bool isPaused = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
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

        if (playerController != null)
            playerController.canMove = true;

        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}