using Cysharp.Threading.Tasks;
using Unit.Soldier;
using UnityEngine;

namespace Unit.Swordsman
{
    public class SwordsmanSoldierCombatController : SoldierCombatController
    {
        protected override async UniTask PerformAttack()
        {
            if (Vector3.Distance(transform.position, CurrentTarget.transform.position) <= AttackRange)
            {
                ApplyDamage();
            }
            else
            {
                await movementController.MoveTowards(CurrentTarget.transform.position, AttackRange);
                ApplyDamage();
                movementController.SetFollowFormation(true);
            }
        }
    }
}