using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    public string sceneToLoad;
    public float teleportDelay = 1.5f;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTeleporting && other.CompareTag("Player"))
        {
            isTeleporting = true;

            Debug.Log("Player entered the door! Loading " + sceneToLoad);

            Invoke(nameof(LoadScene), teleportDelay);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}