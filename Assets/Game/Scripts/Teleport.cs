using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    public string sceneToLoad;

    [Tooltip("The name of the spawn point GameObject in the TARGET scene where the player should appear")]
    public string spawnPointName;

    public float teleportDelay = 1.5f;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTeleporting && other.CompareTag("Player"))
        {
            isTeleporting = true;
            PlayerPrefs.SetString("SpawnPoint", spawnPointName);
            PlayerPrefs.Save(); // Ensure it's written immediately
            Debug.Log($"Teleporting to scene '{sceneToLoad}', spawn point '{spawnPointName}'");
            Invoke(nameof(LoadScene), teleportDelay);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}