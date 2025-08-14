using System.Collections.Generic;
using Formations;
using Unit.Soldier;
using UnityEngine;

namespace Unit
{
    public class UnitSoldierController : MonoBehaviour
    {
        [SerializeField] private Soldier.Soldier _soldierPrefab;

        private List<ISoldier> _soldiers = new();

        public void Initialize(int unitSize, UnitSide side)
        {
            var positions =
                BoxFormationHelper.GetPositions(unitSize, 2.0f);

            for (int i = 0; i < unitSize; i++)
            {
                var soldier = Instantiate(_soldierPrefab, positions[i], Quaternion.identity);
                soldier.Init(side);
                soldier.transform.position = transform.position;
                soldier.TryGetComponent(out SoldierMovementController movementController);
                movementController.Init(positions[i], transform);
                _soldiers.Add(soldier);
            }
        }

        public void HighlightSoldiers()
        {
            foreach (var soldier in _soldiers)
            {
                if (soldier == null) continue;
                soldier.transform.TryGetComponent(out SoldierVisualController soldierVisual);
                soldierVisual.Highlight();
            }
        }

        public void SetNormalSoldiers()
        {
            foreach (var soldier in _soldiers)
            {
                if (soldier == null) continue;
                soldier.transform.TryGetComponent(out SoldierVisualController soldierVisual);
                soldierVisual.SetNormal();
            }
        }
    }
}