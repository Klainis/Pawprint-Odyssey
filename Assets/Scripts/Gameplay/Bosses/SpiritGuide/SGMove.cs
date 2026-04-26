using System;
using UnityEngine;

public class SGMove : MonoBehaviour
{
    [SerializeField] private LayerMask turnLayerMask;
    [SerializeField] private Transform wallCheck;

    public event Action OnWallHit;

    private SpiritGuideView sgView;
    private SGAnimation sgAnimation;

    private bool isObstacle;

    private void Awake()
    {
        sgView = GetComponent<SpiritGuideView>();
        sgAnimation = GetComponent<SGAnimation>();
    }

    private void FixedUpdate()
    {
        isObstacle = Physics2D.OverlapCircle(wallCheck.position, 0.1f, turnLayerMask);
        if (isObstacle)
            OnWallHit?.Invoke();
    }

    public void Move(bool isAccelerated = false, float acceleratedSpeed = 0f)
    {
        if (sgView.MoveDisabled || sgView.IsHitted || Mathf.Abs(sgView.RigidBody.linearVelocity.y) > 0.5f)
            return;

        var moveSpeed = isAccelerated ? acceleratedSpeed : sgView.Model.Speed;
        var moveDirection = sgView.FacingRight ? -1 : 1;

        if (!sgView.IsHitted)
        {
            if (!isAccelerated)
            {
                sgAnimation.SetRunAnimation(false);
                sgAnimation.SetWalkAnimation(true);
            }
            else
            {
                sgAnimation.SetWalkAnimation(false);
                sgAnimation.SetRunAnimation(true);
            }

            sgView.RigidBody.linearVelocity = new Vector2(moveDirection * moveSpeed, sgView.RigidBody.linearVelocity.y);
        }
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
