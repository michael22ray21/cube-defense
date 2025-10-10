using UnityEngine;

public class TDManager : MonoBehaviour
{
    // the instance of this class - This should be the ONLY instance
    private static TDManager _instance;

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
    }

    private void InitializeGame()
    {
        Debug.Log("Game Initialized!");
    }
}
