using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomInvestigationManager : MonoBehaviour
{
    [Header("=== DOOR 1 ROOM INVESTIGATION ===")]
    public InteractTrigger closetTrigger;
    public InteractTrigger holeInWallTrigger;
    public InteractTrigger bedTrigger;
    public InteractTrigger drawerTrigger;

    [Header("After All 4 Done")]
    public DialogueTrigger allDoneAndSparkleDialogue;
    public InteractTrigger bedSecondInteract;

    [Header("=== BED VISUAL ===")]
    public GameObject fixedBed;
    public GameObject destroyedBed;

    [Header("=== BEAR PUZZLE ITEMS ===")]
    [Header("Room Items")]
    public InteractTrigger pincushionTrigger;
    public InteractTrigger poloShirtTrigger;
    public InteractTrigger bedFoamTrigger;

    [Header("Living Room Items")]
    public InteractTrigger sewingKitTrigger;
    public InteractTrigger ribbonTrigger;

    [Header("Bear Puzzle Dialogues")]
    public DialogueTrigger dialogue44;
    public DialogueTrigger dialogue50;

    [Header("=== CUTSCENE ===")]
    public CutsceneManager cutsceneManager;

    [Header("=== WALL FIX ===")]
    public GameObject objectsWallTilemap;

    // =====================
    // SESSION STATE
    // =====================
    private static HashSet<string> completedSteps = new HashSet<string>();

    private const string STEP_CLOSET = "closetDone";
    private const string STEP_HOLE = "holeDone";
    private const string STEP_BED = "bedDone";
    private const string STEP_DRAWER = "drawerDone";
    private const string STEP_ALL_ROOM = "allRoomDone";
    private const string STEP_BED_SECOND = "bedSecondDone";
    private const string STEP_PINCUSHION = "pincushionCollected";
    private const string STEP_POLO = "poloShirtCollected";
    private const string STEP_SEWING = "sewingKitCollected";
    private const string STEP_RIBBON = "ribbonCollected";
    private const string STEP_BEDFOAM = "bedFoamCollected";
    private const string STEP_BEAR_PUZZLE = "bearPuzzleDone";

    private bool IsDone(string step) => completedSteps.Contains(step);
    private void MarkDone(string step) => completedSteps.Add(step);

    private void Start()
    {
        RestoreState();
    }

    void RestoreState()
    {
        if (IsDone(STEP_BEDFOAM))
        {
            if (fixedBed != null) fixedBed.SetActive(false);
            if (destroyedBed != null) destroyedBed.SetActive(true);
        }

        if (IsDone(STEP_BEAR_PUZZLE))
        {
            // Bear puzzle already completed — trigger the final dialogue immediately
            // (or skip if you only want it to play once per session)
            HideCollectedItems();
            return;
        }

        if (IsDone(STEP_BED_SECOND))
        {
            if (pincushionTrigger != null)
                pincushionTrigger.gameObject.SetActive(!IsDone(STEP_PINCUSHION));
            if (poloShirtTrigger != null)
                poloShirtTrigger.gameObject.SetActive(!IsDone(STEP_POLO));
            if (sewingKitTrigger != null)
                sewingKitTrigger.gameObject.SetActive(!IsDone(STEP_SEWING));
            if (ribbonTrigger != null)
                ribbonTrigger.gameObject.SetActive(!IsDone(STEP_RIBBON));
            if (bedFoamTrigger != null)
                bedFoamTrigger.gameObject.SetActive(IsDone(STEP_RIBBON) && !IsDone(STEP_BEDFOAM));
            return;
        }

        if (IsDone(STEP_ALL_ROOM))
        {
            if (bedSecondInteract != null) bedSecondInteract.gameObject.SetActive(true);
            return;
        }

        if (bedSecondInteract != null) bedSecondInteract.gameObject.SetActive(false);
        if (bedFoamTrigger != null) bedFoamTrigger.gameObject.SetActive(false);
        if (pincushionTrigger != null) pincushionTrigger.gameObject.SetActive(false);
        if (poloShirtTrigger != null) poloShirtTrigger.gameObject.SetActive(false);
        if (sewingKitTrigger != null) sewingKitTrigger.gameObject.SetActive(false);
        if (ribbonTrigger != null) ribbonTrigger.gameObject.SetActive(false);
    }

    void HideCollectedItems()
    {
        if (pincushionTrigger != null) pincushionTrigger.gameObject.SetActive(false);
        if (poloShirtTrigger != null) poloShirtTrigger.gameObject.SetActive(false);
        if (sewingKitTrigger != null) sewingKitTrigger.gameObject.SetActive(false);
        if (ribbonTrigger != null) ribbonTrigger.gameObject.SetActive(false);
        if (bedFoamTrigger != null) bedFoamTrigger.gameObject.SetActive(false);
        if (bedSecondInteract != null) bedSecondInteract.gameObject.SetActive(false);
    }

    // =====================
    // ROOM INVESTIGATION
    // =====================

    public void SetClosetDone() { MarkDone(STEP_CLOSET); CheckAllRoomDone(); }
    public void SetHoleDone() { MarkDone(STEP_HOLE); CheckAllRoomDone(); }
    public void SetBedDone() { MarkDone(STEP_BED); CheckAllRoomDone(); }
    public void SetDrawerDone() { MarkDone(STEP_DRAWER); CheckAllRoomDone(); }

    void CheckAllRoomDone()
    {
        if (IsDone(STEP_ALL_ROOM)) return;
        if (!IsDone(STEP_CLOSET) || !IsDone(STEP_HOLE) ||
            !IsDone(STEP_BED) || !IsDone(STEP_DRAWER)) return;

        MarkDone(STEP_ALL_ROOM);
        StartCoroutine(AllRoomDoneSequence());
    }

    IEnumerator AllRoomDoneSequence()
    {
        if (allDoneAndSparkleDialogue != null)
            allDoneAndSparkleDialogue.TriggerDialogue();

        yield return new WaitUntil(() =>
            allDoneAndSparkleDialogue == null || !allDoneAndSparkleDialogue.dialoguePanel.activeSelf);

        if (bedSecondInteract != null)
            bedSecondInteract.gameObject.SetActive(true);
    }

    // =====================
    // BED SECOND INTERACT
    // =====================

    public void SetBedSecondDone()
    {
        MarkDone(STEP_BED_SECOND);
        StartCoroutine(BearFoundSequence());
    }

    IEnumerator BearFoundSequence()
    {
        yield return new WaitUntil(() =>
            bedSecondInteract == null || !bedSecondInteract.dialoguePanel.activeSelf);

        yield return new WaitForSeconds(0.5f);

        if (dialogue44 != null)
            dialogue44.TriggerDialogue();

        yield return new WaitUntil(() =>
            dialogue44 == null || !dialogue44.dialoguePanel.activeSelf);

        if (pincushionTrigger != null) pincushionTrigger.gameObject.SetActive(true);
        if (poloShirtTrigger != null) poloShirtTrigger.gameObject.SetActive(true);
        if (sewingKitTrigger != null) sewingKitTrigger.gameObject.SetActive(true);
        if (ribbonTrigger != null) ribbonTrigger.gameObject.SetActive(true);
    }

    // =====================
    // BEAR PUZZLE ITEMS
    // =====================

    public void SetPincushionCollected()
    {
        MarkDone(STEP_PINCUSHION);
        if (pincushionTrigger != null) pincushionTrigger.gameObject.SetActive(false);
        CheckBearPuzzleDone();
    }

    public void SetPoloShirtCollected()
    {
        MarkDone(STEP_POLO);
        if (poloShirtTrigger != null) poloShirtTrigger.gameObject.SetActive(false);
        CheckBearPuzzleDone();
    }

    public void SetSewingKitCollected()
    {
        MarkDone(STEP_SEWING);
        if (sewingKitTrigger != null) sewingKitTrigger.gameObject.SetActive(false);
        CheckBearPuzzleDone();
    }

    public void SetRibbonCollected()
    {
        MarkDone(STEP_RIBBON);
        if (ribbonTrigger != null) ribbonTrigger.gameObject.SetActive(false);
        if (bedFoamTrigger != null) bedFoamTrigger.gameObject.SetActive(true);
        CheckBearPuzzleDone();
    }

    public void SetBedFoamCollected()
    {
        MarkDone(STEP_BEDFOAM);
        if (bedFoamTrigger != null) bedFoamTrigger.gameObject.SetActive(false);
        StartCoroutine(SwapBedVisual());
        CheckBearPuzzleDone();
    }

    IEnumerator SwapBedVisual()
    {
        yield return null;

        if (fixedBed != null) fixedBed.SetActive(false);
        if (destroyedBed != null) destroyedBed.SetActive(true);

        yield return null;

        if (objectsWallTilemap != null)
        {
            objectsWallTilemap.SetActive(false);
            objectsWallTilemap.SetActive(true);
        }
    }

    void CheckBearPuzzleDone()
    {
        if (IsDone(STEP_BEAR_PUZZLE)) return;

        // Print the exact state of all 5 items to the console
        Debug.Log($"Checking Puzzle State -> Pincushion: {IsDone(STEP_PINCUSHION)} | Polo: {IsDone(STEP_POLO)} | Sewing: {IsDone(STEP_SEWING)} | Ribbon: {IsDone(STEP_RIBBON)} | Foam: {IsDone(STEP_BEDFOAM)}");

        if (!IsDone(STEP_PINCUSHION) || !IsDone(STEP_POLO) ||
            !IsDone(STEP_SEWING) || !IsDone(STEP_RIBBON) || !IsDone(STEP_BEDFOAM))
        {
            Debug.LogWarning("Bear puzzle NOT done. Missing items.");
            return;
        }

        Debug.Log("All 5 items collected! Triggering BearFixedSequence...");
        MarkDone(STEP_BEAR_PUZZLE);
        StartCoroutine(BearFixedSequence());
    }

    IEnumerator BearFixedSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (dialogue50 != null)
            dialogue50.TriggerDialogue();

        yield return new WaitUntil(() =>
            dialogue50 == null || !dialogue50.dialoguePanel.activeSelf);

        StartCoroutine(BearEndingSequence());
    }

    // =====================
    // BEAR ENDING
    // =====================

    IEnumerator BearEndingSequence()
    {
        if (cutsceneManager != null)
        {
            bool cutsceneDone = false;
            cutsceneManager.gameObject.SetActive(true);
            yield return null; // wait one frame
            cutsceneManager.PlayCutscene(() => cutsceneDone = true);
            yield return new WaitUntil(() => cutsceneDone);
        }
    }
}