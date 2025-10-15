using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private Image _hpImage;
    [SerializeField] private Image _effectImage;

    [Title("Parameters")]
    [SerializeField] private float _hurtSpeed = 0.001f;

    private Monster _monster;
    // private Canvas _canvas;

    public void Initialize(Monster monster)
    {
        _monster = monster;
        // check for image components
        if (_hpImage == null)
        {
            Debug.Log("Fill Image is not set! Finding component...");
            _hpImage = GameObject.Find("HealthBar").GetComponent<Image>();
        }
        if (_effectImage == null)
        {
            Debug.Log("Effect Image is not set! Finding component...");
            _effectImage = GameObject.Find("EffectFill").GetComponent<Image>();
        }
        // _canvas = GetComponentInParent<Canvas>();

        _monster.OnTakeDamage += UpdateHealthBar;
    }

    private void Update()
    {
        if (_effectImage.fillAmount > _hpImage.fillAmount)
        {
            _effectImage.fillAmount -= _hurtSpeed;
        }
        else
        {
            _effectImage.fillAmount = _hpImage.fillAmount;
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercent = (float)_monster.CurrentHealth / _monster.MaxHealth;
        healthPercent = Mathf.Clamp01(healthPercent);
        _hpImage.fillAmount = healthPercent;
    }
}
