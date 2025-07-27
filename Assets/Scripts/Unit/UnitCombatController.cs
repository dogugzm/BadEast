using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unit
{
    public class UnitCombatController : MonoBehaviour, ICombatController
    {
        public CombatStatus CombatStatus { get; protected set; }

        public virtual UniTask StartCombat(ICombatController targetCombatController)
        {
            CombatStatus = CombatStatus.InCombat;
            if (!transform.TryGetComponent(out UnitSoldierController unitSoldierController))
                return UniTask.CompletedTask;
            unitSoldierController.StartCombat(targetCombatController.transform.GetComponent<IUnit>());
            return UniTask.CompletedTask;
        }

        public virtual UniTask EndCombat(ICombatController targetCombatController)
        {
            CombatStatus = CombatStatus.OutOfCombat;
            if (!transform.TryGetComponent(out UnitSoldierController unitSoldierController))
                return UniTask.CompletedTask;
            unitSoldierController.EndCombat();
            return UniTask.CompletedTask;
        }
    }
}