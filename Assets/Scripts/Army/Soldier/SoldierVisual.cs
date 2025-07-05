using System;
using UnityEngine;

namespace Army.Soldier
{
    public class SoldierVisual : MonoBehaviour
    {
        private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");
        [SerializeField] private Renderer soldierRenderer;
        private const float HighlightedIntensity = 2.0f;
        private const float NormalIntensity = 1.0f;

        private void Awake()
        {
            SetNormal();
        }

        public void Higlight()
        {
            if (soldierRenderer != null)
            {
                soldierRenderer.material.SetColor(EmissiveColor, Color.cyan * HighlightedIntensity);
            }
        }

        public void SetNormal()
        {
            if (soldierRenderer != null)
            {
                soldierRenderer.material.SetColor(EmissiveColor, Color.cyan * NormalIntensity);
            }
        }
    }
}