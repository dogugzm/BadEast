using Army.Soldier;
using Formations;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Serialization;

namespace Army
{
    public class UnitMovement : MonoBehaviour
    {
        [FormerlySerializedAs("_soldiers")] [SerializeField]
        private SoldierMovement[] soldiers;

        [SerializeField] private float stoppingDistance = 0.1f;
        [SerializeField] private float speed = 7f;

        private Vector3 _targetPosition;
        private bool _isMoving;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (soldiers == null || soldiers.Length == 0) return;

            var positions =
                BoxFormationHelper.GetPositions(soldiers.Length, transform.position, 2.0f, 1);

            for (int i = 0; i < soldiers.Length; i++)
            {
                if (soldiers[i] == null) continue;
                soldiers[i].Init(positions[i], transform);
            }
        }

        private void Start()
        {
            _targetPosition = transform.position;
        }

        public void SetTarget(Vector3 position)
        {
            var newPos = position;
            newPos.y = transform.position.y;
            _targetPosition = newPos;
            _isMoving = true;
        }

        private void Update()
        {
            if (!_isMoving) return;

            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);

            if (distanceToTarget > stoppingDistance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _targetPosition,
                    speed * Time.deltaTime
                );
            }
            else
            {
                _isMoving = false;
                OnTargetReached();
            }
        }

        private void OnTargetReached()
        {
            if (!TryGetComponent(out ISelectable selectable)) return;
            selectable.Deselect();
        }

        private void OnDrawGizmos()
        {
            if (_isMoving)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_targetPosition, stoppingDistance);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _targetPosition);
            }
        }
    }
}