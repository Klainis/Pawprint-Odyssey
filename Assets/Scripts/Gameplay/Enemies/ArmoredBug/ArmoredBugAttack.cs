using UnityEngine;
using System;

public class ArmoredBugAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;

    public event Action OnPlayerDetected;

    private ArmoredBugView _bugView;

    private void Awake()
    {
        _bugView = GetComponent<ArmoredBugView>();
    }

    private void FixedUpdate()
    {
        var playerHitDir = _bugView.FacingRight ? Vector2.left : Vector2.right;

        var playerHit = Physics2D.Raycast(transform.position, playerHitDir, _bugView.PlayerDetectDist, _playerLayer);
        if (playerHit.collider != null)
            OnPlayerDetected?.Invoke();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _bugView.IsAccelerated = false;
            if (!_bugView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_bugView.Model.Damage, transform.position);
            }
        }
    }
}
