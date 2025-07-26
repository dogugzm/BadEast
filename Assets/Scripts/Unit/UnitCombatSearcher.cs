using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unit
{
    public class UnitCombatSearcher : MonoBehaviour
    {
        public class Data
        {
            public float CheckUnitDelay { get; }
            public float CheckUnitRadius { get; }

            public Data(float checkUnitDelay, float checkUnitRadius)
            {
                CheckUnitDelay = checkUnitDelay;
                CheckUnitRadius = checkUnitRadius;
            }
        }

        [SerializeField] private LayerMask targetLayerMask; // it can also be part of data after factory generation

        private CancellationTokenSource _cts;
        private Data _data;
        private Collider[] otherUnits;

        private void Start()
        {
            _data = new Data(1, 10);
            _cts = new CancellationTokenSource();
            CheckOtherUnitsAround().Forget();
        }

        private void OnDestroy()
        {
            if (_cts == null) return;
            _cts.Cancel();
            _cts = null;
        }

        private async UniTask CheckOtherUnitsAround()
        {
            while (!_cts.IsCancellationRequested)
            {
                if (!TryGetComponent(out ICombatController myUnit)) return;

                if (myUnit.CombatStatus is CombatStatus.InCombat) return;

                var otherUnits = Physics.OverlapSphere(transform.position, _data.CheckUnitRadius,
                    targetLayerMask);

                Debug.Log("Found " + otherUnits.Length + " units around " + gameObject.name);
                if (otherUnits.Length > 0)
                {
                    if (otherUnits[0].TryGetComponent(out ICombatController unit))
                    {
                        if (unit.CombatStatus is not CombatStatus.InCombat)
                        {
                            unit.StartCombat(myUnit);
                            myUnit.StartCombat(unit);
                        }
                    }
                }

                await UniTask.WaitForSeconds(_data.CheckUnitDelay, cancellationToken: _cts.Token);
            }
        }
    }
}