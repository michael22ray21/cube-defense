using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TowerSpot : MonoBehaviour
{
    #region Vars, Fields, Getters
    [Title("Parameters")]
    [SerializeField] private int _towerCost = 50;
    [SerializeField] private int _selectedTowerTypeIndex = 0;

    [Title("References")]
    [SerializeField] private TDManager _tdManager;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private TowerType[] _availableTowerTypes;

    [Title("Editor")]
    [SerializeField] private bool _showDebug = false;

    private bool _isOccupied = false;
    private bool _isHovering = false;
    private Material _originalMaterial;
    private Color _originalColor;
    #endregion

    #region Behavior
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalMaterial = _renderer.material;
            _originalColor = _renderer.material.color;
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }
    #endregion

    #region Utilities
    private void PlaceTower(TowerType towerType)
    {
        if (towerType.Prefab == null)
        {
            Debug.LogError("Tower Type has no prefab set yet.");
            return;
        }

        // instantiate a new tower
        GameObject tower = Instantiate(towerType.Prefab, transform.position, Quaternion.identity);
        tower.transform.SetParent(transform);

        _isOccupied = true;

        if (_showDebug) Debug.Log($"Tower deployed. ${_towerCost} incurred!");

        // hide the tower spot, now a tower is deployed here
        if (_renderer != null)
        {
            SetRenderer(false);
        }
    }

    private void ChangeColorToTowerType()
    {
        _renderer.material.color = _availableTowerTypes[_selectedTowerTypeIndex].TowerColor;
    }

    private void ResetMaterialAndColor()
    {
        _renderer.material = _originalMaterial;
        _renderer.material.color = _originalColor;
    }

    private TowerType GetSelectedTowerType()
    {
        if (_availableTowerTypes.Length == 0)
        {
            Debug.LogError("No tower types available!");
            return null;
        }
        return _availableTowerTypes[_selectedTowerTypeIndex];
    }

    private void SetRenderer(bool state)
    {
        _renderer.enabled = state;
    }
    #endregion

    #region Events
    private void HandleMouseInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool isHit = Physics.Raycast(ray, out RaycastHit hit);
        if (isHit && hit.collider.gameObject == gameObject)
        {
            _isHovering = true;
            OnHoverEnter();

            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }

            // Cycle through towers with right click
            if (Input.GetMouseButtonDown(1))
            {
                CycleTowerType();
            }
        }
        else
        {
            if (_isHovering)
            {
                _isHovering = false;
                OnHoverExit();
            }
        }
    }

    // when the spot is clicked
    private void OnClick()
    {
        if (_isOccupied)
        {
            if (_showDebug) Debug.Log("Tower spot occupied!");
            return;
        }

        // money checking
        if (!_tdManager.MoneyManager.TrySpendMoney(_towerCost))
        {
            if (_showDebug) Debug.Log("Not enough money!");
            return;
        }

        PlaceTower(_availableTowerTypes[_selectedTowerTypeIndex]);
    }

    private void CycleTowerType()
    {
        if (_availableTowerTypes.Length == 0) return;

        _selectedTowerTypeIndex = (_selectedTowerTypeIndex + 1) % _availableTowerTypes.Length;
        if (_showDebug) Debug.Log($"Switched to {GetSelectedTowerType().TowerName}");
    }

    // when user hovers over spot
    private void OnHoverEnter()
    {
        if (!_isOccupied && _renderer != null)
        {
            //TODO change this to show a shadow of the tower that's about to be placed
            ChangeColorToTowerType();
        }
    }

    // when user exits the hover
    private void OnHoverExit()
    {
        if (!_isOccupied && _renderer != null)
        {
            ResetMaterialAndColor();
        }
    }
    #endregion
}
