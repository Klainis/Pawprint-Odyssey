using UnityEngine;

public class GuardianOwlAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMoveAnimation(bool isWalking)
    {
        animator.SetBool("isMoving", isWalking);
    }

    //public void SetRunTelegraphAnimation(bool isTelegraph)
    //{
    //    animator.SetBool("IsRunTelegraph", isTelegraph);
    //}

    //public void SetRunAnimation(bool isRunning)
    //{
    //    animator.SetBool("isRunning", isRunning);
    //}

    public void SetLightAttackAnimation(bool isLightAttacking)
    {
        animator.SetBool("isLightAttack", isLightAttacking);
    }

    public void SetDeathAnimation()
    {
        animator.SetBool("isMoving", false);
        //animator.SetBool("isLightAttack", false);
        animator.SetBool("isDead", true);
    }
}
