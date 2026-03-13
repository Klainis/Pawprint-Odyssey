using UnityEngine;
using System;

public class ArmoredBugAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    private ArmoredBugView _bugView;

    private float _lastAttackTime;
    private bool _isAttacking = false;

    public float LastAttackTime { get { return _lastAttackTime; } }
    public bool IsAttacking { get { return _isAttacking; } }

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

    public bool CanAttack(float attackCooldown)
    {
        return !((Time.time < _lastAttackTime + attackCooldown) || _isAttacking);
    }

    public void SetIsAttacking(bool flag)
    {
        _isAttacking = flag;
    }

    public void UpdateLastAttackTime()
    {
        _lastAttackTime = Time.time;
    }

    public float DashAttack(bool facingRight, float distance)
    {
        var rb = _bugView.RigidBody;
        var direction = facingRight ? -1 : 1;
        var dashSpeed = 20f;
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0);
        return distance / dashSpeed;
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
