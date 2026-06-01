using System;
using UnityEngine;

public class WSMove : MonoBehaviour
{
    [SerializeField] private LayerMask turnLayerMask;
    [SerializeField] private float checkRadius = 0.2f;

    public event Action OnWallHit;
    public event Action OnTrapped;

    private WanderingSpiritView wsView;

    private Transform fallCheck;
    private Transform wallCheck;

    private Rigidbody2D rb;

    private Vector2 backFallCheckPosition;
    private Vector2 backWallCheckPosition;

    private void Awake()
    {
        wsView = GetComponent<WanderingSpiritView>();
        rb = GetComponent<Rigidbody2D>();

        fallCheck = transform.Find("FallCheck");
        wallCheck = transform.Find("WallCheck");
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            var isPlatFront = Physics2D.OverlapCircle(fallCheck.position, checkRadius, turnLayerMask);
            var isObstacleFront = Physics2D.OverlapCircle(wallCheck.position, checkRadius, turnLayerMask);
            var isBlockedFront = !isPlatFront || isObstacleFront;

            var fallCheckDist = Mathf.Abs(fallCheck.localPosition.x);
            var wallCheckDist = Mathf.Abs(wallCheck.localPosition.x);

            var backDirection = wsView.FacingRight ? 1 : -1;

            backFallCheckPosition = (Vector2)transform.position + new Vector2(backDirection * fallCheckDist, fallCheck.localPosition.y);
            backWallCheckPosition = (Vector2)transform.position + new Vector2(backDirection * wallCheckDist, wallCheck.localPosition.y);

            var isPlatBack = Physics2D.OverlapCircle(backFallCheckPosition, checkRadius, turnLayerMask);
            var isObstacleBack = Physics2D.OverlapCircle(backWallCheckPosition, checkRadius, turnLayerMask);
            var isBlockedBack = !isPlatBack || isObstacleBack;

            if (isBlockedFront && isBlockedBack)
            {
                OnTrapped?.Invoke();
                
                if (isPlatFront && !isPlatBack)
                    OnWallHit?.Invoke();
            }
            else if (isBlockedFront)
                OnWallHit?.Invoke();
        }
    }

    public void Move(bool isAccelerated = false, float acceleratedSpeed = 0f)
    {
        if (wsView.IsHitted || Mathf.Abs(wsView.RigidBody.linearVelocity.y) > 0.5f)
            return;

        var moveSpeed = isAccelerated ? acceleratedSpeed: wsView.Model.Speed;
        var moveDirection = wsView.FacingRight ? -1 : 1;

        if (!wsView.IsHitted)
            wsView.RigidBody.linearVelocity = new Vector2(moveDirection * moveSpeed, wsView.RigidBody.linearVelocity.y);
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
