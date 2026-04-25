using UnityEngine;
using System;

public class ArmoredBugAttack : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;

    [Header("Attack")]
    [SerializeField] private float _playerDetectDist = 5f;
    [SerializeField] private float _attackDist = 1f;

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    public event Action OnPlayerLeftHitDetected;
    public event Action OnPlayerRightHitDetected;

    private ArmoredBugView _view;

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

    private void Update()
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
        var playerHitLeft = Physics2D.Raycast(transform.position, Vector2.left, _attackDist, _playerLayer);
        var playerHitRight = Physics2D.Raycast(transform.position, Vector2.right, _attackDist, _playerLayer);

        //Collider2D playerHit = Physics2D.OverlapBox(transform.position, new Vector2(_attackDist * 2, 1f), 0, _playerLayer);

        //bool rightHit = false;

        //if (playerHit != null)
        //{
        //    rightHit = playerHit.transform.position.x > transform.position.x ? true : false;

        //    if (!rightHit)
        //    {
        //        OnPlayerRightHitDetected?.Invoke();
        //        return;
        //    }
        //    else if (rightHit)
        //    {
        //        OnPlayerLeftHitDetected?.Invoke();
        //        return;
        //    }
        //}

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

        var playerDetectHitLeft = Physics2D.Raycast(transform.position, Vector2.left, _playerDetectDist, _playerLayer);
        var playerDetectHitRight = Physics2D.Raycast(transform.position, Vector2.right, _playerDetectDist, _playerLayer);

        //Collider2D playerDetect = Physics2D.OverlapBox(transform.position, new Vector2(_playerDetectDist * 2, 1.5f), 0, _playerLayer);

        //bool rightDetect = false;

        //if (playerDetect != null)
        //{
        //    rightDetect = playerDetect.transform.position.x > transform.position.x ? true : false;

        //    if (!rightDetect)
        //    {
        //        OnPlayerLeftDetected?.Invoke();
        //        return;
        //    }
        //    else if (rightDetect)
        //    {
        //        OnPlayerRightDetected?.Invoke();
        //        return;
        //    }
        //}
        //if (playerDetectHitLeft.collider == null && playerDetectHitRight.collider == null)
        //{
        //    _view.IsTargeting = false;
        //    return;
        //}

        if (playerDetectHitLeft.collider == null && playerDetectHitRight.collider == null)
        {
            _view.IsTargeting = false;
            return;
        }

        if (playerDetectHitLeft.collider != null)
        {
            _view.IsTargeting = true;
            OnPlayerLeftDetected?.Invoke();
            return;
        }
        else if (playerDetectHitRight.collider != null)
        {
            _view.IsTargeting = true;
            OnPlayerRightDetected?.Invoke();
            return;
        }
    }
}
