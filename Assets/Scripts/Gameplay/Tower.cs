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
        if (_currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);
            if (distance <= _range)
            {
                return; // Keep current target
            }
        }

        // Find new target. This function looks for colliders that is inside the sphere (overlaps sphere)
        _currentTarget = null;
        Monster[] allMonsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);

        float closestDistance = Mathf.Infinity;

        // then look for the closest Monster from the tower
        foreach (Monster monster in allMonsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);

            if (distance <= _range && distance < closestDistance)
            {
                closestDistance = distance;
                _currentTarget = monster;
            }
        }
    }

    private void Shoot()
    {
        if (_currentTarget == null) return;

        // this part is supposed to change the direction of the tower is facing
        // Vector3 direction = (_currentTarget.transform.position - transform.position).normalized;
        // Debug.Log(direction);
        // transform.LookAt(transform.position + direction, Vector3.up);

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
