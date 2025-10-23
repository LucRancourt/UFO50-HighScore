using UnityEngine;


namespace _Project.Code.Gameplay.PlayerController._Base
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class CharacterControllerMotor : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.gravityScale = 0.0f;
        }

        public void Move(Vector2 direction, float speed)
        {
            Vector2 moveVector = direction.normalized * speed * Time.fixedDeltaTime;

            moveVector += _rigidbody2D.position;

            _rigidbody2D.MovePosition(moveVector);
        }
    }
}

