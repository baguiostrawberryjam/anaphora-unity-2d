using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    private void Awake()
    {
        // Singleton - persist across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0 || scene.buildIndex == 1)
        {
            return;
        }

        SaveGame();
        LoadIntoStaticVars();
    }

    // =====================
    // SAVE
    // =====================
    public void SaveGame()
    {
        PlayerPrefs.SetInt("hasFlashlight", PlayerController.hasFlashlight ? 1 : 0);
        PlayerPrefs.SetInt("hasInteractedSwitch", PlayerController.hasInteractedSwitch ? 1 : 0);
        PlayerPrefs.SetInt("hasKey", PlayerController.hasKey ? 1 : 0);
        PlayerPrefs.SetInt("hasInteractedDrawer", PlayerController.hasInteractedDrawer ? 1 : 0);
        PlayerPrefs.SetInt("hasInteractedRef", PlayerController.hasInteractedRef ? 1 : 0);
        PlayerPrefs.SetInt("hasCheckedBulletinBoard", PlayerController.hasCheckedBulletinBoard ? 1 : 0);
        PlayerPrefs.SetInt("hasCheckedCaseFiles", PlayerController.hasCheckedCaseFiles ? 1 : 0);

        PlayerPrefs.SetString("savedScene", SceneManager.GetActiveScene().name);

        PlayerPrefs.SetInt("hasTalkedMarie", NPCInteract.hasTalkedMarie ? 1 : 0);
        PlayerPrefs.SetInt("hasTalkedSimon", NPCInteract.hasTalkedSimon ? 1 : 0);
        PlayerPrefs.SetInt("hasTalkedJohnuelle", NPCInteract.hasTalkedJohnuelle ? 1 : 0);

        PlayerPrefs.SetInt("room_closetDone", RoomInvestigationManager.IsDoneStatic("closetDone") ? 1 : 0);
        PlayerPrefs.SetInt("room_holeDone", RoomInvestigationManager.IsDoneStatic("holeDone") ? 1 : 0);
        PlayerPrefs.SetInt("room_bedDone", RoomInvestigationManager.IsDoneStatic("bedDone") ? 1 : 0);
        PlayerPrefs.SetInt("room_drawerDone", RoomInvestigationManager.IsDoneStatic("drawerDone") ? 1 : 0);
        PlayerPrefs.SetInt("room_allRoomDone", RoomInvestigationManager.IsDoneStatic("allRoomDone") ? 1 : 0);
        PlayerPrefs.SetInt("room_bedSecondDone", RoomInvestigationManager.IsDoneStatic("bedSecondDone") ? 1 : 0);
        PlayerPrefs.SetInt("room_pincushionCollected", RoomInvestigationManager.IsDoneStatic("pincushionCollected") ? 1 : 0);
        PlayerPrefs.SetInt("room_poloShirtCollected", RoomInvestigationManager.IsDoneStatic("poloShirtCollected") ? 1 : 0);
        PlayerPrefs.SetInt("room_sewingKitCollected", RoomInvestigationManager.IsDoneStatic("sewingKitCollected") ? 1 : 0);
        PlayerPrefs.SetInt("room_ribbonCollected", RoomInvestigationManager.IsDoneStatic("ribbonCollected") ? 1 : 0);
        PlayerPrefs.SetInt("room_bedFoamCollected", RoomInvestigationManager.IsDoneStatic("bedFoamCollected") ? 1 : 0);
        PlayerPrefs.SetInt("room_bearPuzzleDone", RoomInvestigationManager.IsDoneStatic("bearPuzzleDone") ? 1 : 0);

        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Game saved.");
    }

    public void LoadIntoStaticVars()
    {
        PlayerController.hasFlashlight = PlayerPrefs.GetInt("hasFlashlight", 0) == 1;
        PlayerController.hasInteractedSwitch = PlayerPrefs.GetInt("hasInteractedSwitch", 0) == 1;
        PlayerController.hasKey = PlayerPrefs.GetInt("hasKey", 0) == 1;
        PlayerController.hasInteractedDrawer = PlayerPrefs.GetInt("hasInteractedDrawer", 0) == 1;
        PlayerController.hasInteractedRef = PlayerPrefs.GetInt("hasInteractedRef", 0) == 1;
        PlayerController.hasCheckedBulletinBoard = PlayerPrefs.GetInt("hasCheckedBulletinBoard", 0) == 1;
        PlayerController.hasCheckedCaseFiles = PlayerPrefs.GetInt("hasCheckedCaseFiles", 0) == 1;

        NPCInteract.hasTalkedMarie = PlayerPrefs.GetInt("hasTalkedMarie", 0) == 1;
        NPCInteract.hasTalkedSimon = PlayerPrefs.GetInt("hasTalkedSimon", 0) == 1;
        NPCInteract.hasTalkedJohnuelle = PlayerPrefs.GetInt("hasTalkedJohnuelle", 0) == 1;

        RoomInvestigationManager.LoadStepStatic("closetDone", PlayerPrefs.GetInt("room_closetDone", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("holeDone", PlayerPrefs.GetInt("room_holeDone", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("bedDone", PlayerPrefs.GetInt("room_bedDone", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("drawerDone", PlayerPrefs.GetInt("room_drawerDone", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("allRoomDone", PlayerPrefs.GetInt("room_allRoomDone", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("bedSecondDone", PlayerPrefs.GetInt("room_bedSecondDone", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("pincushionCollected", PlayerPrefs.GetInt("room_pincushionCollected", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("poloShirtCollected", PlayerPrefs.GetInt("room_poloShirtCollected", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("sewingKitCollected", PlayerPrefs.GetInt("room_sewingKitCollected", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("ribbonCollected", PlayerPrefs.GetInt("room_ribbonCollected", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("bedFoamCollected", PlayerPrefs.GetInt("room_bedFoamCollected", 0) == 1);
        RoomInvestigationManager.LoadStepStatic("bearPuzzleDone", PlayerPrefs.GetInt("room_bearPuzzleDone", 0) == 1);

        LoadHashSet(DialogueTrigger.sessionTriggers, PlayerPrefs.GetString("savedDialogueTriggers", ""));
        LoadHashSet(DoorTeleport.triggeredDoors, PlayerPrefs.GetString("savedDoorTeleports", ""));
        LoadHashSet(SceneTeleport.passedConditions, PlayerPrefs.GetString("savedSceneTeleports", ""));

        Debug.Log("[SaveSystem] Game loaded into static vars.");
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PlayerController.hasFlashlight = false;
        PlayerController.hasInteractedSwitch = false;
        PlayerController.hasKey = false;
        PlayerController.hasInteractedDrawer = false;
        PlayerController.hasInteractedRef = false;
        PlayerController.hasCheckedBulletinBoard = false;
        PlayerController.hasCheckedCaseFiles = false;

        NPCInteract.hasTalkedMarie = false;
        NPCInteract.hasTalkedSimon = false;
        NPCInteract.hasTalkedJohnuelle = false;

        RoomInvestigationManager.ClearAllSteps();

        Debug.Log("[SaveSystem] Save deleted.");
    }

    private void LoadHashSet(HashSet<string> hashSet, string savedData)
    {
        hashSet.Clear();
        if (!string.IsNullOrEmpty(savedData))
        {
            string[] items = savedData.Split(',');
            foreach (string item in items)
            {
                hashSet.Add(item);
            }
        }
    }
}