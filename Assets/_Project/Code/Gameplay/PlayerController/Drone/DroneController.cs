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

        private int _maxHealth = 3;
        private int _currentHealth;
        private Vector3 _startPos;

        private bool _wasRecentlyHit = false;
        [SerializeField] private float invincibilityTimer = 3.0f;
        private float _currentInvicibilityTimer;

        public CharacterControllerMotor Motor => _motor;
        public Vector2 MoveInput { get; set; }


        // Shooter specific properties
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private AudioCue fireSFX;
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private float shootDelay = 0.5f;
        private float _currentShootDelay;
        private PooledFactory<ProjectileBase> _projectilePoolFactory;


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

            _currentShootDelay = shootDelay;

            _startPos = transform.position;

        }

        protected override void Start()
        {
            base.Start();

            _projectilePoolFactory = new PooledFactory<ProjectileBase>(projectilePrefab);

            _playerService = ServiceLocator.Get<PlayerService>();
            _playerService.RegisterPlayer(this);

            EventBus.Instance.Subscribe<GameStateChangedEvent>(this, Reset);
        }


        public void OnTakeDamage(Color colorOfHitter)
        {
            if (_wasRecentlyHit) return;

            _wasRecentlyHit = true;

            _currentHealth--;

            if (_currentHealth <= 0)
                ServiceLocator.Get<GameManagementService>().TransitionToMenu();
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
                _spriteRenderer.color = Color.white;
                _currentHealth = _maxHealth;
                transform.position = _startPos;
                _currentInvicibilityTimer = invincibilityTimer;
            }
        }

        protected override void Update()
        {
            base.Update();

            _currentShootDelay -= Time.deltaTime;

            if (_wasRecentlyHit)
                _currentInvicibilityTimer -= Time.deltaTime;

            if (_currentInvicibilityTimer <= 0.0f)
            {
                _currentInvicibilityTimer = invincibilityTimer;
                _wasRecentlyHit = false;
            }
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
                projectile.OnHit += ReturnProjectile;

            AudioManager.Instance.PlaySound(fireSFX);

            Color.RGBToHSV(_spriteRenderer.color, out float h, out float s, out float v);
            s = Mathf.Clamp(s - 0.025f, 0.0f, s);

            if (s < 0.1f)
                s = 0.0f;

            _spriteRenderer.color = Color.HSVToRGB(h, s, v);
        }

        private void ReturnProjectile(ProjectileBase projectile)
        {
            _projectilePoolFactory.Return(projectile);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _projectilePoolFactory.Clear();

            _playerService?.UnregisterPlayer(this);
        }
    }
}