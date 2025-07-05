using System;
using Army.Soldier;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Army
{
    public class ArmyVisualController : MonoBehaviour
    {
        [SerializeField] private ArmySelectable armySelectable;
        [SerializeField] private SoldierVisual[] soldierVisuals;

        private void Awake()
        {
            armySelectable.OnSelected += HandleSelected;
            armySelectable.OnDeselected += HandleDeselected;
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