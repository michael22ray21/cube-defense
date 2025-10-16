using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public partial class WaveManager : MonoBehaviour
{
    [Title("Editor")]
    [SerializeField] private bool _showDebug = false;

    [Title("References")]
    [SerializeField] private TDManager _tdManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform[] _pathWaypoints;
    [SerializeField] private WaveConfig[] _waveConfigs;

    [Title("Parameters")]
    [SerializeField] private float _timeBetweenWaves = 5f;

    private int _currentWaveNumber = 0;
    private int _monstersAlive = 0;
    private bool _waveInProgress = false;
    private bool _allWavesComplete = false;

    public int CurrentWaveNumber => _currentWaveNumber;
    public int MonstersAlive => _monstersAlive;
    public float TimeBetweenWaves => _timeBetweenWaves;
    public bool AllWavesComplete => _allWavesComplete;

    public event Action OnWaveStarted;
    public event Action OnAllWavesComplete;

    #region UTILITY
    public void StartWaves()
    {
        StartCoroutine(WaveSequence());
    }

    private void SpawnMonster(GameObject monsterPrefab)
    {
        if (_pathWaypoints == null || _pathWaypoints.Length == 0)
        {
            Debug.LogError("No path waypoints set!");
            return;
        }

        GameObject monsterObj = Instantiate(monsterPrefab, _pathWaypoints[0].position, Quaternion.identity);
        if (monsterObj.TryGetComponent<Monster>(out var monster))
        {
            monster.Initialize(_tdManager, _camera, _pathWaypoints);
            _monstersAlive++;
        }
    }

    public void OnMonsterDead()
    {
        _monstersAlive--;
        if (_monstersAlive < 0) _monstersAlive = 0;
    }
    #endregion

    #region COROUTINE
    private IEnumerator WaveSequence()
    {
        for (int waveIndex = 0; waveIndex < _waveConfigs.Length; waveIndex++)
        {
            // puts a delay in between wave, EVEN before the first wave
            _waveInProgress = false;
            if (_showDebug) Debug.Log($"Wave {_currentWaveNumber} complete. Next wave in {_timeBetweenWaves} seconds..."); //^ logging

            yield return new WaitForSeconds(_timeBetweenWaves);

            // start the new wave
            _currentWaveNumber = waveIndex + 1;
            _waveInProgress = true;
            OnWaveStarted?.Invoke();
            if (_showDebug) Debug.Log($"Wave {_currentWaveNumber} is starting!"); //^ logging

            yield return StartCoroutine(SpawnWave(_waveConfigs[waveIndex]));

            // wait till all monsters are gone
            yield return new WaitUntil(() => _monstersAlive == 0);
        }

        // at this point, all waves are done
        _allWavesComplete = true;
        OnAllWavesComplete?.Invoke();
        if (_showDebug) Debug.Log("All waves completed! Player wins!");
    }

    private IEnumerator SpawnWave(WaveConfig waveConfig)
    {
        foreach (WaveConfig.MonsterSpawnData spawnData in waveConfig.MonsterSpawns)
        {
            // spawn the monster prefab
            SpawnMonster(spawnData.MonsterPrefab);
            yield return new WaitForSeconds(spawnData.SpawnTime);
        }
    }
    #endregion
}
