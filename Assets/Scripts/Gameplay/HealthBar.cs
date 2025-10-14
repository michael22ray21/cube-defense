using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image _fillImage;
    private Monster _monster;
    // private Canvas _canvas;

    public void Initialize(Monster monster)
    {
        _monster = monster;
        Image _bgImage = GetComponentInChildren<Image>();
        _fillImage = _bgImage.GetComponentInChildren<Image>();
        _fillImage.type = Image.Type.Filled;
        _fillImage.fillMethod = Image.FillMethod.Horizontal;
        // _canvas = GetComponentInParent<Canvas>();
        transform.position = _monster.transform.position + Vector3.up;

        if (_fillImage == null)
        {
            Debug.LogError("HealthBar needs a child Image component!");
        }

        _monster.OnTakeDamage += UpdateHealthBar;
        _monster.OnDeath += DestroyObject;
    }

    private void UpdateHealthBar()
    {
        float healthPercent = (float)_monster.CurrentHealth / _monster.MaxHealth;
        healthPercent = Mathf.Clamp01(healthPercent);
        Debug.Log(healthPercent);
        _fillImage.fillAmount = healthPercent;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
