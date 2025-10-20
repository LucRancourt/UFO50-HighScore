using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private float hitpoints = 10.0f;
    [SerializeField] private float hpLossOnHit = 1.0f;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.gravityScale = 0.0f;
    }

    public void OnTakeDamage(Color colorOfHitter)
    {
        Color.RGBToHSV(_spriteRenderer.color, out float myH, out float myS, out float myV);
        Color.RGBToHSV(colorOfHitter, out float h, out float s, out float v);

        if (myH == h && myS != 0.0f)
        {
            hitpoints -= hpLossOnHit * 4.0f;
            return;
        }

        hitpoints -= hpLossOnHit;
        Debug.Log(hitpoints);
    }
}
