using Unit.Soldier;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit
{
    public class UnitVisualController : MonoBehaviour
    {
        [FormerlySerializedAs("armySelectable")] [SerializeField]
        private UnitSelectable unitSelectable;

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