using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Vars, Fields, Getters
    [Title("Parameters")]
    [SerializeField] private float _speed = 10f;

    [Title("References")]
    [SerializeField] private GameObject _impactEffectPrefab;

    private Monster _target;
    private int _damage;
    #endregion

    #region Behavior
    public void Initialize(Monster target, int damage, Tower sourceTower)
    {
        _target = target;
        _damage = damage;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_target == null)
        {
            // the target is gone
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (_target.transform.position - transform.position).normalized;
        transform.position += _speed * Time.deltaTime * direction;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
        if (distanceToTarget < 0.2f)
        {
            HitTarget();
        }
    }
    #endregion

    #region Utilities
    private void HitTarget()
    {
        if (_target != null)
        {
            _target.TakeDamage(_damage);
        }

        // Spawn impact effect if assigned
        if (_impactEffectPrefab != null)
        {
            Instantiate(_impactEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
    #endregion
}
