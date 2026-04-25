using UnityEngine;

public class PileMovingAnimation : MonoBehaviour
{
    private Animator _animator;

    private float _randomTime;

    private bool _isMoving = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    //private void Start()
    //{
    //    _animator.SetTrigger("Moving");
    //    _randomTime = Random.Range(3f, 6f);
    //}

    private void Update()
    {
        _randomTime -= Time.deltaTime;

        if (_randomTime <= 0 && !_isMoving)
        {
            _isMoving = true;
            _randomTime = Random.Range(3f, 6f);
            _animator.SetBool("Moving", _isMoving);
        }
    }

    private void OnEnable()
    {
        _isMoving = true;
        _randomTime = Random.Range(3f, 6f);
        _animator.SetBool("Moving", _isMoving);
    }

    public void EndMovingAnimation()
    {
        _isMoving = false;
        _animator.SetBool("Moving", _isMoving);
    }
}
