using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ApplyRootMotion(bool value)
    {
        animator.applyRootMotion = value;
    }

    public void SetBoolJumpUp(bool value)
    {
        animator.SetBool("JumpUp", value);
    }

    public void SetBoolIsJumping(bool value)
    {
        animator.SetBool("IsJumping", value);
    }

    public void SetBoolIsDoubleJumping(bool value)
    {
        animator.SetBool("IsDoubleJumping", value);
    }

    public void SetBoolIsDashing(bool value)
    {
        animator.SetBool("IsDashing", value);
    }

    public void SetBoolIsWallRunning(bool value)
    {
        animator.SetBool("IsWallRunning", value);
    }

    public void SetBoolIsWallSliding(bool value)
    {
        animator.SetBool("IsWallSliding", value);        
    }

    public void SetBoolHit(bool value)
    {
        animator.SetBool("Hit", value);
    }

    public void SetBoolIsDead(bool value)
    {
        animator.SetBool("IsDead", value);
    }

    public void SetFloatSpeed(float value)
    {
        animator.SetFloat("Speed", value);
    }

    public void SetBoolIsAttacking(bool value)
    {
        animator.SetBool("IsAttacking", value);
    }

    public void SetTriggerAttack(int attackNum)
    {
        var triggerName = "Attack" + attackNum;
        animator.SetTrigger(triggerName);
    }

    public void ResetTriggerAttack(int attackNum)
    {
        var triggerName = "Attack" + attackNum;
        animator.ResetTrigger(triggerName);
    }

    public void SetTriggerClaw()
    {
        animator.SetTrigger("Claw");
    }
}
