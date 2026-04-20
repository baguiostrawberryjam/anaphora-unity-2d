using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Teleport : MonoBehaviour
{

#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    private string sceneToLoad;

    [Tooltip("The name of the spawn point GameObject in the TARGET scene where the player should appear")]
    public string spawnPointName;

    public float teleportDelay = 1.5f;

    private bool isTeleporting = false;

    void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            sceneToLoad = sceneAsset.name;
        }
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTeleporting && other.CompareTag("Player"))
        {
            isTeleporting = true;

            PlayerPrefs.SetString("SpawnPoint", spawnPointName);
            PlayerPrefs.Save();

            Debug.Log($"Teleporting to scene '{sceneToLoad}', spawn point '{spawnPointName}'");

            Invoke(nameof(LoadScene), teleportDelay);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}