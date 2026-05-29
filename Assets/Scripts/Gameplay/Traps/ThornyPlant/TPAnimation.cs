using UnityEngine;

public class TPAnimation : MonoBehaviour
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

    public void SetBoolHide(bool value)
    {
        _animator.SetBool("Hide", value);
    }

    public void SetBoolReveal(bool value)
    {
        _animator.SetBool("Reveal", value);
    }

    public void SetBoolAttack(bool value)
    {
        _animator.SetBool("Attack", value);
    }
}
