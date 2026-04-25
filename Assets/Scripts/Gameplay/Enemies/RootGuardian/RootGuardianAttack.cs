using System;
using System.Collections;
using UnityEngine;

public class RootGuardianAttack : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask _playerLayer;

    [Header("Attack")]
    [SerializeField] private float _playerDetectDist = 5f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _telegraphTime = 0.8f;

    [Header("Attack Check Transform")]
    [SerializeField] private Transform _attackCheck;

    #region Variables

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    private RootGuardianAnimation _animation;
    private Rigidbody2D _rb;

    private RigidbodyConstraints2D _defaultConstraints;

    private float _lastAttackTime = 0f;

    #endregion

    #region Properties

    //public float PlayerDetectDist { get; set; }
    //public float AttackCooldown { get; set; }
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
    }

    private void FixedUpdate()
    {
        if (IsAttacking || !CanAttack) return;
        CheckPlayerHits();
    }

    #endregion

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

        Collider2D playerHit = Physics2D.OverlapBox(_attackCheck.position, new Vector2(_playerDetectDist * 2, 1.2f), 0, _playerLayer);

        bool right = false;

        if (playerHit != null)
        {
            right = playerHit.transform.position.x > transform.position.x ? true : false;

            if (!right)
                OnPlayerLeftDetected?.Invoke();
            else if (right)
                OnPlayerRightDetected?.Invoke();
        }
    }

    public IEnumerator AttackTelegraphRoutine(/*float telegraphTime*/)
    {
        //var renderer = GetComponent<SpriteRenderer>();
        //renderer.color = Color.red;
        _animation.SetBoolTelegraph(true);
        //_rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        IsAttacking = true;

        yield return new WaitForSeconds(_telegraphTime);

        //renderer.color = _defaultColor;
        _rb.constraints = _defaultConstraints;
        _animation.SetBoolTelegraph(false);
        _animation.SetBoolAttack(true);

        yield return new WaitForSeconds(0.5f);

        _animation.SetBoolAttack(false);
        UpdateLastAttackTime();
        IsAttacking = false;
    }
}
