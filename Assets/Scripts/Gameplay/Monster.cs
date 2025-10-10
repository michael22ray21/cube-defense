using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _moneyReward = 10;

    private int _currentHealth;
    private Transform[] _pathPoints; // these path points are to direct the monster movements
    private int _currentPathIndex = 0;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void Initialize(Transform[] pathPoints)
    {
        _pathPoints = pathPoints;
        _currentPathIndex = 0;

        if (_pathPoints.Length > 0)
        {
            transform.position = _pathPoints[0].position;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_pathPoints == null || _pathPoints.Length == 0) return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (_currentPathIndex >= _pathPoints.Length)
        {
            ReachedEnd();
            return;
        }

        Transform targetPoint = _pathPoints[_currentPathIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;

        transform.position += _moveSpeed * Time.deltaTime * direction;

        // Check if we reached the current waypoint
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            _currentPathIndex++;
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Monster died! Cha-ching!");

        // notify the wave manager, the monster has died
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnMonsterDead();
        }

        // destroy the game object
        Destroy(gameObject);
    }

    private void ReachedEnd()
    {
        Debug.Log("Monster breached the base!");

        // damage the base
        PlayerBase playerBase = FindFirstObjectByType<PlayerBase>();
        if (playerBase != null)
        {
            playerBase.TakeDamage(1);
        }

        // notify the wave manager, the monster has died
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnMonsterDead();
        }

        // destroy the game object
        Destroy(gameObject);
    }
}