using UnityEngine;
using System;

public class ArmoredBugAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    public event Action OnPlayerLeftHitDetected;
    public event Action OnPlayerRightHitDetected;

    private ArmoredBugView _view;

    private float _lastAttackTime;

    public bool IsAttacking { get; set; } = false;

    private void Awake()
    {
        _view = GetComponent<ArmoredBugView>();
    }

    private void FixedUpdate()
    {
        CheckPlayerHits();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _view.IsAccelerated = false;
            if (!_view.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_view.Model.Damage, transform.position, gameObject);
            }
        }
    }

    public bool CanAttack(float attackCooldown)
    {
        return !((Time.time < _lastAttackTime + attackCooldown) || IsAttacking);
    }

    public void UpdateLastAttackTime()
    {
        _lastAttackTime = Time.time;
    }

    private void CheckPlayerHits()
    {
        var playerHitLeft = Physics2D.Raycast(transform.position, Vector2.right, _view.AttackDist, _playerLayer);
        var playerHitRight = Physics2D.Raycast(transform.position, Vector2.left, _view.AttackDist, _playerLayer);
        if (playerHitLeft.collider != null)
        {
            OnPlayerLeftHitDetected?.Invoke();
            return;
        }
        else if (playerHitRight.collider != null)
        {
            OnPlayerRightHitDetected?.Invoke();
            return;
        }

        var playerDetectHitLeft = Physics2D.Raycast(transform.position, Vector2.right, _view.PlayerDetectDist, _playerLayer);
        var playerDetectHitRight = Physics2D.Raycast(transform.position, Vector2.left, _view.PlayerDetectDist, _playerLayer);
        if (playerDetectHitLeft.collider != null)
        {
            OnPlayerLeftDetected?.Invoke();
            return;
        }
        else if (playerDetectHitRight.collider != null)
        {
            OnPlayerRightDetected?.Invoke();
            return;
        }
    }
}
