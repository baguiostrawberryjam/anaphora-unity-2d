using UnityEngine;

namespace Anaphora.HousePrototype
{
    [ExecuteAlways, RequireComponent(typeof(Renderer))]
    public sealed class HouseSurface : MonoBehaviour
    {
        public Color Tint = Color.white;
        private bool reality = true;
        private void OnEnable() { Apply(); }
        private void OnValidate() { Apply(); }
        public void SetWorld(bool isReality) { reality = isReality; Apply(); }
        public void Apply()
        {
            Renderer surface = GetComponent<Renderer>();
            var properties = new MaterialPropertyBlock();
            surface.GetPropertyBlock(properties);
            float grey = Mathf.Lerp(0.25f, 0.85f, Tint.grayscale);
            properties.SetColor("_BaseColor", reality ? Tint : new Color(grey, grey, grey, Tint.a));
            surface.SetPropertyBlock(properties);
        }
    }
}
