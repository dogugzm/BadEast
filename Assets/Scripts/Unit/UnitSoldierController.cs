using Formations;
using Unit.Soldier;
using UnityEngine;

namespace Unit
{
    public class UnitSoldierController : MonoBehaviour
    {
        [SerializeField] private Soldier.Soldier[] soldiers;

        public void Initialize()
        {
            if (soldiers == null || soldiers.Length == 0) return;

            var positions =
                BoxFormationHelper.GetPositions(soldiers.Length, transform.position, 2.0f, 1);

            for (int i = 0; i < soldiers.Length; i++)
            {
                if (soldiers[i] == null) continue;
                soldiers[i].TryGetComponent(out SoldierMovementController movementController);
                movementController.Init(positions[i], transform);
            }
        }

        public void HighlightSoldiers()
        {
            foreach (var soldier in soldiers)
            {
                if (soldier == null) continue;
                soldier.TryGetComponent(out SoldierVisualController soldierVisual);
                soldierVisual.Highlight();
            }
        }

        public void SetNormalSoldiers()
        {
            foreach (var soldier in soldiers)
            {
                if (soldier == null) continue;
                soldier.TryGetComponent(out SoldierVisualController soldierVisual);
                soldierVisual.SetNormal();
            }
        }
    }
}