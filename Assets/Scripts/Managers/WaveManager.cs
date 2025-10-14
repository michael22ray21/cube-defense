using System.Collections;
using UnityEngine;

public partial class WaveManager : MonoBehaviour
{
    [SerializeField] private TDManager _tdManager;
    [SerializeField] private Transform[] _pathWaypoints;
    [SerializeField] private WaveMonster[] _waveMonsters;
    [SerializeField] private float _timeBetweenWaves = 5f;

    private int _currentWaveNumber = 0;
    private int _monstersAlive = 0;
    private bool _waveInProgress = false;

    public int CurrentWaveNumber => _currentWaveNumber;
    public int MonstersAlive => _monstersAlive;
    public float TimeBetweenWaves => _timeBetweenWaves;

    public event System.Action OnWaveStarted;

    public void StartWaves()
    {
        StartCoroutine(WaveSequence());
    }

    private void SpawnMonster(GameObject monsterPrefab)
    {
        if (_pathWaypoints == null || _pathWaypoints.Length == 0)
        {
            Debug.Log("No path waypoints set!");
            return;
        }

        GameObject monsterObj = Instantiate(monsterPrefab, _pathWaypoints[0].position, Quaternion.identity);
        if (monsterObj.TryGetComponent<Monster>(out var monster))
        {
            monster.Initialize(_tdManager, _pathWaypoints);
            _monstersAlive++;
        }
    }

    public void OnMonsterDead()
    {
        _monstersAlive--;
        if (_monstersAlive < 0) _monstersAlive = 0;
    }

    private IEnumerator WaveSequence()
    {
        while (true)
        {
            // puts a delay in between wave, EVEN before the first wave
            _waveInProgress = false;
            Debug.Log($"Wave {_currentWaveNumber} complete. Next wave in {_timeBetweenWaves} seconds..."); //^ logging

            yield return new WaitForSeconds(_timeBetweenWaves);

            // start the new wave
            _currentWaveNumber++;
            _waveInProgress = true;
            OnWaveStarted?.Invoke();
            Debug.Log($"Wave {_currentWaveNumber} is starting!"); //^ logging

            yield return StartCoroutine(SpawnWave());

            // wait till all monsters are gone
            yield return new WaitUntil(() => _monstersAlive == 0);
        }
    }

    private IEnumerator SpawnWave()
    {
        foreach (WaveMonster waveMonster in _waveMonsters)
        {
            // spawn the monster prefab
            SpawnMonster(waveMonster.MonsterPrefab);
            yield return new WaitForSeconds(waveMonster.SpawnDelay);
        }
    }
}
