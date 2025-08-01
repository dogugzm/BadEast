using System;
using Cysharp.Threading.Tasks;
using Unit.Soldier;
using UnityEngine;

namespace Unit.Archer
{
    public class ArcherSoldierCombatController : SoldierCombatController
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private float accuracyRadius = 1f; // The radius of inaccuracy around the target.

        /// <summary>
        /// Overrides the base attack method to fire an arrow projectile.
        /// </summary>
        protected override async UniTask PerformAttack()
        {
            if (CurrentTarget == null) return;

            // The archer "charges" their attack for a moment.
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            if (CurrentTarget == null || CurrentTarget.CurrentHealth <= 0)
            {
                return; // Target died during the charge time.
            }

            // Get the target's position with some random inaccuracy.
            Vector3 targetPosition = CurrentTarget.transform.position;
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * accuracyRadius;
            targetPosition.x += randomOffset.x;
            targetPosition.z += randomOffset.y;

            // Instantiate the arrow at the archer's position.
            GameObject newArrowObject = Instantiate(arrowPrefab);
            newArrowObject.transform.position = transform.position + Vector3.up * 0.5f; // Adjust height if needed

            // Get the Arrow script and initialize it.
            if (newArrowObject.TryGetComponent(out Arrow arrowScript))
            {
                arrowScript.Initialize(Damage, targetPosition);
            }
        }
    }
}