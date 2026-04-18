using System;
using UnityEngine;

public class RootGuardianMove : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask _turnLayerMask;
    [SerializeField] private Collider2D _patrolBounds;

    #region Variables

    private Transform _fallCheck;
    private Transform _wallCheck;
    private Rigidbody2D _rb;

    #endregion

    public event Action OnWallHit;

    #region Common Methods

    private void Awake()
    {
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

            var isInsideZone = true;
            if (_patrolBounds != null)
                isInsideZone = _patrolBounds.OverlapPoint(_wallCheck.position);

            if (!isPlat || isObstacle || !isInsideZone)
                OnWallHit?.Invoke();
        }
    }

    #endregion

    public void Move(float speed, bool facingRight)
    {
        if (Mathf.Abs(_rb.linearVelocity.y) > 0.5f)
            return;

        var moveDirection = facingRight ? 1 : -1;
        _rb.linearVelocity = new Vector2(moveDirection * speed, _rb.linearVelocity.y);
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
