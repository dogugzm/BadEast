using UnityEngine;

namespace Unit
{
    public class UnitVisualController : MonoBehaviour
    {
        [SerializeField] private UnitSelectionController unitSelectionController;
        [SerializeField] private UnitSoldierController unitSoldierController;

        private void Awake()
        {
            unitSelectionController.OnSelected += HandleSelected;
            unitSelectionController.OnDeselected += HandleDeselected;
        }

        private void HandleDeselected(ISelectable _)
        {
            unitSoldierController.HighlightSoldiers();
        }

        private void HandleSelected(ISelectable _)
        {
            unitSoldierController.SetNormalSoldiers();
        }
    }
}