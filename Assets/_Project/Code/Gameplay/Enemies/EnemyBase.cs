using System;
using UnityEngine;

using _Project.Code.Core.Pool;


[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour, IDamageable, IPoolable
{
    public event Action OnDestroyed;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private float hitpoints = 10.0f;
    private float _currentHitpoints;
    [SerializeField] private float hpLossOnHit = 1.0f;
    [SerializeField] private int score = 1;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float fireSpeed;

    private bool _hasBeenInitialized = false;


    protected virtual void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_hasBeenInitialized) return;

        _spriteRenderer = GetComponent<SpriteRenderer>();

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.gravityScale = 0.0f;

        _hasBeenInitialized = true;
    }

    public void OnTakeDamage(Color colorOfHitter)
    {
        Color.RGBToHSV(_spriteRenderer.color, out float myH, out float myS, out float myV);
        Color.RGBToHSV(colorOfHitter, out float h, out float s, out float v);

        if (myH == h && s != 0.0f)
        {
            _currentHitpoints -= hpLossOnHit * 4.0f;
        }
        else
        {
            _currentHitpoints -= hpLossOnHit;
        }

        if (_currentHitpoints <= 0.0f)
            Die();
    }

    private void Die()
    {
        // Score Manager add score

        ScoreManager.Instance.AddScore(score);
        OnDestroyed?.Invoke();
    }

    public void OnSpawnFromPool()
    {
        Initialize();


        // Get difficulty modifier from a singleton in scene

        fireSpeed = 0.0f;
        moveSpeed = 0.0f;
        _currentHitpoints = hitpoints;
    }

    public void OnReturnToPool()
    {
    }
}
