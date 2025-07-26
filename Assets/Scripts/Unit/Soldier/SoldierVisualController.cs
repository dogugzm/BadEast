using UnityEngine;

namespace Unit.Soldier
{
    public class SoldierVisualController : MonoBehaviour
    {
        private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");
        private const float HighlightedIntensity = 2.0f;
        private const float NormalIntensity = 1.0f;

        [SerializeField] private Renderer soldierRenderer;
        [SerializeField] private Color emissiveColor;

        private void Awake()
        {
            soldierRenderer.material.color = emissiveColor;
            SetNormal();
        }

        public void Highlight()
        {
            if (soldierRenderer != null)
            {
                soldierRenderer.material.SetColor(EmissiveColor, emissiveColor * HighlightedIntensity);
            }
        }

        public void SetNormal()
        {
            if (soldierRenderer != null)
            {
                soldierRenderer.material.SetColor(EmissiveColor, emissiveColor * NormalIntensity);
            }
        }
    }
}