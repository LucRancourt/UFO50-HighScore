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


namespace _Project.Code.Gameplay.PlayerController.Drone
{
    [RequireComponent(typeof(CharacterControllerMotor))]
    public class DroneController : BasePlayerController
    {
        [Header("PlayerController Settings")]
        [field: SerializeField] public DroneMovementProfile MovementProfile { get; private set; }

        private CharacterControllerMotor _motor;
        private PlayerService _playerService;

        public CharacterControllerMotor Motor => _motor;
        public Vector2 MoveInput { get; set; }


        // Shooter specific properties
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
            _currentShootDelay = shootDelay;
        }

        protected override void Start()
        {
            base.Start();

            _projectilePoolFactory = new PooledFactory<ProjectileBase>(projectilePrefab);

            _playerService = ServiceLocator.Get<PlayerService>();
            _playerService.RegisterPlayer(this);
        }

        public override void Initialize()
        {
            var idleState = new DroneIdleState(this);
            StateMachine = new FiniteStateMachine<IState>(idleState);

            StateMachine.AddState(new DroneMovingState(this));
        }

        protected override void Update()
        {
            base.Update();

            _currentShootDelay -= Time.deltaTime;
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

        public void FireProjectile()
        {
            if (_currentShootDelay > 0.0f) return;

            _currentShootDelay = shootDelay;

            var projectile = _projectilePoolFactory.Create(transform.position, transform.rotation);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _projectilePoolFactory.Clear();

            _playerService?.UnregisterPlayer(this);
        }
    }
}