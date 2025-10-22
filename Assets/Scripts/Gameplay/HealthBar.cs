using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region Vars, Fields, Getters
    [Title("Parameters")]
    [SerializeField] private float _hurtVisualTime = 1f;
    // [SerializeField] private float _hurtSpeed = 0.001f;

    [Title("References")]
    [SerializeField] private Image _hpImage;
    [SerializeField] private Image _effectImage;

    private Monster _monster;
    private float _hurtVisualVelocity = 0f;
    #endregion;

    #region Behavior
    public void Initialize(Monster monster)
    {
        _monster = monster;
        // check for image components
        if (_hpImage == null)
        {
            Debug.LogError("Fill Image is not set!");
        }
        if (_effectImage == null)
        {
            Debug.LogError("Effect Image is not set!");
        }
        // _canvas = GetComponentInParent<Canvas>();

        _monster.OnTakeDamage += UpdateHealthBar;
    }

    private void Update()
    {
        HandleHurtVisual();
    }

    private void HandleHurtVisual()
    {
        _effectImage.fillAmount = Mathf.SmoothDamp(_effectImage.fillAmount, _hpImage.fillAmount, ref _hurtVisualVelocity, _hurtVisualTime);
    }
    #endregion

    #region Utilities
    private void UpdateHealthBar()
    {
        float healthPercent = (float)_monster.CurrentHealth / _monster.MaxHealth;
        healthPercent = Mathf.Clamp01(healthPercent);
        _hpImage.fillAmount = healthPercent;
    }
    #endregion
}
