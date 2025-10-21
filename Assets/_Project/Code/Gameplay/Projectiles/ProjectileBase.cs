using UnityEngine;

using _Project.Code.Core.Pool;
using System;


namespace _Project.Code.Gameplay.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public class ProjectileBase : MonoBehaviour, IPoolable
    {
        public event Action<ProjectileBase> OnHit;

        [SerializeField] private ProjectileType projectileType;

        [SerializeField] private float defaultSpeed = 10.0f;
        private float _currentSpeed;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private bool _hasBeenInitialized = false;
        public bool HasOnHitBeenAdded { get; private set; } = false;

        private void Initialize()
        {
            _currentSpeed = defaultSpeed;

            if (_hasBeenInitialized) return;

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            _spriteRenderer = GetComponent<SpriteRenderer>();

            GetComponent<BoxCollider2D>().isTrigger = true;

            _hasBeenInitialized = true;
        }

        public void OnSpawnFromPool()
        {
            Initialize();
        }

        private void FixedUpdate()
        {
            Vector2 moveVector = transform.up.normalized * _currentSpeed * Time.deltaTime;

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

    enum ProjectileType
    {
        Up,
        Down,
        Left,
        Right,
        PlayerTarget
    }
}