using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TDManager : MonoBehaviour
{
    #region Vars, Fields, Getters
    // the instance of this class - This should be the ONLY instance
    private static TDManager _instance;

    [Title("References")]
    [SerializeField] private MoneyManager _moneyManager;
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private PlayerBase _playerBase;

    [Title("Editor")]
    [SerializeField] private bool _showDebug = false;

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
    #endregion

    #region Behavior
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

        _playerBase.OnBaseDestroyed += StopGame;
    }

    private void InitializeGame()
    {
        Debug.Log("Game Initialized!");

        if (_moneyManager == null || _waveManager == null || _playerBase == null)
        {
            Debug.LogError("[ERROR] No instance(s) of required managers!");
        }
    }

    private void StartWaves()
    {
        _waveManager.StartWaves();
    }
    #endregion

    #region Utilities
    private void StopGame()
    {
        Time.timeScale = 0f; // pause the game
    }

    public void RestartGame()
    {
        if (_showDebug) Debug.Log("Restarting Game...");
        Time.timeScale = 1f; // Unpause the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        if (_showDebug) Debug.Log("Exiting Game...");
        Time.timeScale = 1f; // unpause the game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    #endregion
}
