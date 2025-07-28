using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Unit.Soldier
{
    public interface IDamageable
    {
        public Transform transform { get; }
        public float CurrentHealth { get; }
        public void TakeDamage(float damage);
        public UniTask Die();
    }

    public interface ITargetSearchable
    {
        public float VisibleRange { get; }
        public CancellationTokenSource SearchCts { get; set; }
        public UniTask SearchForTargets();

        // public ISoldier GetNearestTarget(ISoldier[] soldiers);
        // public ISoldier GetRandomTarget(ISoldier[] soldiers);
        public LayerMask TargetLayerMask { get; }
    }

    public interface IAttackable
    {
        public float AttackRange { get; }
        public float AttackInterval { get; }
        public float Damage { get; }
        [CanBeNull] public IDamageable CurrentTarget { get; }
        public UniTask Attack();
        public CancellationTokenSource AttackCts { get; }
    }

    public class SoldierCombatController : MonoBehaviour, IDamageable, ITargetSearchable, IAttackable
    {
        [Serializable]
        public class Data
        {
            [field: SerializeField] public float AttackInterval { get; set; } = 1f;
            [field: SerializeField] public float Damage { get; set; } = 5f;
            [field: SerializeField] public float Health { get; set; } = 100f;
            [field: SerializeField] public float AttackRange { get; set; } = 1.5f; // Distance to attack
            [field: SerializeField] public float VisibleRange { get; set; } = 3f;
        }

        public float AttackRange => data.AttackRange;
        public float AttackInterval => data.AttackInterval;
        public float Damage => data.Damage;
        public float VisibleRange => data.VisibleRange;
        public float CurrentHealth => data.Health;
        public CancellationTokenSource SearchCts { get; set; }
        public CancellationTokenSource AttackCts { get; set; }
        public IDamageable CurrentTarget { get; set; }
        public LayerMask TargetLayerMask => targetLayerMask;

        [SerializeField] protected SoldierMovementController movementController;
        [SerializeField] private Data data; // Combat data
        [SerializeField] private LayerMask targetLayerMask;

        public void TakeDamage(float damage)
        {
            if (data.Health <= 0) return;

            data.Health -= damage;
            transform.DOKill();
            transform.DOShakeScale(0.2f, 0.4f);
            Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {data.Health}");

            if (data.Health <= 0)
            {
                Die().Forget();
            }
        }

        private void Awake()
        {
            SearchCts = new CancellationTokenSource();
            AttackCts = new CancellationTokenSource();

            SearchForTargets().Forget();
            Attack().Forget();
        }

        public UniTask Die()
        {
            Debug.Log($"{gameObject.name} has died.");
            SearchCts?.Cancel();
            AttackCts?.Cancel();
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        public async UniTask SearchForTargets()
        {
            while (SearchCts is not null && !SearchCts.IsCancellationRequested)
            {
                var possibleTargets = Physics.OverlapSphere(transform.position, data.VisibleRange, targetLayerMask);

                if (possibleTargets.Length > 0)
                {
                    // Process found targets
                    foreach (var target in possibleTargets)
                    {
                        if (!target.TryGetComponent(out IDamageable damageableSoldier)) continue;
                        CurrentTarget = damageableSoldier;
                        break;
                    }
                }

                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: SearchCts.Token);
            }
        }

        public async UniTask Attack()
        {
            while (AttackCts is not null && !AttackCts.IsCancellationRequested)
            {
                if (CurrentTarget == null || CurrentTarget.CurrentHealth <= 0)
                {
                    Debug.Log($"{gameObject.name} has no valid target to attack.");
                    await UniTask.Delay(TimeSpan.FromSeconds(data.AttackInterval), cancellationToken: AttackCts.Token);
                    continue;
                }

                await PerformAttack();

                await UniTask.Delay(TimeSpan.FromSeconds(data.AttackInterval), cancellationToken: AttackCts.Token);
            }
        }

        protected virtual async UniTask PerformAttack()
        {
        }
    }
}