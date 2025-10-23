using UnityEngine;

namespace _Project.Code.Gameplay.Projectiles
{
    public class ColorBomb : ProjectileBase
    {
        private GameObject _explosion;
        public bool WasFullCharged;

        private void Awake()
        {
            _explosion = transform.GetChild(0).gameObject;
            _explosion.SetActive(false);
        }

        protected override void Initialize()
        {
            base.Initialize();

            WasFullCharged = false;
            _explosion.SetActive(false);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("OutOfBounds"))
            {
                CallOnHit();
                return;
            }

            _explosion.GetComponent<SpriteRenderer>().color = _spriteRenderer.color;
            _explosion.SetActive(true);
            Invoke("HideExplosion", 0.5f);
            _currentSpeed = 0.0f;

            if (collision.TryGetComponent(out EnemyBase originalHit))
            {
                originalHit.Die();
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.0f);


            foreach (Collider2D hit in hits)
            {
                if (collision == hit) return;

                if (hit.TryGetComponent(out IDamageable damaged))
                {
                    if (hit.TryGetComponent(out EnemyBase enemyHit))
                    {
                        damaged.OnTakeDamage(_spriteRenderer.color);
                    }
                }
            }
        }

        private void HideExplosion()
        {
            _explosion.SetActive(false);
            WasFullCharged = false;
            CallOnHit();
        }
    }
}