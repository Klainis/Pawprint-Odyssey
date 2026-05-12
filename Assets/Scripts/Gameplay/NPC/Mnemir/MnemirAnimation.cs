using UnityEngine;

public class MnemirAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetBoolMove(bool value)
    {
        _animator.SetBool("Move", value);
    }

    public void SetBoolIdle(bool value)
    {
        _animator.SetBool("Idle", value);
    }
}
