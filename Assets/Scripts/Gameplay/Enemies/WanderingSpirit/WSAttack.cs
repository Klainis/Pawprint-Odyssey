using System;
using UnityEngine;

public class WSAttack : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    public event Action OnPlayerDetected;

    private WanderingSpiritView wsView;

    private void Awake()
    {
        wsView = GetComponent<WanderingSpiritView>();
    }

    private void FixedUpdate()
    {
        var playerHitDir = wsView.FacingRight ? Vector2.left : Vector2.right;
        var playerHit = Physics2D.Raycast(transform.position, playerHitDir, wsView.PlayerDetectDist, playerLayer);
        if (playerHit.collider != null)
            OnPlayerDetected?.Invoke();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            wsView.IsAccelerated = false;
            if (!wsView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(wsView.Model.Damage, transform.position);
            }
        }
    }
}
