using System;
using UnityEngine;

[Serializable]
public class WaveConfig
{
    [SerializeField] private MonsterSpawnData[] _monsterSpawns;
    [SerializeField] private float _waveDuration;

    public MonsterSpawnData[] MonsterSpawns => _monsterSpawns;
    public float WaveDuration => _waveDuration;

    [Serializable]
    public class MonsterSpawnData
    {
        [SerializeField] private GameObject _monsterPrefab;
        [SerializeField] private float _spawnTime;

        public GameObject MonsterPrefab => _monsterPrefab;
        public float SpawnTime => _spawnTime;
    }
}
