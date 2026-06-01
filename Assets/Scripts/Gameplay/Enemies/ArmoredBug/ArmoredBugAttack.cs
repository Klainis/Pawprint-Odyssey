using UnityEngine;
using System;

public class ArmoredBugAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer1;
    [SerializeField] private LayerMask _playerLayer2;

    [Header("Attack")]
    [SerializeField] private float _playerDetectDist = 5f;
    [SerializeField] private float _attackDist = 1f;

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    public event Action OnPlayerLeftHitDetected;
    public event Action OnPlayerRightHitDetected;

    private ArmoredBugView _view;

    private GameObject _player;

    private float _lastAttackTime;

    public bool IsAttacking { get; set; } = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_attackDist * 2, 1f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(_playerDetectDist * 2, 1.5f));
    }

    private void Awake()
    {
        _view = GetComponent<ArmoredBugView>();
    }

    private void FixedUpdate()
    {
        if (IsAttacking) return;
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

    public bool InAttackCooldown(float attackCooldown)
    {
        return Time.time < _lastAttackTime + attackCooldown;
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
        var playerHitLeft1 = Physics2D.Raycast(transform.position, Vector2.left, _attackDist, _playerLayer1);
        var playerHitRight1 = Physics2D.Raycast(transform.position, Vector2.right, _attackDist, _playerLayer1);
        var playerHitLeft2 = Physics2D.Raycast(transform.position, Vector2.left, _attackDist, _playerLayer2);
        var playerHitRight2 = Physics2D.Raycast(transform.position, Vector2.right, _attackDist, _playerLayer2);

        if (playerHitLeft1.collider != null || playerHitLeft2.collider != null)
        {
            OnPlayerLeftHitDetected?.Invoke();
            return;
        }
        else if (playerHitRight1.collider != null || playerHitRight2.collider != null)
        {
            OnPlayerRightHitDetected?.Invoke();
            return;
        }

        var playerDetectLeft1 = Physics2D.Raycast(transform.position, Vector2.left, _playerDetectDist, _playerLayer1);
        var playerDetectRight1 = Physics2D.Raycast(transform.position, Vector2.right, _playerDetectDist, _playerLayer1);
        var playerDetectLeft2 = Physics2D.Raycast(transform.position, Vector2.left, _playerDetectDist, _playerLayer2);
        var playerDetectRight2 = Physics2D.Raycast(transform.position, Vector2.right, _playerDetectDist, _playerLayer2);

        if ((playerDetectLeft1.collider == null && playerDetectRight1.collider == null) && 
            playerDetectLeft2.collider == null && playerDetectRight2.collider == null)
        {
            _view.IsTargeting = false;
            return;
        }

        if (playerDetectLeft1.collider != null || playerDetectLeft2.collider != null)
        {
            _view.IsTargeting = true;
            OnPlayerLeftDetected?.Invoke();
            return;
        }
        else if (playerDetectRight1.collider != null || playerDetectRight2.collider != null)
        {
            _view.IsTargeting = true;
            OnPlayerRightDetected?.Invoke();
            return;
        }
    }
}
