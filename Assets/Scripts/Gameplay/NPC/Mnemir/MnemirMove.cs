using System;
using UnityEngine;

public class MnemirMove : MonoBehaviour
{
    #region Variables

    private Rigidbody2D _rb;
    private Transform _fallCheck;
    private Transform _wallCheck;

    #endregion

    public LayerMask TurnLayerMask { get; set; }
    public event Action OnWallHit;

    #region Common Methods

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _fallCheck = transform.Find("FallCheck");
        _wallCheck = transform.Find("WallCheck");
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(_rb.linearVelocity.y) < 0.1f)
        {
            var isPlat = Physics2D.OverlapCircle(_fallCheck.position, .3f, TurnLayerMask);
            var isObstacle = Physics2D.OverlapCircle(_wallCheck.position, .3f, TurnLayerMask);

            if (!isPlat || isObstacle)
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
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        else
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        transform.rotation = Quaternion.Euler(rotator);
        return !facingRight;
    }

    public void StopMove()
    {
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
    }
}
