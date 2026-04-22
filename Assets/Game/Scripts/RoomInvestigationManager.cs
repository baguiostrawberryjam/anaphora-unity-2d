using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

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

    [Header("Bear Final Interact")]
    public InteractTrigger bearFinalTrigger;

    [Header("=== WALL FIX ===")]
    public GameObject objectsWallTilemap;

    // Room investigation state
    private bool closetDone = false;
    private bool holeDone = false;
    private bool bedDone = false;
    private bool drawerDone = false;
    private bool allRoomDoneTriggered = false;

    // Bear puzzle item state
    private bool pincushionCollected = false;
    private bool poloShirtCollected = false;
    private bool sewingKitCollected = false;
    private bool ribbonCollected = false;
    private bool bedFoamCollected = false;

    private void Start()
    {

        if (bedSecondInteract != null)
            bedSecondInteract.gameObject.SetActive(false);

        if (bedFoamTrigger != null)
            bedFoamTrigger.gameObject.SetActive(false);

        if (bearFinalTrigger != null)
            bearFinalTrigger.gameObject.SetActive(false);

        if (pincushionTrigger != null)
            pincushionTrigger.gameObject.SetActive(false);

        if (poloShirtTrigger != null)
            poloShirtTrigger.gameObject.SetActive(false);

        if (sewingKitTrigger != null)
            sewingKitTrigger.gameObject.SetActive(false);

        if (ribbonTrigger != null)
            ribbonTrigger.gameObject.SetActive(false);
    }

    // =====================
    // ROOM INVESTIGATION
    // =====================

    public void SetClosetDone() { closetDone = true; CheckAllRoomDone(); }
    public void SetHoleDone() { holeDone = true; CheckAllRoomDone(); }
    public void SetBedDone() { bedDone = true; CheckAllRoomDone(); }
    public void SetDrawerDone() { drawerDone = true; CheckAllRoomDone(); }

    void CheckAllRoomDone()
    {
        if (allRoomDoneTriggered) return;
        if (!closetDone || !holeDone || !bedDone || !drawerDone) return;

        allRoomDoneTriggered = true;
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

        if (pincushionTrigger != null)
            pincushionTrigger.gameObject.SetActive(true);

        if (poloShirtTrigger != null)
            poloShirtTrigger.gameObject.SetActive(true);

        if (sewingKitTrigger != null)
            sewingKitTrigger.gameObject.SetActive(true);

        if (ribbonTrigger != null)
            ribbonTrigger.gameObject.SetActive(true);
    }

    // =====================
    // BEAR PUZZLE ITEMS
    // =====================

    public void SetPincushionCollected()
    {
        pincushionCollected = true;
        CheckBearPuzzleDone();
    }

    public void SetPoloShirtCollected()
    {
        poloShirtCollected = true;
        CheckBearPuzzleDone();
    }

    public void SetSewingKitCollected()
    {
        sewingKitCollected = true;
        CheckBearPuzzleDone();
    }

    public void SetRibbonCollected()
    {
        ribbonCollected = true;

        if (bedFoamTrigger != null)
            bedFoamTrigger.gameObject.SetActive(true);

        CheckBearPuzzleDone();
    }

    public void SetBedFoamCollected()
    {
        bedFoamCollected = true;
        StartCoroutine(SwapBedVisual());
        CheckBearPuzzleDone();
    }

    IEnumerator SwapBedVisual()
    {
        yield return null; // wait one frame

        fixedBed.SetActive(false);
        destroyedBed.SetActive(true);

        yield return null; // wait another frame after swap

        // Force the wall layer back on after bed swap
        if (objectsWallTilemap != null)
        {
            objectsWallTilemap.SetActive(false);
            objectsWallTilemap.SetActive(true);
        }
    }

    void CheckBearPuzzleDone()
    {
        if (!pincushionCollected || !poloShirtCollected ||
            !sewingKitCollected || !ribbonCollected || !bedFoamCollected) return;

        StartCoroutine(BearFixedSequence());
    }

    IEnumerator BearFixedSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (dialogue50 != null)
            dialogue50.TriggerDialogue();

        yield return new WaitUntil(() =>
            dialogue50 == null || !dialogue50.dialoguePanel.activeSelf);

        if (bearFinalTrigger != null)
            bearFinalTrigger.gameObject.SetActive(true);
    }
}