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

    [Header("Conditions (leave unchecked to ignore)")]
    public bool requireFlashlight = false;
    public bool requireInteractedSwitch = false;
    public bool requireKey = false;
    public bool requiredInteractedDrawer = false;
    public bool requiredInteractedRef = false;
    public bool requiredInteractedBulletinBoard = false;

    [Header("Blocked Dialogue")]
    public DialogueTrigger blockedDialogueTrigger;

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
        if (isTeleporting) return;
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (!MeetsConditions(player))
        {
            if (blockedDialogueTrigger != null)
                blockedDialogueTrigger.TriggerDialogue();
            return;
        }

        isTeleporting = true;

        PlayerPrefs.SetString("SpawnPoint", spawnPointName);
        PlayerPrefs.Save();

        Debug.Log($"Teleporting to scene '{sceneToLoad}', spawn point '{spawnPointName}'");

        Invoke(nameof(LoadScene), teleportDelay);
    }

    private bool MeetsConditions(PlayerController player)
    {
        if (player == null) return false;

        if (requireFlashlight && !player.hasFlashlight) return false;
        if (requireInteractedSwitch && !player.hasInteractedSwitch) return false;
        if (requireKey && !player.hasKey) return false;
        if (requiredInteractedDrawer && !player.hasInteractedDrawer) return false;
        if (requiredInteractedRef && !player.hasInteractedRef) return false;
        if (requiredInteractedBulletinBoard && !player.hasCheckedBulletinBoard) return false;

        return true;
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}