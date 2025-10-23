using UnityEngine;

using _Project.Code.Core.Factory;
using _Project.Code.Gameplay.Input;
using _Project.Code.Gameplay.Player;
using _Project.Code.Gameplay.PlayerController._Base;
using _Project.Code.Gameplay.PlayerController._Profile;
using _Project.Code.Gameplay.PlayerController.Drone.States;
using _Project.Code.Gameplay.Projectiles;

using _Project.Code.Core.ServiceLocator;
using _Project.Code.Core.StateMachine;
using _Project.Code.Gameplay.GameManagement;
using _Project.Code.Core.Events;
using _Project.Code.Utilities.Audio;
using _Project.Code.Core.Audio;

namespace _Project.Code.Gameplay.PlayerController.Drone
{
    [RequireComponent(typeof(CharacterControllerMotor), typeof(SpriteRenderer))]
    public class DroneController : BasePlayerController, IDamageable
    {
        [Header("PlayerController Settings")]
        [field: SerializeField] public DroneMovementProfile MovementProfile { get; private set; }

        private CharacterControllerMotor _motor;
        private PlayerService _playerService;

        private int _maxHealth = 4;
        private int _currentHealth;
        private Vector3 _startPos;

        private bool _wasRecentlyHit = false;
        [SerializeField] private float invincibilityTimer = 3.0f;
        private float _currentInvicibilityTimer;

        public CharacterControllerMotor Motor => _motor;
        public Vector2 MoveInput { get; set; }


        // Shooter specific properties
        [SerializeField] private GameObject[] hearts;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private AudioCue fireSFX;
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private ProjectileBase colorBombPrefab;
        [SerializeField] private float shootDelay = 0.5f;
        private float _currentShootDelay;
        private PooledFactory<ProjectileBase> _projectilePoolFactory;
        private PooledFactory<ProjectileBase> _colorBombPoolFactory;

        private ParticleSystem _chargePFX;
        private bool _hasFiredHalfCharge;
        private bool _hasFiredFullCharge;

        private EColor _color;


        // Drone specific properties
        public float ForwardInput => MoveInput.y;
        public float RightInput => MoveInput.x;

        public bool IsMovingForward => ForwardInput > ServiceLocator.Get<InputService>().Profile.DirectionMagnitudeThreshold;
        public bool IsMovingBackward => ForwardInput < -ServiceLocator.Get<InputService>().Profile.DirectionMagnitudeThreshold;
        public bool IsMovingRight => RightInput > ServiceLocator.Get<InputService>().Profile.DirectionMagnitudeThreshold;
        public bool IsMovingLeft => RightInput < -ServiceLocator.Get<InputService>().Profile.DirectionMagnitudeThreshold;


        private void Awake()
        {
            _motor = GetComponent<CharacterControllerMotor>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _chargePFX = GetComponent<ParticleSystem>();

            _currentShootDelay = shootDelay;

            _startPos = transform.position;

        }

        protected override void Start()
        {
            base.Start();

            _projectilePoolFactory = new PooledFactory<ProjectileBase>(projectilePrefab);
            _colorBombPoolFactory = new PooledFactory<ProjectileBase>(colorBombPrefab);

            _playerService = ServiceLocator.Get<PlayerService>();
            _playerService.RegisterPlayer(this);

            EventBus.Instance.Subscribe<GameStateChangedEvent>(this, Reset);
        }


        public void OnTakeDamage(Color colorOfHitter)
        {
            if (_wasRecentlyHit) return;

            _wasRecentlyHit = true;

            InvokeRepeating("AnimateIFrames", 0.2f, 0.2f);

            _currentHealth--;

            if (_currentHealth <= 0)
                ServiceLocator.Get<GameManagementService>().TransitionToMenu();
            else
                hearts[_currentHealth-1].SetActive(false);
        }

        public override void Initialize()
        {
            var idleState = new DroneIdleState(this);
            StateMachine = new FiniteStateMachine<IState>(idleState);

            StateMachine.AddState(new DroneMovingState(this));
        }

        private void Reset(GameStateChangedEvent gameState)
        {
            if (gameState.StateName == "Gameplay")
            {
                _color = EColor.White;
                _spriteRenderer.color = ServiceLocator.Get<GameManagementService>().EColorToColor(_color);

                _currentHealth = _maxHealth;
                transform.position = _startPos;
                _currentInvicibilityTimer = invincibilityTimer;

                foreach (GameObject heart in hearts)
                {
                    heart.SetActive(true);
                }

                _chargePFX.Stop();

                _hasFiredHalfCharge = false;
                _hasFiredFullCharge = false;
            }
        }

