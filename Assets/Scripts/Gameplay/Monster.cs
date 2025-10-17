using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Monster : MonoBehaviour
{
    #region Vars, Fields, Getters
    [Title("Editor")]
    [SerializeField] private bool _showDebug = false;

    [Title("References")]
    // [SerializeField] private GameObject _healthBarPrefab;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _healthBar;

    [Title("Parameters")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _moneyReward = 10;

    private TDManager _tdManager;
    private bool _healthBarShown = false;
    private int _currentHealth;
    private Transform[] _pathPoints; // these path points are to direct the monster movements
    private int _currentPathIndex = 0;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    public Action OnTakeDamage;
    public Action OnDeath;
    #endregion

    #region Behavior
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SetCurrentHealth();
        CheckTDManager();
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
    private void SetCurrentHealth()
    {
        _currentHealth = _maxHealth;
    }

    private void CheckTDManager()
    {
        if (_tdManager == null)
        {
            _tdManager = FindFirstObjectByType<TDManager>();
        }
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

        // Vector3.MoveTowards
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
        _tdManager.MoneyManager.AddMoney(_moneyReward);

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