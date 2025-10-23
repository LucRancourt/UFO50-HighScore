using System;
using UnityEngine;

using UnityEngine.Splines;

using _Project.Code.Core.Pool;
using _Project.Code.Gameplay.Projectiles;
using _Project.Code.Core.Factory;
using _Project.Code.Utilities.Audio;
using _Project.Code.Core.Audio;
using _Project.Code.Gameplay.GameManagement;
using _Project.Code.Core.ServiceLocator;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D))]
[RequireComponent(typeof(SplineAnimate))]
public class EnemyBase : MonoBehaviour, IDamageable, IPoolable
{
    public event Action<EnemyBase> OnDestroyed;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private SplineAnimate _splineAnimate;

    [Header("Not to Change")]
    [SerializeField] private AudioCue fireSFX;
    private PooledFactory<ProjectileBase> _projectilePoolFactory;
    [SerializeField] private ProjectileBase projectilePrefab;
    [SerializeField] private SplineContainer[] splines;

    [Header("Projectile Type")]
    [SerializeField] private ProjectileType projectileType;

    [Header("HP/Score Stats")]
    [SerializeField] private float hitpoints = 10.0f;
    private float _currentHitpoints;
    private float hpLossOnHit = 1.0f;
    [SerializeField] private int score = 1;

    [Header("Default Stats")]
    [SerializeField] private float defaultBulletSpeed;
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private float defaultFireDelay;
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
        _splineAnimate.PlayOnAwake = false;
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


    public void Die()
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
        _fireDelay = defaultFireDelay; // - modifier
    }

    private void Update()
    {
        _fireDelay -= Time.deltaTime;

        if (_fireDelay <= 0.0f)
            FireProjectile();
    }

    public void ColorSwitch(EColor color)
    {
        _spriteRenderer.color = ServiceLocator.Get<GameManagementService>().EColorToColor(color);
    }

    public void SetSplinePath(bool isLeft)
    {
        if (isLeft)
            _splineAnimate.Container = splines[0];
        else
            _splineAnimate.Container = splines[1];
    }

    private void FireProjectile()
    {
        ResetFireDelay();

        ProjectileBase projectile = _projectilePoolFactory.Create(transform.position, transform.rotation);
        projectile.SetDirection();
        projectile.SetSpeed(defaultBulletSpeed);

        AudioManager.Instance.PlaySound(fireSFX);

        projectile.ColorSwitch(_spriteRenderer.color);
        if (!projectile.HasOnHitBeenAdded)
            projectile.OnHit += ReturnProjectile;
    }

    private void ReturnProjectile(ProjectileBase projectile)
    {
        _projectilePoolFactory.Return(projectile);
    }
}
