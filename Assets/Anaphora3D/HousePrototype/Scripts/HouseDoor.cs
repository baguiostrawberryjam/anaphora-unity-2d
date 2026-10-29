using UnityEngine;

namespace Anaphora.HousePrototype
{
    [DefaultExecutionOrder(-50)]
    public sealed class HouseDoor : MonoBehaviour, IHouseInteractable
    {
        public string Label = "Door";
        public Transform Leaf;
        public BoxCollider LeafCollider;
        public float OpenAngle = -95;
        public float DegreesPerSecond = 150;
        public bool Paused { get; set; }
        public bool IsOpen { get; private set; }
        public bool Obstructed { get; private set; }
        private Quaternion closedRotation;
        private float angle;
        public string InteractionPrompt => "E  " + (IsOpen ? "Close " : "Open ") + Label +
            (Obstructed ? "\nStep clear to let the door move." : "");
        public void Interact() { Toggle(); }

        private void Awake() { closedRotation = transform.localRotation; }

        public void Toggle() { IsOpen = !IsOpen; }

        public void ResetDoor()
        {
            IsOpen = false;
            angle = 0;
            Obstructed = false;
            transform.localRotation = closedRotation;
        }

        private void Update()
        {
            if (Paused || Leaf == null || LeafCollider == null) return;
            float next = Mathf.MoveTowards(angle, IsOpen ? OpenAngle : 0, DegreesPerSecond * Time.deltaTime);
            if (Mathf.Approximately(next, angle)) return;
            Obstructed = false;
            // Check the swing in small steps even after a slow frame, so it cannot skip past the player.
            int steps = Mathf.CeilToInt(Mathf.Abs(next - angle) / 3f);
            float start = angle;
            for (int step = 1; step <= steps; step++)
            {
                float candidate = Mathf.Lerp(start, next, step / (float)steps);
                Quaternion previous = transform.localRotation;
                transform.localRotation = closedRotation * Quaternion.Euler(0, candidate, 0);
                Vector3 center = Leaf.TransformPoint(LeafCollider.center);
                Vector3 half = Vector3.Scale(LeafCollider.size * 0.5f, Leaf.lossyScale) + Vector3.one * 0.06f;
                foreach (Collider hit in Physics.OverlapBox(center, half, Leaf.rotation, ~0, QueryTriggerInteraction.Ignore))
                {
                    if (hit is CharacterController) { Obstructed = true; break; }
                }
                if (Obstructed) { transform.localRotation = previous; break; }
                angle = candidate;
            }
            Physics.SyncTransforms();
        }

        // Used by the editor's geometry check without running a frame loop.
        public void SetOpenImmediately(bool open)
        {
            if (!Application.isPlaying) closedRotation = transform.localRotation * Quaternion.Inverse(Quaternion.Euler(0, angle, 0));
            IsOpen = open;
            angle = open ? OpenAngle : 0;
            transform.localRotation = closedRotation * Quaternion.Euler(0, angle, 0);
        }
    }
}
