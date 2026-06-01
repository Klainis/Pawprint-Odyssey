using System;
using System.Collections;
using UnityEngine;

public class RootGuardianAttack : MonoBehaviour
{
    #region SerializeFields

    [Header("Main params")]
    [SerializeField] private LayerMask _playerLayer;

    [Header("Attack")]
    [SerializeField] private float _playerDetectDist = 5f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _telegraphTime = 0.8f;
    [SerializeField] private Collider2D _weaponCollider;

    [Header("Attack Check Transform")]
    [SerializeField] private Transform _attackCheck;

    #endregion

    #region Variables

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    private RootGuardianAnimation _animation;
    private Rigidbody2D _rb;

    private RigidbodyConstraints2D _defaultConstraints;

    private float _lastAttackTime = 0f;

    #endregion

    #region Properties

    public bool IsAttacking { get; set; }
    public bool CanAttack {
        get {
            return !((Time.time < _lastAttackTime + _attackCooldown) || IsAttacking);
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_attackCheck.position, new Vector2(_playerDetectDist * 2, 1.2f));
    }

    #region Common Methods

    private void Awake()
    {
        _animation = GetComponent<RootGuardianAnimation>();
        _rb = GetComponent<Rigidbody2D>();

        _defaultConstraints = _rb.constraints;
        _weaponCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        if (IsAttacking || !CanAttack) return;
        CheckPlayerHits();
    }

    #endregion

    public bool InAttackCooldown()
    {
        return Time.time < _lastAttackTime + _attackCooldown;
    }

    public void UpdateLastAttackTime()
    {
        _lastAttackTime = Time.time;
    }

    private void CheckPlayerHits()
    {
        if (_attackCheck == null)
        {
            Debug.LogError($"Не назначен Attack Check {_attackCheck}");
            return;
        }

        var playerHit = Physics2D.OverlapBox(_attackCheck.position, new Vector2(_playerDetectDist * 2, 1.2f), 0, _playerLayer);
        if (playerHit != null)
        {
            var right = playerHit.transform.position.x > transform.position.x ? true : false;

            if (!right)
                OnPlayerLeftDetected?.Invoke();
            else if (right)
                OnPlayerRightDetected?.Invoke();
        }
    }

    public IEnumerator AttackTelegraphRoutine(/*float telegraphTime*/)
    {
        _animation.SetBoolTelegraph(true);
        IsAttacking = true;

        yield return new WaitForSeconds(_telegraphTime);

        _weaponCollider.enabled = true;
        _rb.constraints = _defaultConstraints;
        _animation.SetBoolTelegraph(false);
        _animation.SetBoolAttack(true);

        yield return new WaitForSeconds(0.5f);

        _weaponCollider.enabled = false;
        _animation.SetBoolAttack(false);
        UpdateLastAttackTime();
        IsAttacking = false;
    }
}
