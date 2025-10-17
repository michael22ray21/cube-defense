using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    #region Vars, Fields, Getters
    [Title("Editor")]
    [SerializeField] private bool _showDebug = false;

    [Title("Parameters")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    public Action OnBaseDestroyed;
    #endregion

    #region Behavior
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SetCurrentHealth();
    }
    #endregion

    #region Utilities
    private void SetCurrentHealth()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_showDebug) Debug.Log($"The base got damaged. {_currentHealth}/{_maxHealth}");

        if (_currentHealth == 0)
        {
            BaseDestroyed();
        }
    }

    private void BaseDestroyed()
    {
        OnBaseDestroyed?.Invoke();
        if (_showDebug) Debug.Log("Player Base Destroyed! Game Over!");
    }
    #endregion
}
