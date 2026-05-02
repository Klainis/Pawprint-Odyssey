using UnityEngine;

public class FogShadowProjectile : MonoBehaviour
{
    #region Variables

    private Rigidbody2D _rb;
    private int _damage;

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerView = collision.GetComponent<PlayerView>();
            playerView?.ApplyDamage(_damage, transform.position, gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 velocity, int damage)
    {
        _rb = GetComponent<Rigidbody2D>();
        _damage = damage;
        _rb.linearVelocity = velocity;
    }
}
