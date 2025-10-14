using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [Title("Parameters")]
    [SerializeField] private int _startingMoney = 50;
    private int _currentMoney;

    public int CurrentMoney => _currentMoney;
    public event Action<int> OnMoneyChanged;

    #region BEHAVIOUR
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _currentMoney = _startingMoney;
        OnMoneyChanged?.Invoke(_currentMoney);
        Debug.Log($"Starting money: ${_currentMoney}");
    }
    #endregion

    #region UTILITY
    // function to spend money
    public bool TrySpendMoney(int amount)
    {
        if (_currentMoney >= amount)
        {
            _currentMoney -= amount;
            OnMoneyChanged?.Invoke(_currentMoney);
            Debug.Log($"${amount} spent! Balance: ${_currentMoney}");
            return true;
        }

        Debug.Log($"Not enough money! Have {_currentMoney}/{amount}");
        return false;
    }

    // gain money
    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        OnMoneyChanged?.Invoke(_currentMoney);
        Debug.Log($"Gained ${amount}. Balance ${_currentMoney}");
    }
    #endregion
}
