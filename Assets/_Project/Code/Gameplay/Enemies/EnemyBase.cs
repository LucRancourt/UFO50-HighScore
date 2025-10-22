using System;
using UnityEngine;

using UnityEngine.Splines;

using _Project.Code.Core.Pool;
using _Project.Code.Gameplay.Projectiles;
using _Project.Code.Core.Factory;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D))]
[RequireComponent(typeof(SplineAnimate))]
public class EnemyBase : MonoBehaviour, IDamageable, IPoolable
{
    public event Action<EnemyBase> OnDestroyed;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private SplineAnimate _splineAnimate;

    private PooledFactory<ProjectileBase> _projectilePoolFactory;
    [SerializeField] private ProjectileBase projectilePrefab;
    [SerializeField] private ProjectileType projectileType;

    [SerializeField] private float hitpoints = 10.0f;
    private float _currentHitpoints;
    [SerializeField] private float hpLossOnHit = 1.0f;
    [SerializeField] private int score = 1;

    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private float defaultFireSpeed;
    private float _fireDelay;

    private bool _hasBeenInitialized = false;


    protected virtual void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_hasBeenInitialized) return;

        projectilePrefab.SetProjectTileType(projectileType);
        _projectilePoolFactory = new PooledFactory<ProjectileBase>(projectilePrefab);

        _splineAnimate = GetComponent<SplineAnimate>();
        _splineAnimate.Alignment = SplineAnimate.AlignmentMode.None;
        _splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
        //_splineAnimate.PlayOnAwake = false;
        _splineAnimate.Loop = SplineAnimate.LoopMode.Once;
        _splineAnimate.Completed += DieWithoutScore;

        _spriteRenderer = GetComponent<SpriteRenderer>();

        transform.rotation = Quaternion.Euler(Vector3.zero);

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
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
        OnDestroyed?.Invoke(this);
    }

    private void DieWithoutScore()
    {
        OnDestroyed?.Invoke(this);
    }

    public void OnSpawnFromPool()
    {
        Initialize();


        // Get difficulty modifier from a singleton in scene

        _splineAnimate.Restart(false);

        ResetFireDelay();
        _splineAnimate.MaxSpeed = defaultMoveSpeed;  // + or * multiplier (probs +)

        _currentHitpoints = hitpoints;

        _splineAnimate.Play();
    }

    public void OnReturnToPool()
    {
    }

    private void ResetFireDelay()
    {
        _fireDelay = defaultFireSpeed; // - modifier
    }

    private void Update()
    {
        _fireDelay -= Time.deltaTime;

        if (_fireDelay <= 0.0f)
            FireProjectile();
    }

    private void FireProjectile()
    {
        ResetFireDelay();

        ProjectileBase projectile = _projectilePoolFactory.Create(transform.position, transform.rotation);
        projectile.SetDirection();

        projectile.ColorSwitch(_spriteRenderer.color);
        if (!projectile.HasOnHitBeenAdded)
            projectile.OnHit += ReturnProjectile;
    }

    private void ReturnProjectile(ProjectileBase projectile)
    {
        _projectilePoolFactory.Return(projectile);
    }
}
