using UnityEngine;

public class PimenAnimation : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetIsMove(bool value)
    {
        animator.SetBool("IsMove", value);
    }

    public void SetIsTrick(bool value)
    {
        animator.SetBool("IsTrick", value);
    }

    public void SetIsGetOutOfGround(bool value)
    {
        animator.SetBool("IsGetOutGround", value);
    }
}