        protected override void Update()
        {
            base.Update();

            Color.RGBToHSV(_spriteRenderer.color, out float h, out float s, out float v);

            if (s >= 0.5f && !_hasFiredHalfCharge)
            {
                var pfx = _chargePFX.main;
                pfx.startColor = ServiceLocator.Get<GameManagementService>().EColorToColor(_color);
                pfx.startSpeed = 3.5f;
                pfx.startSize = 1.0f;

                var emission = _chargePFX.emission;
                emission.rateOverTime = 25.0f;

                _chargePFX.Play();
                _hasFiredHalfCharge = true;
            }
            else if (s == 1.0f && !_hasFiredFullCharge)
            {
                var pfx = _chargePFX.main;
                pfx.startColor = ServiceLocator.Get<GameManagementService>().EColorToColor(_color);
                pfx.startSpeed = 5.0f;
                pfx.startSize = 2.0f;

                var emission = _chargePFX.emission;
                emission.rateOverTime = 50.0f;

                _chargePFX.Play();
                _hasFiredFullCharge = true;
            }

            _currentShootDelay -= Time.deltaTime;

            if (_wasRecentlyHit)
            {
                _currentInvicibilityTimer -= Time.deltaTime;
            }

            if (_currentInvicibilityTimer <= 0.0f)
            {
                CancelInvoke();
                _spriteRenderer.enabled = true;
                _currentInvicibilityTimer = invincibilityTimer;
                _wasRecentlyHit = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out EnemyBase enemy))
            {
                enemy.Die();
                OnTakeDamage(Color.white);
            }
        }

        private void AnimateIFrames()
        {
            _spriteRenderer.enabled = !_spriteRenderer.enabled;
        }

        public Vector3 GetForwardDirection()
        {
            return transform.up;
        }

        public Vector3 GetBackwardDirection()
        {
            return -transform.up;
        }

        public Vector3 GetRightDirection()
        {
            return transform.right;
        }

        public Vector3 GetLeftDirection()
        {
            return -transform.right;
        }

        public void ColorSwitch(Color color)
        {
            _spriteRenderer.color = color;
        }

        public void FireProjectile()
        {
            if (_currentShootDelay > 0.0f) return;

            _currentShootDelay = shootDelay;

            ProjectileBase projectile = _projectilePoolFactory.Create(transform.position, transform.rotation);
            projectile.SetDirection();
            projectile.ColorSwitch(_spriteRenderer.color);
            if (!projectile.HasOnHitBeenAdded)
            {
                projectile.OnHitForPlayer += ReturnProjectileThatHitEnemy;
                projectile.OnHit += ReturnProjectile;
            }

            AudioManager.Instance.PlaySound(fireSFX);
        }

        public void FireSecondaryProjectile()
        {
            Color.RGBToHSV(_spriteRenderer.color, out float h, out float s, out float v);

            if (s >= 0.5f)
            {
                ProjectileBase projectile = _colorBombPoolFactory.Create(transform.position, transform.rotation);

                if (projectile.TryGetComponent(out ColorBomb bomb))
                {
                    if (s == 1.0f)
                        bomb.WasFullCharged = true;
                }

                projectile.SetDirection();
                projectile.ColorSwitch(_spriteRenderer.color);
                if (!projectile.HasOnHitBeenAdded)
                    projectile.OnHit += ReturnColorBomb;

                AudioManager.Instance.PlaySound(fireSFX);

                _chargePFX.Stop();
                _hasFiredHalfCharge = false;
                _hasFiredFullCharge = false;
            }

            _color = EColor.White;
            _spriteRenderer.color = ServiceLocator.Get<GameManagementService>().EColorToColor(EColor.White);
        }

        private void ReturnProjectile(ProjectileBase projectile)
        {
            _projectilePoolFactory.Return(projectile);
        }
        private void ReturnColorBomb(ProjectileBase projectile)
        {
            _colorBombPoolFactory.Return(projectile);
        }

        private void ReturnProjectileThatHitEnemy(ProjectileBase projectile, EColor color)
        {
            ReturnProjectile(projectile);

            if (color == EColor.White) return;

            Color.RGBToHSV(_spriteRenderer.color, out float h, out float s, out float v);

            if (s == 0.0f || color == _color)
            {
                _color = color;

                Color.RGBToHSV(ServiceLocator.Get<GameManagementService>().EColorToColor(color), out float newH, out float garbageS, out float garbageV);

                s = Mathf.Clamp(s + 0.05f, 0.0f, 1.0f);

                if (s > 0.9f)
                    s = 1.0f;
                else if (s < 0.15f)
                    s = 0.15f;

                _spriteRenderer.color = Color.HSVToRGB(newH, s, v);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _projectilePoolFactory.Clear();

            _playerService?.UnregisterPlayer(this);
        }
    }
}