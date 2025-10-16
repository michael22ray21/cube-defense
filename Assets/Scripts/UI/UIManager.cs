using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private TDManager _tdManager;

    [Title("GUI Elements")]
    [SerializeField] private TextMeshProUGUI _waveNumberText;
    [SerializeField] private TextMeshProUGUI _monstersRemainingText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _playerHealthText;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _winUI;

    #region BEHAVIOUR
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        HandleSubscribeToEvents(true);
        // Update the UI immediately for start of game
        UpdateWaveUI();
        UpdateMoneyUI(_tdManager.MoneyManager == null ? _tdManager.MoneyManager.CurrentMoney : 0);
        UpdateHealthUI();
    }

    // Update is called once per frame
    private void Update()
    {
        // these are updated every frame for seamlessness
        UpdateMonstersRemainingUI();
        UpdateHealthUI();
    }
    #endregion

    #region UTILITY
    private void HandleSubscribeToEvents(bool sub)
    {
        if (sub)
        {
            _tdManager.WaveManager.OnWaveStarted += UpdateWaveUI;
            _tdManager.WaveManager.OnAllWavesComplete += ShowWinUI;
            _tdManager.MoneyManager.OnMoneyChanged += UpdateMoneyUI;
            _tdManager.PlayerBase.OnBaseDestroyed += ShowGameOverUI;
        }
        else
        {
            _tdManager.WaveManager.OnWaveStarted -= UpdateWaveUI;
            _tdManager.WaveManager.OnAllWavesComplete -= ShowWinUI;
            _tdManager.MoneyManager.OnMoneyChanged -= UpdateMoneyUI;
            _tdManager.PlayerBase.OnBaseDestroyed -= ShowGameOverUI;
        }
    }

    // wave number UI
    private void UpdateWaveUI()
    {
        if (_waveNumberText != null && _tdManager.WaveManager != null)
        {
            _waveNumberText.text = $"Wave: {_tdManager.WaveManager.CurrentWaveNumber}";
        }
    }

    // Monsters remaining UI
    private void UpdateMonstersRemainingUI()
    {
        if (_monstersRemainingText != null && _tdManager.WaveManager != null)
        {
            _monstersRemainingText.text = $"Monsters remaining: {_tdManager.WaveManager.MonstersAlive}";
        }
    }

    // money UI
    private void UpdateMoneyUI(int amount)
    {
        if (_moneyText != null)
        {
            _moneyText.text = $"Money: ${amount}";
        }
    }

    // health UI
    private void UpdateHealthUI()
    {
        if (_playerHealthText != null && _tdManager.PlayerBase != null)
        {
            _playerHealthText.text = $"Health: {_tdManager.PlayerBase.CurrentHealth}/{_tdManager.PlayerBase.MaxHealth}";
        }
    }

    private void ShowGameOverUI()
    {
        _gameOverUI.SetActive(true);
    }

    private void ShowWinUI()
    {
        _winUI.SetActive(true);
    }
    #endregion
}
