using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unit.Soldier
{
    public class SoldierMovementController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stoppingDistance = 0.2f; // Used in combat, not formation
        [SerializeField] private float noiseAmplitude = 0.2f;
        [SerializeField] private float noiseFrequency = 0.5f;

        private bool _isReady;
        private Vector3 _offsetFromUnit;
        private Transform _unitTransform;
        private float _delayTime;
        private Vector2 _noiseSeed;
        private bool _followFormation = true;

        public float GetMoveSpeed() => moveSpeed;
        public float GetStoppingDistance() => stoppingDistance;
        public void SetFollowFormation(bool follow) => _followFormation = follow;

        private Vector3 TargetTransform()
        {
            Vector3 target = _unitTransform.localPosition + _offsetFromUnit;

            // Add Perlin noise to create natural deviation
            float time = Time.time * noiseFrequency + _delayTime; // Incorporate delay for variation
            float noiseX = Mathf.PerlinNoise(time + _noiseSeed.x, _noiseSeed.y) * 2 - 1; // Range [-1, 1]
            float noiseZ = Mathf.PerlinNoise(_noiseSeed.x, time + _noiseSeed.y) * 2 - 1; // Range [-1, 1]
            Vector3 noiseOffset = new Vector3(noiseX, 0, noiseZ) * noiseAmplitude;

            return target + noiseOffset;
        }

        public void Init(Vector3 offset, Transform army)
        {
            _offsetFromUnit = offset;
            _unitTransform = army;
            _isReady = true;
            _noiseSeed = new Vector2(Random.value * 100, Random.value * 100);
            moveSpeed *= Random.Range(0.9f, 1.1f);
        }

        public async UniTask MoveTowards(Vector3 target, float range)
        {
            if (!_isReady || !_unitTransform) return;

            _followFormation = false;
            while (Vector3.Distance(transform.position, target) > range)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
                await UniTask.Yield();
            }
        }

        private void Update()
        {
            if (!_isReady || !_unitTransform || !_followFormation) return;

            Vector3 targetPosition = TargetTransform();
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }

        private void OnDrawGizmos()
        {
            if (_isReady && _unitTransform != null && _followFormation)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, TargetTransform());

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(TargetTransform(), stoppingDistance);
            }
        }
    }
}