using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unit.Soldier;
using UnityEngine;

namespace Unit.Spearman
{
    public class SpearmanSoldierCombatController : SoldierCombatController
    {
        [SerializeField] private Transform spearTransform;

        private Vector3 initialSpearRotation;

        private void OnEnable()
        {
            OnTargetFound += OnTargetFoundSpearman;
            OnTargetLost += OnTargetLostSpearman;
            initialSpearRotation = spearTransform.localEulerAngles;
        }

        private void OnTargetLostSpearman()
        {
            spearTransform.DORotate(initialSpearRotation, .2f);
            spearTransform.DOLocalMoveZ(0f, 0.2f);
        }

        private void OnTargetFoundSpearman()
        {
            if (CurrentTarget == null) return;

            //rotate towards the target
            var direction = (CurrentTarget.transform.position - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(direction);
            transform.DORotateQuaternion(targetRotation, 0.2f);
            spearTransform.DORotateQuaternion(targetRotation, 0.4f);
        }

        protected override async UniTask PerformAttack()
        {
            if (CurrentTarget == null) return;
            //TODO: movement control will be added later

            if (Vector3.Distance(transform.position, CurrentTarget.transform.position) > AttackRange) return;

            spearTransform.DOLocalMoveZ(1f, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                spearTransform.DOLocalMoveZ(0f, 0.2f).SetEase(Ease.OutQuad);
            });

            ApplyDamage();
        }
    }
}