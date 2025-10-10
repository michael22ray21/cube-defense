using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float _fireRate = 1f; // Shots per second
    [SerializeField] private float _range = 5f;
    [SerializeField] private int _damage = 25;
    [SerializeField] private LayerMask _enemyLayer;

    private float _fireCooldown = 0f;
    private Monster _currentTarget;

    // Update is called once per frame
    private void Update()
    {
        _fireCooldown -= Time.deltaTime;

        FindTarget();

        if (_currentTarget != null && _fireCooldown <= 0f)
        {
            Shoot();
            _fireCooldown = 1f / _fireRate;
        }
    }

    private void FindTarget()
    {
        // Check if current target is still valid
        if (_currentTarget != null && Vector3.Distance(transform.position, _currentTarget.transform.position) <= _range)
        {
            return; // Keep current target
        }

        // Find new target. This function looks for colliders that is inside the sphere (overlaps sphere)
        _currentTarget = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range, _enemyLayer);

        float closestDistance = Mathf.Infinity;

        // then look for the closest Monster from the tower
        foreach (Collider col in hitColliders)
        {
            if (col.TryGetComponent<Monster>(out var monster))
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _currentTarget = monster;
                }
            }
        }
    }

    private void Shoot()
    {
        if (_currentTarget == null) return;

        Debug.Log($"Tower shooting at {_currentTarget.name} for {_damage} damage!");
        _currentTarget.TakeDamage(_damage);
    }

    // Visualize range in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}
