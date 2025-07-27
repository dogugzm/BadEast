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
            [field: SerializeField] public float AttackInterval { get; set; } = 1f;
            [field: SerializeField] public float MinDamage { get; set; } = 5f;
            [field: SerializeField] public float MaxDamage { get; set; } = 15f;
            [field: SerializeField] public float Health { get; set; } = 100f;
            [field: SerializeField] public float AttackRange { get; set; } = 1.5f; // Distance to attack
        }

        [SerializeField] private Data data; // Data for combat settings
        [SerializeField] private SoldierMovementController movementController; // Reference to movement controller

        private bool _isInCombat = false;
        private IUnit _targetUnit;
        private CancellationTokenSource _combatCts;
        private Soldier _currentTargetSoldier; // Track the current target soldier

        public float CurrentHealth => data.Health;

        public async UniTask StartCombat(IUnit unit)
        {
            if (unit == null || _isInCombat || movementController == null) return;

            _targetUnit = unit;
            _isInCombat = true;
            _combatCts = new CancellationTokenSource();

            try
            {
                // Disable formation following
                movementController.SetFollowFormation(false);
                await CombatRoutine(_combatCts.Token);
            }
            catch (OperationCanceledException)
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
            _currentTargetSoldier = null;
            _combatCts?.Dispose();
            _combatCts = null;

            // Re-enable formation following
            if (movementController != null)
            {
                movementController.SetFollowFormation(true);
            }
        }

        private async UniTask CombatRoutine(CancellationToken cancellationToken)
        {
            while (_isInCombat && data.Health > 0 && !cancellationToken.IsCancellationRequested)
            {
                if (_targetUnit is not null &&
                    _targetUnit.transform.TryGetComponent<UnitSoldierController>(out var unitController))
                {
                    // Select a new target if none or current target is dead
                    _currentTargetSoldier = GetRandomLivingSoldier(unitController.GetSoldiers());

                    if (_currentTargetSoldier == null)
                    {
                        // No living enemies, end combat
                        EndCombat();
                        break;
                    }

                    // Calculate distance to target
                    Vector3 targetPosition = _currentTargetSoldier.transform.position;
                    float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

                    if (distanceToTarget > data.AttackRange)
                    {
                        // Move toward the target
                        await MoveToTarget(_currentTargetSoldier, cancellationToken);
                    }

                    // If still in combat and target is valid, attack
                    if (_isInCombat && _currentTargetSoldier != null &&
                        _currentTargetSoldier.TryGetComponent<SoldierCombatController>(out var targetCombat) &&
                        targetCombat.CurrentHealth > 0 &&
                        Vector3.Distance(transform.position, _currentTargetSoldier.transform.position) <= data.AttackRange)
                    {
                        float damage = Random.Range(data.MinDamage, data.MaxDamage);
                        DealDamage(_currentTargetSoldier, damage);
                    }
                }
                else
                {
                    // Target unit is invalid, end combat
                    EndCombat();
                    break;
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

        private async UniTask MoveToTarget(Soldier targetSoldier, CancellationToken cancellationToken)
        {
            float moveSpeed = movementController.GetMoveSpeed();
            float attackRange = data.AttackRange; // Use AttackRange for stopping

            while (_isInCombat && data.Health > 0 && !cancellationToken.IsCancellationRequested &&
                   targetSoldier != null &&
                   targetSoldier.TryGetComponent<SoldierCombatController>(out var targetCombat) &&
                   targetCombat.CurrentHealth > 0)
            {
                Vector3 targetPosition = targetSoldier.transform.position;
                float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

                if (distanceToTarget <= attackRange)
                {
                    break; // Within attack range, stop moving
                }

                // Move toward the target, aiming to be exactly at AttackRange
                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                Vector3 desiredPosition = targetPosition - directionToTarget * attackRange;
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    desiredPosition,
                    moveSpeed * Time.deltaTime);

                await UniTask.Yield(cancellationToken);
            }
        }

        private Soldier GetRandomLivingSoldier(Soldier[] soldiers)
        {
            if (soldiers == null || soldiers.Length == 0) return null;

            var livingSoldiers = soldiers
                .Where(s => s != null && s.TryGetComponent<SoldierCombatController>(out var combat) &&
                            combat.CurrentHealth > 0).ToArray();

            if (livingSoldiers.Length == 0) return null;

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