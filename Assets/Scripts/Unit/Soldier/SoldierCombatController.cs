using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Unit.Soldier
{
    public class SoldierCombatController : MonoBehaviour
    {
        [Serializable]
        public class Data
        {
            [field: SerializeField] public float AttackInterval { get; set; }
            [field: SerializeField] public float MinDamage { get; set; }
            [field: SerializeField] public float MaxDamage { get; set; }
            [field: SerializeField] public float Health { get; set; }
        }

        [SerializeField] private Data data; // Data for combat settings

        private bool _isInCombat = false;
        private IUnit _targetUnit;
        private CancellationTokenSource _combatCts;

        public float CurrentHealth => data.Health;

        public async UniTask StartCombat(IUnit unit)
        {
            if (unit == null || _isInCombat) return;

            _targetUnit = unit;
            _isInCombat = true;
            _combatCts = new CancellationTokenSource();

            try
            {
                await CombatRoutine(_combatCts.Token);
            }
            catch (System.OperationCanceledException)
            {
                // Cancellation is expected, no need to log
            }
            finally
            {
                EndCombatInternal();
            }
        }

        public void EndCombat()
        {
            if (!_isInCombat) return;
            _combatCts?.Cancel();
        }

        private void EndCombatInternal()
        {
            _isInCombat = false;
            _targetUnit = null;
            _combatCts?.Dispose();
            _combatCts = null;
        }

        private async UniTask CombatRoutine(CancellationToken cancellationToken)
        {
            while (_isInCombat && data.Health > 0 && !cancellationToken.IsCancellationRequested)
            {
                // Get all soldiers from the target unit
                if (_targetUnit is not null &&
                    _targetUnit.transform.TryGetComponent<UnitSoldierController>(out var unitController))
                {
                    Soldier[] enemySoldiers = unitController.GetSoldiers();

                    // Select a random living enemy soldier
                    var targetSoldier = GetRandomLivingSoldier(enemySoldiers);

                    if (targetSoldier != null)
                    {
                        // Deal random damage to the target
                        float damage = Random.Range(data.MinDamage, data.MaxDamage);
                        DealDamage(targetSoldier, damage);
                    }
                }

                await UniTask.Delay(TimeSpan.FromSeconds(data.AttackInterval),
                    cancellationToken: cancellationToken);
            }

            if (data.Health <= 0)
            {
                await UniTask.Yield();
                gameObject.SetActive(false);
            }
        }

        private Soldier GetRandomLivingSoldier(Soldier[] soldiers)
        {
            if (soldiers == null || soldiers.Length == 0) return null;

            // Filter living soldiers
            var livingSoldiers = soldiers
                .Where(s => s != null && s.TryGetComponent<SoldierCombatController>(out var combat) &&
                            combat.CurrentHealth > 0).ToArray();

            if (livingSoldiers.Length == 0) return null;

            // Return a random living soldier
            return livingSoldiers[Random.Range(0, livingSoldiers.Length)];
        }

        public void TakeDamage(float damage)
        {
            data.Health = Mathf.Max(0, data.Health - damage);

            if (data.Health <= 0)
            {
                EndCombat();
                gameObject.SetActive(false);
            }
        }

        private void DealDamage(Soldier target, float damage)
        {
            if (target != null && target.TryGetComponent<SoldierCombatController>(out var combatController))
            {
                combatController.TakeDamage(damage);
            }
        }

        private void OnDestroy()
        {
            _combatCts?.Cancel();
            _combatCts?.Dispose();
        }
    }
}