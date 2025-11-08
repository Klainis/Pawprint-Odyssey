using UnityEngine;

public class WSAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetBoolHit(bool value)
    {
        animator.SetBool("Hit", value);
    }

    public void SetTriggerDead()
    {
        animator.SetTrigger("Dead");
    }
}
