using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unit
{
    public interface ICombatController
    {
        Transform transform { get; }
        CombatStatus CombatStatus { get; }
        UniTask StartCombat(ICombatController targetCombatController);
        UniTask EndCombat(ICombatController combatController);
    }
}