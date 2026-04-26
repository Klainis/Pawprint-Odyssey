using UnityEngine;

public class SGAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalkAnimation(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public void SetRunTelegraphAnimation(bool isTelegraph)
    {
        animator.SetBool("IsRunTelegraph", isTelegraph);
    }

    public void SetRunAnimation(bool isRunning)
    {
        animator.SetBool("isRunning", isRunning);
    }

    public void SetLightAttackAnimation(bool isLightAttacking)
    {
        animator.SetBool("isLightAttack", isLightAttacking);
    }
}
