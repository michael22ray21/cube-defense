using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Type", menuName = "Tower Defense/Tower Type")]
public class TowerType : ScriptableObject
{
    #region Vars, Fields, Getters
    [Title("Parameters")]
    [SerializeField] private string _towerName;
    [SerializeField] private int _cost = 50;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private float _range = 5f;
    [SerializeField] private int _damage = 25;
    [SerializeField] private Color _towerColor = Color.white;

    [Title("References")]
    [SerializeField] private GameObject _prefab;

    public string TowerName => _towerName;
    public int Cost => _cost;
    public float FireRate => _fireRate;
    public float Range => _range;
    public int Damage => _damage;
    public GameObject Prefab => _prefab;
    public Color TowerColor => _towerColor;
    #endregion
}
