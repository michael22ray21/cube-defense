using UnityEngine;

public class TDManager : MonoBehaviour
{
    // the instance of this class - This should be the ONLY instance
    private static TDManager _instance;

    [SerializeField] private MoneyManager _moneyManager;
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private PlayerBase _playerBase;

    public MoneyManager MoneyManager => _moneyManager;
    public WaveManager WaveManager => _waveManager;
    public PlayerBase PlayerBase => _playerBase;

    // get and set functions
    public static TDManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("TDManager not found.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        InitializeGame();
        StartWaves();
    }

    private void InitializeGame()
    {
        Debug.Log("Game Initialized!");

        if (_moneyManager == null || _waveManager == null || _playerBase == null)
        {
            Debug.Log("[ERROR] No instance(s) of required managers!");
        }
    }

    private void StartWaves()
    {
        _waveManager.StartWaves();
    }
}
