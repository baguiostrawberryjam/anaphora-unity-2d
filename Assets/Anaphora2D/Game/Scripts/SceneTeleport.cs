using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTeleport : MonoBehaviour
{
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    [SerializeField, HideInInspector]
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
    public bool requiredCheckedCaseFiles = false;
    public bool requireTalkedMarie = false;
    public bool requireTalkedSimon = false;
    public bool requireTalkedJohnuelle = false;

    [Header("Blocked Dialogue")]
    public DialogueTrigger blockedDialogueTrigger;

    [Header("One-Time Condition Check")]
    public bool checkOnlyOnce = false;
    public string conditionID = "";

    public static HashSet<string> passedConditions = new HashSet<string>();
    private bool isTeleporting = false;

    void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
            sceneToLoad = sceneAsset.name;
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (checkOnlyOnce && !string.IsNullOrEmpty(conditionID) && passedConditions.Contains(conditionID))
        {
            ExecuteTeleport();
            return;
        }

        if (!MeetsConditions(player))
        {
            if (blockedDialogueTrigger != null)
                blockedDialogueTrigger.TriggerDialogue();
            return;
        }

        if (checkOnlyOnce && !string.IsNullOrEmpty(conditionID))
            passedConditions.Add(conditionID);

        ExecuteTeleport();
    }

    private bool MeetsConditions(PlayerController player)
    {
        if (player == null) return false;

        if (requireFlashlight && !PlayerController.hasFlashlight) return false;
        if (requireInteractedSwitch && !PlayerController.hasInteractedSwitch) return false;
        if (requireKey && !PlayerController.hasKey) return false;
        if (requiredInteractedDrawer && !PlayerController.hasInteractedDrawer) return false;
        if (requiredInteractedRef && !PlayerController.hasInteractedRef) return false;
        if (requiredInteractedBulletinBoard && !PlayerController.hasCheckedBulletinBoard) return false;
        if (requiredCheckedCaseFiles && !PlayerController.hasCheckedCaseFiles) return false;
        if (requireTalkedMarie && !NPCInteract.hasTalkedMarie) return false;
        if (requireTalkedSimon && !NPCInteract.hasTalkedSimon) return false;
        if (requireTalkedJohnuelle && !NPCInteract.hasTalkedJohnuelle) return false;

        return true;
    }

    private void ExecuteTeleport()
    {
        isTeleporting = true;

        PlayerPrefs.SetString("SpawnPoint", spawnPointName);
        PlayerPrefs.Save();

        Debug.Log($"Teleporting to scene '{sceneToLoad}', spawn point '{spawnPointName}'");

        SceneFadeIn fader = Object.FindFirstObjectByType<SceneFadeIn>();
        if (fader != null)
            fader.StartCoroutine(fader.FadeOutToBlack(teleportDelay));

        Invoke(nameof(LoadScene), teleportDelay);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}