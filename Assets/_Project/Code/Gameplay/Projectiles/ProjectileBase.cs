using UnityEngine;

using _Project.Code.Core.Pool;
using System;
using _Project.Code.Gameplay.PlayerController.Drone;
using _Project.Code.Core.Events;
using _Project.Code.Gameplay.GameManagement;

namespace _Project.Code.Gameplay.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class ProjectileBase : MonoBehaviour, IPoolable
    {
        public event Action<ProjectileBase> OnHit;

        [SerializeField] private ProjectileType projectileType;
        private Vector3 _direction;

        [SerializeField] private float defaultSpeed = 10.0f;
        private float _currentSpeed;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private bool _hasBeenInitialized = false;
        public bool HasOnHitBeenAdded { get; private set; } = false;

        private DroneController _player;

        private void Initialize()
        {
            if (!_hasBeenInitialized)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

                _player = FindFirstObjectByType<DroneController>();

                _spriteRenderer = GetComponent<SpriteRenderer>();

                GetComponent<BoxCollider2D>().isTrigger = true;

                EventBus.Instance?.Subscribe<GameStateChangedEvent>(this, Reset);

                _hasBeenInitialized = true;
            }

            _currentSpeed = defaultSpeed;
        }

        private void Reset(GameStateChangedEvent gameState)
        {
            OnHit?.Invoke(this);
        }

        public void SetProjectTileType(ProjectileType projType)
        {
            projectileType = projType;
        }

        public void SetDirection()
        {
            switch (projectileType)
            {
                case ProjectileType.Up:
                    _direction = Vector2.up;
                    break;

                case ProjectileType.Down:
                    _direction = -Vector2.up;
                    break;

                case ProjectileType.Left:
                    _direction = -Vector2.right;
                    break;

                case ProjectileType.Right:
                    _direction = Vector2.right;
                    break;

                case ProjectileType.PlayerTarget:
                    _direction = (Vector2)(_player.transform.position - transform.position);
                    break;
            }
        }

        public void OnSpawnFromPool()
        {
            Initialize();
        }

        private void FixedUpdate()
        {
            Vector2 moveVector = _direction.normalized * _currentSpeed * Time.fixedDeltaTime;

            moveVector += _rigidbody2D.position;

            _rigidbody2D.MovePosition(moveVector);
        }

        public void ColorSwitch(Color color)
        {
            _spriteRenderer.color = color;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damaged))
            {
                damaged.OnTakeDamage(_spriteRenderer.color);
            }

            OnHit?.Invoke(this);
        }

        public void OnReturnToPool()
        {
            HasOnHitBeenAdded = true;
        }
    }

    public enum ProjectileType
    {
        Up,
        Down,
        Left,
        Right,
        PlayerTarget
    }
}