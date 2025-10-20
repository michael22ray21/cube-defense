using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Type", menuName = "Tower Defense/Monster Type")]
public class MonsterType : ScriptableObject
{
    #region Vars, Fields, Getters
    [SerializeField] private string _monsterName;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private int _moneyReward = 10;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Color _monsterColor = Color.white;
    [SerializeField] private float _scale = 1f;
    [SerializeField] private int _armor = 0; // Reduces damage taken

    public string MonsterName => _monsterName;
    public int MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public int MoneyReward => _moneyReward;
    public GameObject Prefab => _prefab;
    public Color MonsterColor => _monsterColor;
    public float Scale => _scale;
    public int Armor => _armor;
    #endregion
}