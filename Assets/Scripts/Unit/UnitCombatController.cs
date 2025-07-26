using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unit
{
    public class UnitCombatController : MonoBehaviour, ICombatController
    {
        public CombatStatus CombatStatus { get; protected set; }

        public virtual UniTask StartCombat(ICombatController combatController)
        {
            CombatStatus = CombatStatus.InCombat;
            return UniTask.CompletedTask;
        }

        public virtual UniTask EndCombat(ICombatController combatController)
        {
            CombatStatus = CombatStatus.OutOfCombat;
            return UniTask.CompletedTask;
        }
    }
}