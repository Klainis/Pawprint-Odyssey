using UnityEngine;

public class FightDoor : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void CloseDoor(bool isClose)
    {
        _animator.SetBool("Close", isClose);
    }
}
