using UnityEngine;

using _Project.Code.Core.Pool;


namespace _Project.Code.Gameplay.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class ProjectileBase : MonoBehaviour, IPoolable
    {
        public event Action On
        private Rigidbody2D _rigidbody2D;
        [SerializeField]

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void OnSpawnFromPool()
        {
            
        }

        private void FixedUpdate()
        {
            Vector2 moveVector = transform.forward.normalized * speed * Time.deltaTime;

            moveVector += _rigidbody2D.position;

            _rigidbody2D.MovePosition(moveVector);
        }

        public void OnReturnToPool()
        {

        }
    }
}