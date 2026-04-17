using System;
using System.Collections;
using UnityEngine;

public class RootGuardianAttack : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private GameObject _spearObj;  // temp decision
    [SerializeField] private float _attackDuration = 1.0f;  // temp decision
    [SerializeField] private Vector3 _targetScale;  // temp decision

    #region Variables

    public event Action OnPlayerLeftDetected;
    public event Action OnPlayerRightDetected;

    private Vector3 _initialScale;

    private float _lastAttackTime;

    #endregion

    #region Properties

    public float PlayerDetectDist { get; set; }
    public bool IsAttacking { get; set; }

    #endregion

    #region Common Methods

    private void Start()
    {
        _initialScale = _spearObj.transform.localScale;
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
        StartCoroutine(ScaleSpearOverTime());
        IsAttacking = false;
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

    #region IEnumerators

    private IEnumerator ScaleSpearOverTime()
    {
        yield return StartCoroutine(ChangeSpearScale(_initialScale, _targetScale, _attackDuration));

        yield return StartCoroutine(ChangeSpearScale(_targetScale, _initialScale, _attackDuration));
    }

    private IEnumerator ChangeSpearScale(Vector3 from, Vector3 to, float time)
    {
        var elapsed = 0f;
        while (elapsed < time)
        {
            _spearObj.transform.localScale = Vector3.Lerp(from, to, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _spearObj.transform.localScale = to;
    }

    #endregion
}
