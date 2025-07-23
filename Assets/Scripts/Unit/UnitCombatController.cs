using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unit
{
    public class UnitCombatController : MonoBehaviour
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
            _data = new Data(1, 20);
            _cts = new CancellationTokenSource();
            CheckOtherUnitsAround().Forget();
        }

        private void OnDestroy()
        {
            if (_cts == null) return;
            _cts.Cancel();
            _cts = null;
        }

        public void Dispose()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
        }

        private async UniTask CheckOtherUnitsAround()
        {
            while (!_cts.IsCancellationRequested)
            {
                var size = Physics.OverlapSphere(transform.position, _data.CheckUnitRadius,
                    targetLayerMask);

                Debug.Log("Size is " + size.Length + " Checking for other units around: ");
                await UniTask.WaitForSeconds(_data.CheckUnitDelay, cancellationToken: _cts.Token);
            }
        }
    }
}