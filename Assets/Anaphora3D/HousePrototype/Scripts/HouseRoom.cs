using UnityEngine;

namespace Anaphora.HousePrototype
{
    public sealed class HouseRoom : MonoBehaviour
    {
        public string Label;
        public bool CountForExploration = true;
        public BoxCollider Area;
        public bool Visited { get; set; }

        public bool Contains(Vector3 point) { return Area != null && Area.bounds.Contains(point); }
    }
}
