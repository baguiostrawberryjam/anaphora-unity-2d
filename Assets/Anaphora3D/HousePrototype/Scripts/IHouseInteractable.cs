using UnityEngine;

namespace Anaphora.HousePrototype
{
    public interface IHouseInteractable
    {
        string InteractionPrompt { get; }
        void Interact();
    }

    public static class HouseInteraction
    {
        public static IHouseInteractable Find(Collider hit)
        {
            foreach (MonoBehaviour component in hit.GetComponentsInParent<MonoBehaviour>())
                if (component is IHouseInteractable interaction) return interaction;
            return null;
        }
    }
}
