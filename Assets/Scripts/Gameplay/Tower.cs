using Sirenix.OdinInspector;
using UnityEngine;

public class Tower : MonoBehaviour
{
    #region Vars, Fields, Getters
    [Title("References")]
    [SerializeField] private TowerType _towerType;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;

    [Title("Editor")]
    [SerializeField] private bool _showDebug = false;

    private float _fireRate; // Shots per second
    private float _range;
    private int _damage;
    private float _fireCooldown = 0f;
    private Monster _currentTarget;
    #endregion

    #region Behavior
    private void Start()
    {
        SetValuesByTowerType();
        CheckProjectileSpawnPoint();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_towerType == null) return;

        _fireCooldown -= Time.deltaTime;

        FindTarget();

        if (_currentTarget != null && _fireCooldown <= 0f)
        {
            Shoot();
            _fireCooldown = 1f / _fireRate;
        }
    }
    #endregion

    #region Utilities
    private void SetValuesByTowerType()
    {
        if (_towerType == null)
        {
            Debug.LogError("Tower Type not set!");
            return;
        }

        _fireRate = _towerType.FireRate;
        _range = _towerType.Range;
        _damage = _towerType.Damage;

        if (_showDebug) Debug.Log($"Tower spawned: {_towerType.TowerName}");
    }

    private void CheckProjectileSpawnPoint()
    {
        if (_projectileSpawnPoint == null)
        {
            _projectileSpawnPoint = transform;
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
            //^ small optimization, use SqrMagnitude
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
        Vector3 direction = (_currentTarget.transform.position - transform.position).normalized;
        direction.y = 0;
        transform.forward = direction;

        if (_projectilePrefab != null)
        {
            GameObject projectileObj = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);
            if (projectileObj.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Initialize(_currentTarget, _damage, this);
            }
        }
        else
        { // Fallback to instant damage when no projectile prefab set
            if (_showDebug) Debug.Log($"Tower shooting at {_currentTarget.name} for {_damage} damage!");
            _currentTarget.TakeDamage(_damage);
        }
    }
    #endregion

    // preprocessor directive
    #region Editor
#if UNITY_EDITOR
    // Visualize range in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
#endif
    #endregion
}
