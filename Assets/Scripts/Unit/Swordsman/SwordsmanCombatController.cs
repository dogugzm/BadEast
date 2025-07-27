using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unit.Swordsman
{
    public class SwordsmanCombatController : UnitCombatController
    {
        public override UniTask StartCombat(ICombatController targetCombatController)
        {
            Debug.Log("Swordsman combat started with " + targetCombatController.transform.name);
            return base.StartCombat(targetCombatController);
        }

        public override UniTask EndCombat(ICombatController targetCombatController)
        {
            Debug.Log("Swordsman combat ended with " + targetCombatController.transform.name);
            return base.EndCombat(targetCombatController);
        }
    }
}