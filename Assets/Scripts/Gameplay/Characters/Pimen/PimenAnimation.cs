using UnityEngine;

public class PimenAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetIsMove(bool value)
    {
        _animator.SetBool("IsMove", value);
    }

    public void SetIsTrick(bool value)
    {
        _animator.SetBool("IsTrick", value);
    }
}
