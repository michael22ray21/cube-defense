using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Monster : MonoBehaviour
{
    #region Vars, Fields, Getters
    [Title("Editor")]
    [SerializeField] private bool _showDebug = false;

    [Title("References")]
    [SerializeField] private MonsterType _monsterType;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _healthBar;

    private TDManager _tdManager;
    private bool _healthBarShown = false;
    private int _currentHealth;
    private Transform[] _pathPoints; // these path points are to direct the monster movements
    private int _currentPathIndex = 0;

    public int MaxHealth => _monsterType.MaxHealth;
    public int CurrentHealth => _currentHealth;
    public MonsterType MonsterType => _monsterType;

    public Action OnTakeDamage;
    public Action OnDeath;
    #endregion

    #region Behavior
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        CheckTDManager();
        CheckMonsterType();
        ApplyMonsterTypeProperties();
    }

    public void Initialize(TDManager tdManager, Camera camera, Transform[] pathPoints)
    {
        _tdManager = tdManager;
        _canvas.worldCamera = camera;
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
    #endregion

    #region Utilities
    private void CheckTDManager()
    {
        if (_tdManager == null)
        {
            _tdManager = FindFirstObjectByType<TDManager>();
        }
    }

    private void CheckMonsterType()
    {
        if (_monsterType == null)
        {
            Debug.LogError("[ERR] Monster Type not assigned!");
        }
    }

    private void ApplyMonsterTypeProperties()
    {
        _currentHealth = _monsterType.MaxHealth;
        // Set scale
        transform.localScale = Vector3.one * _monsterType.Scale;

        if (_showDebug) Debug.Log($"Monster spawned: {_monsterType.MonsterName} (Health: {_monsterType.MaxHealth}, Speed: {_monsterType.MoveSpeed}, Armor: {_monsterType.Armor})");
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

        // Vector3.MoveTowards(transform.position, targetPoint.position, _monsterType.MoveSpeed); ;
        transform.position += _monsterType.MoveSpeed * Time.deltaTime * direction;

        // Check if we reached the current waypoint
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            _currentPathIndex++;
        }
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - _monsterType.Armor);
        _currentHealth -= actualDamage;

        if (!_healthBarShown && _healthBar != null)
        {
            _healthBar.GetComponent<HealthBar>().Initialize(this);
            _healthBar.SetActive(true);
            _healthBarShown = true;
        }

        OnTakeDamage?.Invoke();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_showDebug) Debug.Log("Monster died! Cha-ching!");

        // reward money
        _tdManager.MoneyManager.AddMoney(_monsterType.MoneyReward);

        // notify the wave manager, the monster has died
        _tdManager.WaveManager.OnMonsterDead();

        OnDeath?.Invoke();

        // destroy the game object
        Destroy(gameObject);
    }

    private void ReachedEnd()
    {
        if (_showDebug) Debug.Log("Monster breached the base!");

        // damage the base
        _tdManager.PlayerBase.TakeDamage(1);

        // notify the wave manager, the monster has died
        _tdManager.WaveManager.OnMonsterDead();

        OnDeath?.Invoke();

        // destroy the game object
        //^ for future reference, will use pooling
        Destroy(gameObject);
    }
    #endregion
}