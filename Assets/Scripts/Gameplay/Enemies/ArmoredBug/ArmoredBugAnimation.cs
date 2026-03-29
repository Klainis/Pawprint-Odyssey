using UnityEngine;

public class ArmoredBugAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetTriggerHit()
    {
        _animator.SetTrigger("Hit");
    }

    public void SetTriggerBlockHit()
    {
        _animator.SetTrigger("BlockHit");
    }

    public void SetBoolMove(bool value)
    {
        _animator.SetBool("Move", value);
    }

    public void SetTriggerDead()
    {
        _animator.SetTrigger("Dead");
    }

    public void SetBoolAttack(bool value)
    {
        _animator.SetBool("Attack", value);
    }
}
