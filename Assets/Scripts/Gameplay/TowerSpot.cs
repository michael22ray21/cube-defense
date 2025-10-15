using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TowerSpot : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private GameObject _towerPrefab;
    [SerializeField] private Transform _towerSpawnPoint;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private TDManager _tdManager;

    [Title("Parameters")]
    [SerializeField] private int _towerCost = 50;

    private bool _isOccupied = false;
    private Material _originalMaterial;
    private Color _originalColor;

    #region BEHAVIOUR
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalMaterial = _renderer.material;
            _originalColor = _renderer.material.color;
        }

        if (_towerSpawnPoint == null)
        {
            _towerSpawnPoint = transform;
        }
    }
    #endregion

    #region UTILITY
    private void PlaceTower()
    {
        if (_towerPrefab == null)
        {
            Debug.Log("Tower prefab not set yet.");
            return;
        }

        // instantiate a new tower
        GameObject tower = Instantiate(_towerPrefab, _towerSpawnPoint.position, Quaternion.identity);
        tower.transform.SetParent(transform);

        _isOccupied = true;

        Debug.Log($"Tower deployed. ${_towerCost} incurred!");

        // hide the tower spot, now a tower is deployed here
        if (_renderer != null)
        {
            SetRenderer(false);
        }
    }

    private void ChangeColorGreen()
    {
        _renderer.material.color = Color.green;
    }

    private void ResetMaterialAndColor()
    {
        _renderer.material = _originalMaterial;
        _renderer.material.color = _originalColor;
    }

    private void SetRenderer(bool state)
    {
        _renderer.enabled = state;
    }
    #endregion

    #region EVENT
    // when the spot is clicked
    private void OnMouseDown()
    {
        if (_isOccupied)
        {
            Debug.Log("Tower spot occupied!");
            return;
        }

        // money checking
        if (!_tdManager.MoneyManager.TrySpendMoney(_towerCost))
        {
            Debug.Log("Not enough money!");
            return;
        }

        PlaceTower();
    }

    // when user hovers over spot
    private void OnMouseEnter()
    {
        if (!_isOccupied && _renderer != null)
        {
            ChangeColorGreen();
        }
    }

    // when user exits the hover
    private void OnMouseExit()
    {
        if (!_isOccupied && _renderer != null)
        {
            ResetMaterialAndColor();
        }
    }
    #endregion
}
