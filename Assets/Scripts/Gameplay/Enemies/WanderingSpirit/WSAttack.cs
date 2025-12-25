using System;
using UnityEngine;

public class WSAttack : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    private WanderingSpiritView wsView;

    private void Awake()
    {
        wsView = GetComponent<WanderingSpiritView>();
    }

    private void FixedUpdate()
    {
        CheckPlayerHits();
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

    private void CheckPlayerHits()
    {
        var playerHitLeft = Physics2D.Raycast(transform.position, Vector2.right, wsView.PlayerDetectDist, playerLayer);
        var playerHitRight = Physics2D.Raycast(transform.position, Vector2.left, wsView.PlayerDetectDist, playerLayer);

        if (playerHitLeft.collider != null)
            OnPlayerLeftDetected?.Invoke();
        else if (playerHitRight.collider != null)
            OnPlayerRightDetected?.Invoke();
    }
}
