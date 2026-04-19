using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EnterMainMenu : MonoBehaviour, IPointerDownHandler
{
    public string mainMenuSceneName = "MainMenu";
    public float loadDelay = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (loadDelay <= 0f)
            SceneManager.LoadScene(mainMenuSceneName);
        else
            Invoke(nameof(LoadScene), loadDelay);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
