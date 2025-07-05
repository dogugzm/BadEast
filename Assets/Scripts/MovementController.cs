using System;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

namespace DefaultNamespace
{
   

    public class MovementController : MonoBehaviour
    {
        [SerializeField] private SoldierMovement[] _soldiers;
        [SerializeField] private float stoppingDistance = 0.1f;

        private Vector3 _targetPosition;
        private bool _isMoving;

        private void Awake()
        {
            LeanTouch.OnFingerTap += OnFingerTap;
            Initialize();
        }

        private void Initialize()
        {
            if (_soldiers == null || _soldiers.Length == 0) return;

            var positions =
                BoxFormationHelper.GetPositions(_soldiers.Length, transform.position, 2.0f, 1);

            for (int i = 0; i < _soldiers.Length; i++)
            {
                if (_soldiers[i] == null) continue;
                _soldiers[i].Init(positions[i], transform);
            }
        }

        private void Start()
        {
            _targetPosition = transform.position;
        }


        private void OnDestroy()
        {
            LeanTouch.OnFingerTap -= OnFingerTap;
        }

        private void OnFingerTap(LeanFinger touch)
        {
            if (touch.IsOverGui) return;

            var ray = Camera.main.ScreenPointToRay(touch.ScreenPosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var newTarget = hit.point;
                newTarget.y = transform.position.y; // Keep the y position unchanged
                _targetPosition = newTarget;
                _isMoving = true;
            }
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
                    5 * Time.deltaTime
                );
            }
            else
            {
                _isMoving = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (_isMoving)
            {
                // Draw target position
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_targetPosition, stoppingDistance);

                // Draw line to target
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _targetPosition);
            }
        }
    }
}