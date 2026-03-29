using System;
using Unity.AppUI.Core;
using UnityEngine;

public class WSAttack : MonoBehaviour
{
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private LayerMask playerLayer;

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    private WanderingSpiritView _wsView;

    private float _lastAttackTime;
    private bool _isAttacking = false;

    public float LastAttackTime { get { return _lastAttackTime; } }
    public bool IsAttacking { get { return _isAttacking; } }

    private void Awake()
    {
        _wsView = GetComponent<WanderingSpiritView>();
    }

    private void FixedUpdate()
    {
        if (_isAttacking) return;
        CheckPlayerHits();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _wsView.IsAccelerated = false;
            if (!_wsView.Model.IsDead)
            {
                var playerView = collision.gameObject.GetComponent<PlayerView>();
                playerView.ApplyDamage(_wsView.Model.Damage, transform.position, gameObject);
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

    public bool CanJumpUp(float requiredHeight)
    {
        var checkDistance = requiredHeight + 1f;
        var hit = Physics2D.Raycast(transform.position, Vector2.up, checkDistance, environmentLayer);
        return hit.collider == null;
    }

    public void JumpToPoint(Vector3 targetPos, float jumpHeight)
    {
        var rb = _wsView.RigidBody;
        var gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);

        var deltaX = targetPos.x - transform.position.x;
        var deltaY = targetPos.y - transform.position.y;

        var vy = Mathf.Sqrt(2 * gravity * jumpHeight);

        var timeUp = vy / gravity;
        var timeDown = Mathf.Sqrt(2 * Mathf.Max(0, jumpHeight - deltaY) / gravity);
        var totalTime = timeUp + timeDown;

        var vx = deltaX / totalTime;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(vx, vy), ForceMode2D.Impulse);
    }

    private void CheckPlayerHits()
    {
        var playerHitLeft = Physics2D.Raycast(transform.position, Vector2.right, _wsView.PlayerDetectDist, playerLayer);
        var playerHitRight = Physics2D.Raycast(transform.position, Vector2.left, _wsView.PlayerDetectDist, playerLayer);

        if (playerHitLeft.collider != null)
            OnPlayerLeftDetected?.Invoke();
        else if (playerHitRight.collider != null)
            OnPlayerRightDetected?.Invoke();
    }
}
