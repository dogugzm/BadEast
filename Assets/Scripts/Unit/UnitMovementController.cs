using Formations;
using Unit.Soldier;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Unit
{
    public interface IUnitMovement
    {
        Transform transform { get; }
        void SetTarget(Vector3 position);
        void SetCanMove(bool canMove);
        void ResetMovement();
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovementController : MonoBehaviour, IUnitMovement
    {
        [SerializeField] private float stoppingDistance = 0.1f;
        [SerializeField] private float speed = 7f;

        private Vector3 _targetPosition;

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = stoppingDistance;
            _agent.speed = speed;
            _agent.autoBraking = true;
        }

        private void Start()
        {
            _targetPosition = transform.position;
        }

        public void SetTarget(Vector3 position)
        {
            _targetPosition = position; // No need to flatten y for NavMesh pathfinding
            _agent.ResetPath();
            IsMoving = true;
            _agent.SetDestination(_targetPosition);
        }

        private bool IsMoving { get; set; }

        public void SetCanMove(bool canMove)
        {
            IsMoving = canMove;
            _agent.isStopped = !canMove;
        }

        public void ResetMovement()
        {
            _agent.ResetPath();
        }

        private void Update()
        {
            if (!IsMoving) return;

            if (!_agent.pathPending && _agent.remainingDistance <= stoppingDistance)
            {
                IsMoving = false;
                ResetMovement();
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
            if (IsMoving)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_targetPosition, stoppingDistance);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _targetPosition);
            }
        }
    }
}