using UnityEngine;

using _Project.Code.Core.Pool;


namespace _Project.Code.Gameplay.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class ProjectileBase : MonoBehaviour, IPoolable
    {
        [SerializeField] private ProjectileType projectileType;

        [SerializeField] private float defaultSpeed = 10.0f;
        private float _currentSpeed;

        private Rigidbody2D _rigidbody2D;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            
            GetComponent<BoxCollider2D>().enabled = false;

            _currentSpeed = defaultSpeed;
        }

        public void OnSpawnFromPool()
        {
            _currentSpeed = defaultSpeed;
        }

        private void FixedUpdate()
        {
            Vector2 moveVector = transform.up.normalized * _currentSpeed * Time.deltaTime;

            moveVector += _rigidbody2D.position;

            _rigidbody2D.MovePosition(moveVector);
        }

        public void OnReturnToPool()
        {

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