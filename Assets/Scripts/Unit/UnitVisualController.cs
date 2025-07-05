using System;
using Army.Soldier;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

namespace Army
{
    public class UnitVisualController : MonoBehaviour
    {
        [FormerlySerializedAs("armySelectable")] [SerializeField] private UnitSelectable unitSelectable;
        [SerializeField] private SoldierVisual[] soldierVisuals;

        private void Awake()
        {
            unitSelectable.OnSelected += HandleSelected;
            unitSelectable.OnDeselected += HandleDeselected;
        }

        private void HandleDeselected(ISelectable obj)
        {
            foreach (var soldier in soldierVisuals)
            {
                soldier.SetNormal();
            }
        }

        private void HandleSelected(ISelectable obj)
        {
            foreach (var soldier in soldierVisuals)
            {
                soldier.Higlight();
            }
        }
    }
}