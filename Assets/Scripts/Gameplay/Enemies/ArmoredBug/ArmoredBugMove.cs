using UnityEngine;
using System;

public class ArmoredBugMove : MonoBehaviour
{
    [SerializeField] private LayerMask _turnLayerMask;
    [SerializeField] private float _checkRadius = 0.2f;

    public event Action OnWallHit;

    private ArmoredBugView _view;
    private ArmoredBugAttack _attack;

    private Transform _fallCheck;
    private Transform _wallCheck;

    private bool _isInPlatform;
    private bool _isObstacle;

    private float _lastTurnTime;
    private float _turnCooldown = 0.2f;

    private Vector2 _backFallCheckPosition;
    private Vector2 _backWallCheckPosition;

    public bool IsTrapped { get; private set; }

    private void Awake()
    {
        _view = GetComponent<ArmoredBugView>();
        _attack = GetComponent<ArmoredBugAttack>();

        _fallCheck = transform.Find("FallCheck");
        _wallCheck = transform.Find("WallCheck");
    }

    private void FixedUpdate()
    {
        //_isInPlatform = Physics2D.OverlapCircle(_fallCheck.position, .2f, _turnLayerMask);
        //_isObstacle = Physics2D.OverlapCircle(_wallCheck.position, .2f, _turnLayerMask);

        //if (Mathf.Abs(_view.RigidBody.linearVelocity.y) < 0.1f)
        //{
        //    if ((!_isInPlatform || _isObstacle) && Time.time > _lastTurnTime + _turnCooldown)
        //    {
        //        _lastTurnTime = Time.time;
        //        OnWallHit?.Invoke();
        //    }
        //}

        if (Mathf.Abs(_view.RigidBody.linearVelocity.y) < 0.1f)
        {
            var isInPlatformFront = Physics2D.OverlapCircle(_fallCheck.position, _checkRadius, _turnLayerMask);
            var isObstacleFront = Physics2D.OverlapCircle(_wallCheck.position, _checkRadius, _turnLayerMask);
            var isBlockedFront = !isInPlatformFront || isObstacleFront;
            var backDirection = _view.FacingRight ? -1f : 1f;
            var fallCheckDist = Mathf.Abs(_fallCheck.localPosition.x);
            var wallCheckDist = Mathf.Abs(_wallCheck.localPosition.x);

            _backFallCheckPosition = (Vector2)transform.position + new Vector2(backDirection * fallCheckDist, _fallCheck.localPosition.y);
            _backWallCheckPosition = (Vector2)transform.position + new Vector2(backDirection * wallCheckDist, _wallCheck.localPosition.y);
            
            var isInPlatformBack = Physics2D.OverlapCircle(_backFallCheckPosition, _checkRadius, _turnLayerMask);
            var isObstacleBack = Physics2D.OverlapCircle(_backWallCheckPosition, _checkRadius, _turnLayerMask);
            var isBlockedBack = !isInPlatformBack || isObstacleBack;

            if (isBlockedFront && isBlockedBack)
                IsTrapped = true;
            else
            {
                IsTrapped = false;
                
                if (isBlockedFront && Time.time > _lastTurnTime + _turnCooldown)
                {
                    _lastTurnTime = Time.time;
                    OnWallHit?.Invoke();
                }
            }
        }
        else
        {
            IsTrapped = false;
        }
    }

    public void Move()
    {
        if (_view.IsHitted)
            return;

        var moveSpeed = _view.Model.Speed;
        var moveDirection = _view.FacingRight ? 1 : -1;

        _view.RigidBody.linearVelocity = new Vector2(moveDirection * moveSpeed, _view.RigidBody.linearVelocity.y);
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
}
