using Anaphora.SanitySandbox;
using UnityEngine;

namespace Anaphora.HousePrototype
{
    public sealed class HousePicture : MonoBehaviour, IHouseInteractable
    {
        public SanityController Sanity;
        public bool HasBeenExamined { get; private set; }
        public string InteractionPrompt => HasBeenExamined
            ? "Family picture - already examined" : "E  Examine broken family picture";

        public void Interact()
        {
            if (HasBeenExamined || Sanity == null || Sanity.Paused) return;
            HasBeenExamined = true;
            Sanity.UnlockSwitching();
            Sanity.Shift();
        }

        public void ResetInspection() { HasBeenExamined = false; }
    }
}
