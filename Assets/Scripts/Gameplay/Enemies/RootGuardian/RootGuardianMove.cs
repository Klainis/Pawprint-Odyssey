using System;
using UnityEngine;

public class RootGuardianMove : MonoBehaviour
{
    [Header("Main params")]
    [SerializeField] private LayerMask _turnLayerMask;

    #region Variables

    private Transform _fallCheck;
    private Transform _wallCheck;
    private Rigidbody2D _rb;
    private bool _isPlat;
    private bool _isObstacle;

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
            _isPlat = Physics2D.OverlapCircle(_fallCheck.position, .2f, _turnLayerMask);
            _isObstacle = Physics2D.OverlapCircle(_wallCheck.position, .2f, _turnLayerMask);

            if (!_isPlat || _isObstacle)
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
