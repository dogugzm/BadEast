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

    /// <summary>
    /// Initializes the arrow with target and damage information.
    /// </summary>
    /// <param name="damage">The amount of damage to inflict.</param>
    /// <param name="targetPosition">The final destination of the arrow.</param>
    public void Initialize(float damage, Vector3 targetPosition)
    {
        _damage = damage;
        _startPosition = transform.position;
        _targetPosition = targetPosition;
        _journeyLength = Vector3.Distance(_startPosition, _targetPosition);
        _startTime = Time.time;

        // Calculate the arc height dynamically based on the distance.
        // This ensures a more realistic and consistent arc for different distances.
        _calculatedArcHeight = _journeyLength * baseArcHeightMultiplier;
    }

    private void Update()
    {
        // Calculate the current position along the straight line from start to target.
        float distanceCovered = (Time.time - _startTime) * arrowSpeed;
        float journeyFraction = distanceCovered / _journeyLength;

        if (journeyFraction >= 1)
        {
            // We've reached the target, destroy the arrow.
            Destroy(gameObject);
            return;
        }

        // Calculate the parabolic trajectory based on the journey fraction.
        Vector3 currentPosition = Vector3.Lerp(_startPosition, _targetPosition, journeyFraction);

        // Add the Y-axis displacement to simulate the arc.
        // The parabola is highest at the halfway point (journeyFraction = 0.5).
        float yOffset = _calculatedArcHeight * Mathf.Sin(journeyFraction * Mathf.PI);
        currentPosition.y += yOffset;

        // Store the current position to calculate the direction for the next frame.
        Vector3 previousPosition = transform.position;
        transform.position = currentPosition;

        // Make the arrow always point towards its current direction of travel.
        // This ensures the arrow's "nose" faces the direction it is moving,
        // correctly handling the upward and downward arc.
        Vector3 direction = (currentPosition - previousPosition).normalized;
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    // This method is called when the arrow's collider hits another collider.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the IDamageable interface.
        if (other.TryGetComponent(out IDamageable damageable))
        {
            // Apply damage to the target.
            damageable.TakeDamage(_damage);
        }

        // Destroy the arrow after it hits something.
        //Destroy(gameObject);
    }
}