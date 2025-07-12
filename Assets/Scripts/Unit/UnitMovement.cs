using Army.Soldier;
using Formations;
using UnityEngine;
using UnityEngine.AI; // Add this for NavMesh
using UnityEngine.Serialization;

namespace Army
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : MonoBehaviour
    {
        [FormerlySerializedAs("_soldiers")] [SerializeField]
        private SoldierMovement[] soldiers;

        [SerializeField] private float stoppingDistance = 0.1f;
        [SerializeField] private float speed = 7f;

        private Vector3 _targetPosition;
        private bool _isMoving;

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = stoppingDistance;
            _agent.speed = speed;
            _agent.autoBraking = true;

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
            _targetPosition = position; // No need to flatten y for NavMesh pathfinding
            _isMoving = true;
            _agent.SetDestination(_targetPosition);
        }

        private void Update()
        {
            if (!_isMoving) return;

            if (!_agent.pathPending && _agent.remainingDistance <= stoppingDistance)
            {
                _isMoving = false;
                _agent.ResetPath();
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