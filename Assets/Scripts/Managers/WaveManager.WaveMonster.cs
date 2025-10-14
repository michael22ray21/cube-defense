using System;
using UnityEngine;

public partial class WaveManager
{
    [Serializable]
    public class WaveMonster
    {
        [SerializeField] private GameObject _monsterPrefab;
        [SerializeField] private float _spawnDelay;

        public GameObject MonsterPrefab => _monsterPrefab;
        public float SpawnDelay => _spawnDelay;
    }
}