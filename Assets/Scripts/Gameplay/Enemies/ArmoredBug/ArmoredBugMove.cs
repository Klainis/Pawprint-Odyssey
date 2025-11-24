using UnityEngine;
using System;

public class ArmoredBugMove : MonoBehaviour
{
    [SerializeField] private LayerMask _turnLayerMask;

    public event Action OnWallHit;

    private ArmoredBugView _bugView;

    private Transform _fallCheck;
    private Transform _wallCheck;

    private bool _isInPlatform;
    private bool _isObstacle;

    private void Awake()
    {
        _bugView = GetComponent<ArmoredBugView>();

        _fallCheck = transform.Find("FallCheck");
        _wallCheck = transform.Find("WallCheck");
    }

    private void FixedUpdate()
    {
        _isInPlatform = Physics2D.OverlapCircle(_fallCheck.position, .2f, _turnLayerMask);
        _isObstacle = Physics2D.OverlapCircle(_wallCheck.position, .2f, _turnLayerMask);
        if (!_isInPlatform || _isObstacle)
            OnWallHit?.Invoke();
    }

    public void Move(bool isAccelerated = false, float acceleratedSpeed = 0f)
    {
        if (_bugView.IsHitted || Mathf.Abs(_bugView.RigidBody.linearVelocity.y) > 0.5f)
            return;

        var moveSpeed = isAccelerated ? acceleratedSpeed : _bugView.Model.Speed;
        var moveDirection = _bugView.FacingRight ? -1 : 1;

        if (!_bugView.IsHitted)
            _bugView.RigidBody.linearVelocity = new Vector2(moveDirection * moveSpeed, _bugView.RigidBody.linearVelocity.y);
    }

    public bool Turn(bool facingRight)
    {
        Vector3 rotator;
        if (facingRight)
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        else
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        transform.rotation = Quaternion.Euler(rotator);
        return !facingRight;
    }
}
