using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.GameManagement;
using _Project.Code.Gameplay.Projectiles;
using UnityEngine;
using UnityEngine.UI;

public class BossArm : MonoBehaviour
{
    private EColor _color;
    private EColor _requiredColor;
    private SpriteRenderer[] _spriteRenderers;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthBarImage;
    private bool _wasHitByColorBomb = false;
    private bool _wasHitByFullyChargedColorBomb = false;
    private bool _hasBeenInitialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _color = EColor.White;

        _wasHitByColorBomb = false;
        _wasHitByFullyChargedColorBomb = false;

        if (!_hasBeenInitialized)
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _hasBeenInitialized = true;
        }

        healthBar.value = 0.0f;

        foreach (SpriteRenderer sr in _spriteRenderers)
        {
            sr.color = ServiceLocator.Get<GameManagementService>().EColorToColor(_color);
        }

    }

    public void Reset()
    {
        Initialize();
    }

    public void SetRequiredColor(EColor color)
    {
        _requiredColor = color;
    }

    public bool IsComplete()
    {
        if (healthBar.value == 1.0f && _requiredColor == _color)
            return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ProjectileBase projectile))
        {
            if (collision.gameObject.TryGetComponent(out ColorBomb colorBomb))
            {
                if (colorBomb.WasFullCharged)
                    _wasHitByFullyChargedColorBomb = true;
                else
                    _wasHitByColorBomb = true;
            }
            else
            {
                _wasHitByColorBomb = false;
                _wasHitByFullyChargedColorBomb = false;
            }


            Color.RGBToHSV(collision.gameObject.GetComponent<SpriteRenderer>().color, out float h, out float s, out float v);

            if (s == 0.0f)
            {
                healthBar.value = Mathf.Clamp(healthBar.value - 0.025f, 0.0f, 1.0f);

                if (healthBar.value <= 0.05f)
                    healthBar.value = 0.0f;

                if (healthBar.value == 0.0f)
                    _color = EColor.White;
            }
            else if (_color == EColor.White)
            {
                if (h <= 0.0f)
                    _color = EColor.Red;
                else if (h <= 0.2f)
                    _color = EColor.Yellow;
                else if (h >= 0.5f)
                    _color = EColor.Blue;

                healthBarImage.color = ServiceLocator.Get<GameManagementService>().EColorToColor(_color);
                IncreaseBar();
            }
            else if (h <= 0.0f && _color == EColor.Red)
            {
                healthBarImage.color = ServiceLocator.Get<GameManagementService>().EColorToColor(_color);
                IncreaseBar();
            }
            else if (h <= 0.2f && _color == EColor.Yellow)
            {
                healthBarImage.color = ServiceLocator.Get<GameManagementService>().EColorToColor(_color);
                IncreaseBar();
            }
            else if (h >= 0.5f && _color == EColor.Blue)
            {
                healthBarImage.color = ServiceLocator.Get<GameManagementService>().EColorToColor(_color);
                IncreaseBar();
            }

            if (_requiredColor == _color)
            {
                Color.RGBToHSV(ServiceLocator.Get<GameManagementService>().EColorToColor(_requiredColor), out float newH, out float newS, out float newV);

                foreach (SpriteRenderer sr in _spriteRenderers)
                {
                    sr.color = Color.HSVToRGB(newH, healthBar.value, newV);
                }
            }    
        }
    }

    private void IncreaseBar()
    {
        if (_wasHitByFullyChargedColorBomb)
            healthBar.value = Mathf.Clamp(healthBar.value + 0.5f, 0.0f, 1.0f);
        else if(_wasHitByColorBomb)
            healthBar.value = Mathf.Clamp(healthBar.value + 0.20f, 0.0f, 1.0f);
        else
            healthBar.value = Mathf.Clamp(healthBar.value + 0.005f, 0.0f, 1.0f);


        if (healthBar.value >= 0.95f)
            healthBar.value = 1.0f;
        else if (healthBar.value > 0.0f && healthBar.value < 0.05f)
            healthBar.value = 0.05f;
    }
}
