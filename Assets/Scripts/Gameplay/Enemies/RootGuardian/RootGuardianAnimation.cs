using UnityEngine;

public class RootGuardianAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetBoolAttack(bool value)
    {
        _animator.SetBool("Attack", value);
    }

    public void SetBoolMove(bool value)
    {
        _animator.SetBool("Move", value);
    }

    public void SetTriggerHit()
    {
        _animator.SetTrigger("Hit");
    }

    public void SetTriggerDead()
    {
        _animator.SetTrigger("Dead");
    }
}
