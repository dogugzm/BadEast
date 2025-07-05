using UnityEngine;
using Random = UnityEngine.Random;

namespace Army.Soldier
{
    public class SoldierMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stoppingDistance = 0.1f;

        private bool _isReady;
        private Vector3 _offsetFromArmy;
        private bool _isMoving;
        private Transform _armyTransform;
        private float _delayTime;

        private Vector3 TargetTransform() => _armyTransform.position + _offsetFromArmy;

        public void Init(Vector3 offset, Transform army)
        {
            _offsetFromArmy = offset;
            _armyTransform = army;
            _isReady = true;
            _delayTime = Random.Range(0f, 0.3f);
        }

        private void Update()
        {
            if (!_isReady || !_armyTransform) return;

            Vector3 targetPosition = TargetTransform();
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > stoppingDistance)
            {
                _isMoving = true;
            }

            if (_isMoving)
            {
                if (_delayTime > 0)
                {
                    _delayTime -= Time.deltaTime;
                    return;
                }

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                if (distanceToTarget <= stoppingDistance)
                {
                    _isMoving = false;
                    _delayTime = Random.Range(0f, 0.3f);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_isReady && _armyTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, TargetTransform());

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(TargetTransform(), stoppingDistance);
            }
        }
    }
}