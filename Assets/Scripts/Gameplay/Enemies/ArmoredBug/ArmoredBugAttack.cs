using UnityEngine;
using System;

public class ArmoredBugAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    private ArmoredBugView _bugView;

    private void Awake()
    {
        _bugView = GetComponent<ArmoredBugView>();
    }

    private void FixedUpdate()
    {
        CheckPlayerHits();
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

    private void CheckPlayerHits()
    {
        var playerHitLeft = Physics2D.Raycast(transform.position, Vector2.right, _bugView.PlayerDetectDist, _playerLayer);
        var playerHitRight = Physics2D.Raycast(transform.position, Vector2.left, _bugView.PlayerDetectDist, _playerLayer);

        if (playerHitLeft.collider != null)
            OnPlayerLeftDetected?.Invoke();
        else if (playerHitRight.collider != null)
            OnPlayerRightDetected?.Invoke();
    }
}
