using UnityEngine;
using UnityEngine.SceneManagement;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

public class MainMenu : MonoBehaviour
{
    #if UNITY_EDITOR
        public SceneAsset gameSceneAsset;
    #endif

    private string gameSceneName;
    private string spawnPointName = "DefaultSpawn";

    void OnValidate()
    {
    #if UNITY_EDITOR
            if (gameSceneAsset != null)
            {
                gameSceneName = gameSceneAsset.name;
            }
    #endif
    }

    public void StartGame()
    {
        PlayerPrefs.SetString("SpawnPoint", spawnPointName);
        PlayerPrefs.Save();

        Debug.Log($"Starting game, loading scene '{gameSceneName}', spawn point '{spawnPointName}'");

        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Quitting game...");

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }
}