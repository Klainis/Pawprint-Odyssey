using UnityEngine;

public class ArmoredBugAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetBoolHit(bool value)
    {
        _animator.SetBool("Hit", value);
    }

    public void SetBoolBlockHit(bool value)
    {
        _animator.SetBool("BlockHit", value);
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
