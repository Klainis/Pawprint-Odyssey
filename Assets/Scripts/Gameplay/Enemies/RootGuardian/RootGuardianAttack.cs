using System;
using System.Collections;
using UnityEngine;

public class RootGuardianAttack : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask _playerLayer;

    #region Variables

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    private RootGuardianAnimation _animation;
    private Rigidbody2D _rb;

    private Color _defaultColor;
    private RigidbodyConstraints2D _defaultConstraints;

    private float _lastAttackTime;

    #endregion

    #region Properties

    public float PlayerDetectDist { get; set; }
    public bool IsAttacking { get; set; }

    #endregion

    #region Common Methods

    private void Awake()
    {
        _animation = GetComponent<RootGuardianAnimation>();
        _rb = GetComponent<Rigidbody2D>();

        var renderer = GetComponent<SpriteRenderer>();
        _defaultColor = renderer.color;
        _defaultConstraints = _rb.constraints;
    }

    private void FixedUpdate()
    {
        if (IsAttacking) return;
        CheckPlayerHits();
    }

    #endregion

    public void UpdateLastAttackTime()
    {
        _lastAttackTime = Time.time;
    }

    public bool CanAttack(float attackCooldown)
    {
        return !((Time.time < _lastAttackTime + attackCooldown) || IsAttacking);
    }

    public void Attack()
    {
        IsAttacking = true;
    }

    private void CheckPlayerHits()
    {
        var playerHitLeft = Physics2D.Raycast(transform.position, Vector2.right, PlayerDetectDist, _playerLayer);
        var playerHitRight = Physics2D.Raycast(transform.position, Vector2.left, PlayerDetectDist, _playerLayer);

        if (playerHitLeft.collider != null)
            OnPlayerLeftDetected?.Invoke();
        else if (playerHitRight.collider != null)
            OnPlayerRightDetected?.Invoke();
    }

    public IEnumerator AttackTelegraphRoutine(float telegraphTime)
    {
        var renderer = GetComponent<SpriteRenderer>();
        renderer.color = Color.red;
        _rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        _animation.SetBoolAttack(true);

        yield return new WaitForSeconds(telegraphTime);

        renderer.color = _defaultColor;
        _rb.constraints = _defaultConstraints;

        Attack();

        yield return new WaitForSeconds(0.5f);

        IsAttacking = false;
        _animation.SetBoolAttack(false);
        UpdateLastAttackTime();
    }
}
