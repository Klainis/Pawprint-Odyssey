using TMPro;
using UnityEngine;

public class FogShadowMove : MonoBehaviour
{
    #region Variables

    private FogShadowAttack _attack;
    private FogShadowAnimation _animation;

    private Transform _playerTransform;

    private Vector2 _startPosition;
    private Vector2 _baseChaseOffset;
    private Vector2 _driftOffset;
    private Vector2 _playerAnchorPos;

    private float _macroTimer;
    private float _microTimer;

    #endregion

    #region Properties

    public LayerMask ObstacleLayer { get; set; }
    public Vector2 PatrolRange { get; set; }
    public Vector2 TargetPosition { get; private set; }
    public float Speed { get; set; }
    public float ChaseSpeed => Speed + 2.0f;
    public float FollowDistance { get; set; }
    public float HoverHeight { get; set; }
    public float RandomizeInterval { get; set; } = 1.5f;
    public float RandomRangeX { get; set; } = 2.0f;
    public float MacroInterval { get; set; } = 4.0f;
    public float MicroInterval { get; set; } = 0.8f;
    public float DriftRadius { get; set; } = 1.5f;
    public bool IsChasing { get; set; }

    #endregion

    #region Common Methods

    private void Start()
    {
        _attack = GetComponent<FogShadowAttack>();
        _animation = GetComponent<FogShadowAnimation>();

        _playerTransform = InitializeManager.Instance.player?.transform;
        if (_playerTransform == null)
            Debug.LogWarning("FogShadowMove : _playerTransform NOT FOUND");

        _startPosition = transform.position;
        SetNewTarget();
    }

    #endregion

    #region Move

    public void Chase()
    {
        _animation.SetBoolPatrol(false);
        _animation.SetBoolMove(true);

        _macroTimer -= Time.deltaTime;
        if (_macroTimer <= 0)
        {
            _playerAnchorPos = _playerTransform.position;

            var side = Random.value > 0.5f ? 1 : -1;
            _baseChaseOffset = new Vector2(side * FollowDistance, HoverHeight);

            _macroTimer = MacroInterval;
        }

        _microTimer -= Time.deltaTime;
        if (_microTimer <= 0)
        {
            _driftOffset = new Vector2(
                Random.Range(-DriftRadius, DriftRadius),
                Random.Range(-0.5f, 0.5f)
            );
            _microTimer = MicroInterval;
        }

        var targetX = _playerAnchorPos.x + _baseChaseOffset.x + _driftOffset.x;
        var targetY = _playerAnchorPos.y + _baseChaseOffset.y + _driftOffset.y;

        var rayStart = (Vector2)_playerTransform.position + Vector2.up * 0.5f;
        var hit = Physics2D.Raycast(rayStart, Vector2.up, HoverHeight, ObstacleLayer);

        if (hit.collider != null)
        {
            targetY = hit.point.y - 0.9f;
        }
        targetY = Mathf.Max(targetY, _playerAnchorPos.y + 1f);

        TargetPosition = new Vector2(targetX, targetY);
        transform.position = Vector2.MoveTowards(transform.position, TargetPosition, ChaseSpeed * Time.deltaTime);
    }

    public void Patrol()
    {
        _animation.SetBoolMove(false);
        _animation.SetBoolPatrol(true);

        transform.position = Vector2.MoveTowards(transform.position, TargetPosition, Speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, TargetPosition) < 0.2f)
            SetNewTarget();
    }

    private void SetNewTarget()
    {
        var randomX = Random.Range(_startPosition.x - PatrolRange.x, _startPosition.x + PatrolRange.x);
        var randomY = Random.Range(_startPosition.y - PatrolRange.y, _startPosition.y + PatrolRange.y);

        TargetPosition = new Vector2(randomX, randomY);
    }

    public bool Turn(bool facingRight)
    {
        if (_attack.IsAttacking)
            return facingRight;

        Vector3 rotator;
        if (facingRight)
            rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
        else
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        transform.rotation = Quaternion.Euler(rotator);
        return !facingRight;
    }

    public bool UpdateFacingDirection(bool facingRight)
    {
        if (!IsChasing)
        {
            if (TargetPosition.x < transform.position.x && facingRight
                || TargetPosition.x >= transform.position.x && !facingRight)
                return Turn(facingRight);
        }
        if (IsChasing)
        {
            var shouldFaceRight = _playerTransform.position.x > transform.position.x;
            if (shouldFaceRight != facingRight)
                return Turn(facingRight);
            else if (shouldFaceRight == facingRight)
                return Turn(!facingRight);
        }
        return facingRight;
    }

    #endregion
}
