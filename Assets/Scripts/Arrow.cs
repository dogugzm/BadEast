using Unit;
using Unit.Soldier;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float arrowSpeed = 10f;

    [SerializeField]
    private float baseArcHeightMultiplier = 0.1f; // A multiplier to determine arc height based on distance.

    private float _damage;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _journeyLength;
    private float _startTime;
    private float _calculatedArcHeight;
    private UnitSide _targetSide;

    /// <summary>
    /// Initializes the arrow with target and damage information.
    /// </summary>
    /// <param name="damage">The amount of damage to inflict.</param>
    /// <param name="targetPosition">The final destination of the arrow.</param>
    /// <param name="targetSide"></param>
    public void Initialize(float damage, Vector3 targetPosition, UnitSide targetSide)
    {
        _damage = damage;
        _startPosition = transform.position;
        _targetPosition = targetPosition;
        _journeyLength = Vector3.Distance(_startPosition, _targetPosition);
        _startTime = Time.time;

        _calculatedArcHeight = _journeyLength * baseArcHeightMultiplier;
    }
    
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float flightTime = 1f;

    private void Update()
    {
        float elapsed = Time.time - _startTime;
        float t = elapsed / flightTime;

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 currentPosition = Vector3.Lerp(_startPosition, _targetPosition, t);

        // parabole ekle
        float height = 4 * _calculatedArcHeight * t * (1 - t); // klasik parabol
        currentPosition.y += height;

        Vector3 previousPosition = transform.position;
        transform.position = currentPosition;

        Vector3 direction = (currentPosition - previousPosition).normalized;
        if (direction != Vector3.zero)
            transform.forward = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable damageable)) return;

        if (!damageable.transform.TryGetComponent(out ISoldier soldier)) return;

        if (soldier.Side == _targetSide) return;

        Debug.Log("Arrow hit: " + other.name);
        damageable.TakeDamage(_damage);
    }
}