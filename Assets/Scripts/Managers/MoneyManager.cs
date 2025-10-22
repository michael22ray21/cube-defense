using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    #region Vars, Fields, Getters
    [Title("Parameters")]
    [SerializeField] private int _startingMoney = 50;

    [Title("Editor")]
    [SerializeField] private bool _showDebug = false;

    private int _currentMoney;

    public int CurrentMoney => _currentMoney;
    public event Action<int> OnMoneyChanged;
    #endregion

    #region Behavior
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _currentMoney = _startingMoney;
        OnMoneyChanged?.Invoke(_currentMoney);
        if (_showDebug) Debug.Log($"Starting money: ${_currentMoney}");
    }
    #endregion

    #region Utilities
    // function to spend money
    public bool TrySpendMoney(int amount)
    {
        if (_currentMoney >= amount)
        {
            _currentMoney -= amount;
            OnMoneyChanged?.Invoke(_currentMoney);
            if (_showDebug) Debug.Log($"${amount} spent! Balance: ${_currentMoney}");
            return true;
        }

        if (_showDebug) Debug.Log($"Not enough money! Have {_currentMoney}/{amount}");
        return false;
    }

    // gain money
    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        OnMoneyChanged?.Invoke(_currentMoney);
        if (_showDebug) Debug.Log($"Gained ${amount}. Balance ${_currentMoney}");
    }
    #endregion
}
