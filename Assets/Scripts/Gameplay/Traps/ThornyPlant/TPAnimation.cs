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
}
