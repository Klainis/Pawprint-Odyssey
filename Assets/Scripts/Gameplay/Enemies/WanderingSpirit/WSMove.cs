using System;
using UnityEngine;

public class WSMove : MonoBehaviour
{
    public event Action OnWallHit;

    [SerializeField] private LayerMask turnLayerMask;

    private WanderingSpiritView wsView;

    private Transform fallCheck;
    private Transform wallCheck;

    private bool isPlat;
    private bool isObstacle;

    private void Awake()
    {
        wsView = GetComponent<WanderingSpiritView>();

        fallCheck = transform.Find("FallCheck");
        wallCheck = transform.Find("WallCheck");
    }

    private void FixedUpdate()
    {
        isPlat = Physics2D.OverlapCircle(fallCheck.position, .2f, turnLayerMask);
        isObstacle = Physics2D.OverlapCircle(wallCheck.position, .2f, turnLayerMask);
        if (!isPlat || isObstacle)
            OnWallHit?.Invoke();
    }

    public void Move(bool isAccelerated = false, float acceleratedSpeed = 0f)
    {
        if (wsView.IsHitted || Mathf.Abs(wsView.RigidBody.linearVelocity.y) > 0.5f)
            return;

        var moveSpeed = isAccelerated ? acceleratedSpeed : wsView.Model.Speed;
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
