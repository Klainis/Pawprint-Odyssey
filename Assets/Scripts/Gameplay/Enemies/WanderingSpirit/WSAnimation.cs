using UnityEngine;

public class WSAnimation : MonoBehaviour
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

    public void SetBoolAttack(bool value)
    {
        _animator.SetBool("Attack", value);
    }

    public void SetTriggerDead()
    {
        _animator.SetTrigger("Dead");
    }

    public void SetBoolMove(bool value)
    {
        _animator.SetBool("Move", value);
    }
}
