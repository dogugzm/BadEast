using UnityEngine;

namespace Unit.Soldier
{
    public class SoldierMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stoppingDistance = 0.1f;
        [SerializeField] private float noiseAmplitude = 0.2f;
        [SerializeField] private float noiseFrequency = 0.5f;

        private bool _isReady;
        private Vector3 _offsetFromArmy;
        private Transform _armyTransform;
        private float _delayTime;
        private Vector2 _noiseSeed;

        private Vector3 TargetTransform()
        {
            Vector3 target = _armyTransform.localPosition + _offsetFromArmy;

            // Add Perlin noise to create natural deviation
            float time = Time.time * noiseFrequency + _delayTime; // Incorporate delay for variation
            float noiseX = Mathf.PerlinNoise(time + _noiseSeed.x, _noiseSeed.y) * 2 - 1; // Range [-1, 1]
            float noiseZ = Mathf.PerlinNoise(_noiseSeed.x, time + _noiseSeed.y) * 2 - 1; // Range [-1, 1]
            Vector3 noiseOffset = new Vector3(noiseX, 0, noiseZ) * noiseAmplitude;

            return target + noiseOffset;
        }

        public void Init(Vector3 offset, Transform army)
        {
            _offsetFromArmy = offset;
            _armyTransform = army;
            _isReady = true;
            _noiseSeed = new Vector2(Random.value * 100, Random.value * 100);
            moveSpeed *= Random.Range(0.9f, 1.1f);
        }

        private void Update()
        {
            if (!_isReady || !_armyTransform) return;

            Vector3 targetPosition = TargetTransform();

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
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