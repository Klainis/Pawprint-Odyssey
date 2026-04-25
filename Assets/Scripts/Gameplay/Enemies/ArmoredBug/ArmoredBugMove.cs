using UnityEngine;
using System;

public class ArmoredBugMove : MonoBehaviour
{
    [SerializeField] private LayerMask _turnLayerMask;

    public event Action OnWallHit;

    private ArmoredBugView _view;
    private ArmoredBugAttack _attack;

    private Transform _fallCheck;
    private Transform _wallCheck;

    private bool _isInPlatform;
    private bool _isObstacle;

    private void Awake()
    {
        _view = GetComponent<ArmoredBugView>();
        _attack = GetComponent<ArmoredBugAttack>();

        _fallCheck = transform.Find("FallCheck");
        _wallCheck = transform.Find("WallCheck");
    }

    private void FixedUpdate()
    {
        _isInPlatform = Physics2D.OverlapCircle(_fallCheck.position, .2f, _turnLayerMask);
        _isObstacle = Physics2D.OverlapCircle(_wallCheck.position, .2f, _turnLayerMask);
        if ((!_isInPlatform && _view.RigidBody.linearVelocity.y >= 0) || _isObstacle)
            OnWallHit?.Invoke();
    }

    public void Move()
    {
        if (_view.IsHitted || Mathf.Abs(_view.RigidBody.linearVelocity.y) > 0.5f)
            return;

        var moveSpeed = _view.Model.Speed;
        var moveDirection = _view.FacingRight ? 1 : -1;

        if (!_view.IsHitted)
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
