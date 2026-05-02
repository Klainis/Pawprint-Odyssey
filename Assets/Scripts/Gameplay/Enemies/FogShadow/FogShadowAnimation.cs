using UnityEngine;

public class FogShadowAnimation : MonoBehaviour
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

    public void SetBoolInvisible(bool value)
    {
        _animator.SetBool("Invisible", value);
    }

    public void SetBoolMove(bool value)
    {
        _animator.SetBool("Move", value);
    }

    public void SetBoolPatrol(bool value)
    {
        _animator.SetBool("Patrol", value);
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
