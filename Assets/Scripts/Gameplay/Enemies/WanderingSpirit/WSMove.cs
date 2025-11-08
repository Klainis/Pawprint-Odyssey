using System;
using UnityEngine;

public class WSMove : MonoBehaviour
{
    public event Action OnWallHit;

    [SerializeField] private LayerMask turnLayerMask;

    private WanderingSpiritManager manager;

    private Transform fallCheck;
    private Transform wallCheck;

    private bool isPlat;
    private bool isObstacle;

    private void Awake()
    {
        manager = GetComponent<WanderingSpiritManager>();

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
        if (manager.IsHitted || Mathf.Abs(manager.RigidBody.linearVelocity.y) > 0.5f)
            return;

        var moveSpeed = isAccelerated ? acceleratedSpeed : manager.Data.Speed;
        var moveDirection = manager.FacingRight ? -1 : 1;

        if (!manager.IsHitted)
            manager.RigidBody.linearVelocity = new Vector2(moveDirection * moveSpeed, manager.RigidBody.linearVelocity.y);
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
