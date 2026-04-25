using System;
using UnityEngine;

public class RootGuardianMove : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask _turnLayerMask;
    [SerializeField] private BoxCollider2D _patrolBounds;

    #region Variables

    private RootGuardianAnimation _animation;
    private Rigidbody2D _rb;
    private Transform _fallCheck;
    private Transform _wallCheck;

    #endregion

    public event Action OnWallHit;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (_patrolBounds != null)
            Gizmos.DrawWireCube((Vector2)_patrolBounds.transform.position + _patrolBounds.offset, _patrolBounds.size);
    }

    #region Common Methods

    private void Awake()
    {
        _animation = GetComponent<RootGuardianAnimation>();
        _rb = GetComponent<Rigidbody2D>();

        _fallCheck = transform.Find("FallCheck");
        _wallCheck = transform.Find("WallCheck");
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(_rb.linearVelocity.y) < 0.1f)
        {
            var isPlat = Physics2D.OverlapCircle(_fallCheck.position, .2f, _turnLayerMask);
            var isObstacle = Physics2D.OverlapCircle(_wallCheck.position, .2f, _turnLayerMask);

            if (!isPlat || isObstacle)
            {
                OnWallHit?.Invoke();
                return;
            }

            var isInsideZone = true;
            if (_patrolBounds != null)
                isInsideZone = _patrolBounds.OverlapPoint(_wallCheck.position);

            if (!isInsideZone)
            {
                var directionToCenter = _patrolBounds.bounds.center.x - transform.position.x;
                var moveDirection = _rb.linearVelocity.x;

                if ((directionToCenter > 0 && moveDirection < 0) || (directionToCenter < 0 && moveDirection > 0))
                    OnWallHit?.Invoke();
            }
        }
    }

    #endregion

    public void Move(float speed, bool facingRight)
    {
        if (Mathf.Abs(_rb.linearVelocity.y) > 0.5f)
            return;

        _animation.SetBoolMove(true);
        var moveDirection = facingRight ? 1 : -1;
        _rb.linearVelocity = new Vector2(moveDirection * speed, _rb.linearVelocity.y);
        //Debug.Log($"Moving: {_rb.linearVelocity}");
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

    public void StopMove()
    {
        _animation.SetBoolMove(false);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
    }
}
