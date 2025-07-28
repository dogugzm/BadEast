using Cysharp.Threading.Tasks;
using Unit.Soldier;

namespace Unit.Swordsman
{
    public class SwordsmanSoldierCombatController : SoldierCombatController
    {
        protected override async UniTask PerformAttack()
        {
            if (CurrentTarget is null) return;
            if (CurrentTarget.CurrentHealth <= 0) return;
            CurrentTarget.TakeDamage(Damage);
        }
    }
}