using System;
using System.Linq;
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
        public CancellationTokenSource SearchCts { get; }
        public UniTask SearchForTargets();
        public Action OnTargetFound { get; }
        public Action OnTargetLost { get; }
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
        public Action OnTargetFound { get; protected set; }
        public Action OnTargetLost { get; protected set; }

        [SerializeField] protected SoldierMovementController movementController;
        [SerializeField] private Data data; // Combat data
        [SerializeField] private LayerMask targetLayerMask;

        private UnitSide _side;

        public void TakeDamage(float damage)
        {
            if (data.Health <= 0) return;

            data.Health -= damage;
            transform.DOKill();
            transform.DOShakeScale(0.2f, 0.4f);

            if (data.Health <= 0)
            {
                Die().Forget();
            }
        }

        private async void Awake()
        {
            if (!TryGetComponent(out ISoldier soldier)) return;
            await UniTask.WaitUntil(() => soldier.IsInitialized,
                cancellationToken: this.GetCancellationTokenOnDestroy());
            _side = soldier.Side;

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
                possibleTargets = possibleTargets.Where(target =>
                        target.TryGetComponent(out ISoldier soldier) && soldier.Side != _side)
                    .ToArray();
                if (possibleTargets.Length > 0)
                {
                    OnTargetFound?.Invoke();

                    // Process found targets randomly
                    var randomIndex = UnityEngine.Random.Range(0, possibleTargets.Length);
                    var randomTarget = possibleTargets[randomIndex];
                    if (randomTarget.TryGetComponent(out IDamageable damageableSoldier))
                    {
                        CurrentTarget = damageableSoldier;
                    }
                }
                else
                {
                    CurrentTarget = null;
                    OnTargetLost?.Invoke();
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
                    await UniTask.Delay(TimeSpan.FromSeconds(data.AttackInterval), cancellationToken: AttackCts.Token);
                    continue;
                }

                await PerformAttack();

                await UniTask.Delay(TimeSpan.FromSeconds(data.AttackInterval), cancellationToken: AttackCts.Token);
            }
        }

        protected void ApplyDamage()
        {
            if (CurrentTarget is null) return;
            if (CurrentTarget.CurrentHealth <= 0) return;
            Debug.Log($"{gameObject.name} attacks {CurrentTarget.transform.name} for {Damage} damage.");
            CurrentTarget.TakeDamage(Damage);
        }


        protected virtual async UniTask PerformAttack()
        {
        }
    }
}