using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unit.Swordsman
{
    public class SwordsmanCombatController : UnitCombatController
    {
        public override UniTask StartCombat(ICombatController combatController)
        {
            if (TryGetComponent(out IUnitMovement unitMovement))
            {
                unitMovement.SetTarget(combatController.transform.position);
            }

            Debug.Log("Swordsman combat started with " + combatController.transform.name);
            

            return base.StartCombat(combatController);
        }

        public override UniTask EndCombat(ICombatController combatController)
        {
            Debug.Log("Swordsman combat ended with " + combatController.transform.name);
            return base.EndCombat(combatController);
        }
    }
}