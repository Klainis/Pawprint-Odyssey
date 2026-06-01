using System.Collections;
using TMPro;
using UnityEngine;

public class GuardianOwlMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _moveSpeedModifier = 1.5f;

    [SerializeField] private Transform _topPoint;

    private GuardianOwlView _guardianOwlView;
    private GuardianOwlAnimation _guardianOwlAnimation;

    private GameObject _player;
    private CapsuleCollider2D _bossCollider;

    private float _realMoveModifier = 1;
    private bool _moveToPlayer;

    private bool _facingRight = false;

    private void Awake()
    {
        _guardianOwlView = GetComponent<GuardianOwlView>();
        _guardianOwlAnimation = GetComponent<GuardianOwlAnimation>();
        _bossCollider = GetComponent<CapsuleCollider2D>();
        _player = InitializeManager.Instance.player;
    }

    public IEnumerator MoveUp()
    {
        _guardianOwlAnimation.SetMoveAnimation(true);

        while (Vector3.Distance(transform.position, _topPoint.position) > 0.01f)
        {
            if (_guardianOwlView.Model.IsDead)
            {
                break;
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                _topPoint.position,
                _moveSpeed * _realMoveModifier * Time.deltaTime);

            yield return null;
        }

        _guardianOwlAnimation.SetMoveAnimation(false);
    }

    public IEnumerator MoveToPlayer()
    {
        _guardianOwlAnimation.SetMoveAnimation(true);

        Vector3 playerPosition = _player.transform.position + new Vector3(0, _bossCollider.size.x / 2, 0);

        if (playerPosition.x < transform.position.x && _facingRight)
        {
            Turn();
        }
        else if (playerPosition.x > transform.position.x && !_facingRight)
        {
            Turn();
        }

        while (Vector3.Distance(transform.position, playerPosition) > 0.3f)
        {
            if (_guardianOwlView.Model.IsDead)
            {
                break;
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                playerPosition,
                _moveSpeed * _realMoveModifier * Time.deltaTime);

            yield return null;
        }

        _guardianOwlAnimation.SetMoveAnimation(false);
    }

    public void Turn()
    {
        Vector3 rotator;
        if (_facingRight)
        {
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            _facingRight = !_facingRight;
        }
        else
        {
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            _facingRight = !_facingRight;
        }
        transform.rotation = Quaternion.Euler(rotator);
    }

    public void ApplySpeedModifier()
    {
        _realMoveModifier = _moveSpeedModifier;
    }
    private IEnumerator OwlMove(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime);
            yield return null;
        }
    }
}
