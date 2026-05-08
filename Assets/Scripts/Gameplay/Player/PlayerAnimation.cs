using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private static PlayerAnimation _instance;
    public static PlayerAnimation Instance {  get { return _instance; } }

    [SerializeField] private Animator slashAnimator;
    [SerializeField] private Animator clawAnimator;

    private SlashAnimation endSlashAnimation;
    private ClawAnimation clawAnimation;
    private Animator animator;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;

        animator = GetComponent<Animator>();
        endSlashAnimation = slashAnimator.gameObject.GetComponent<SlashAnimation>();
        clawAnimation = clawAnimator.gameObject.GetComponent<ClawAnimation>();
    }

    public void ResetAnimatorParameters()
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(parameter.name, (int)parameter.defaultInt);
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(parameter.name, parameter.defaultFloat);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(parameter.name, parameter.defaultBool);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(parameter.name);
                    break;
            }
        }

        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
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

    public void SetBoolIsFall(bool value)
    {
        animator.SetBool("IsFall", value);
    }

    public void SetBoolIsLand(bool value)
    {
        animator.SetBool("IsLand", value);
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

    public void SetBoolClaw(bool value)
    {
        animator.SetBool("Claw", value);
    }

    public void SetBoolSoulRelease(bool value)
    {
        animator.SetBool("SoulRelease", value);
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

    public void SetBoolIsChargingAttack(bool value)
    {
        animator.SetBool("IsChargingAttack", value);
    }

    public void SetBoolIsParrying(bool value)
    {
        animator.SetBool("IsParrying", value);
    }

    public void SetTriggerAttack(int attackNum)
    {
        endSlashAnimation.SetActiveSlashObject();

        var triggerName = "Attack" + attackNum;
        var slashTriggerName = "Slash" + attackNum;
        animator.SetTrigger(triggerName);
        slashAnimator.SetTrigger(slashTriggerName);
    }

    public void ResetTriggerAttack(int attackNum)
    {
        var triggerName = "Attack" + attackNum;
        var slashTriggerName = "Slash" + attackNum;
        animator.ResetTrigger(triggerName);
        slashAnimator.ResetTrigger(slashTriggerName);
    }
}
