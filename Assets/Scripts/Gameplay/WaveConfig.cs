using System;
using UnityEngine;

[Serializable]
public class WaveConfig
{
    #region Vars, Fields, Getters
    [SerializeField] private MonsterSpawnData[] _monsterSpawns;
    [SerializeField] private float _waveDuration;

    public MonsterSpawnData[] MonsterSpawns => _monsterSpawns;
    public float WaveDuration => _waveDuration;

    [Serializable]
    public class MonsterSpawnData
    {
        [SerializeField] private MonsterType _monsterType;
        [SerializeField] private float _spawnTime;

        public MonsterType MonsterType => _monsterType;
        public float SpawnTime => _spawnTime;
    }
    #endregion
}
