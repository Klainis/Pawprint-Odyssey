using UnityEngine;

public class MnemirAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void SetBoolMove(bool value)
    {
        _animator.SetBool("Move", value);
    }

    private void SetBoolIdle(bool value)
    {
        _animator.SetBool("Idle", value);
    }

    public void SwitchMoving(bool isMoving)
    {
        SetBoolIdle(!isMoving);
        SetBoolMove(isMoving);
    }
}
