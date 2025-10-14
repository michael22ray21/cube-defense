using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SetCurrentHealth();
    }

    private void SetCurrentHealth()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log($"The base got damaged. {_currentHealth}/{_maxHealth}");

        if (_currentHealth == 0)
        {
            OnBaseDestroyed();
        }
    }

    private void OnBaseDestroyed()
    {
        Debug.Log("Player Base Destroyed! Game Over!");
    }
}
