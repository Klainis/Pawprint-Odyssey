using System.Collections;
using TMPro;
using UnityEngine;

public class GuardianOwlMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 20f;
    [SerializeField] private float _moveSpeedModifier = 1.5f;

    [SerializeField] private Transform _topPoint;

    private GuardianOwlView _guardianOwlView;

    private GameObject _player;
    private BoxCollider2D _bossCollider;

    private float _realMoveModifier = 1;
    private bool _moveToPlayer;

    private void Awake()
    {
        _guardianOwlView = GetComponent<GuardianOwlView>();
        _bossCollider = GetComponent<BoxCollider2D>();
        _player = InitializeManager.Instance.player;
    }

    public IEnumerator MoveUp()
    {
        while (Vector3.Distance(transform.position, _topPoint.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _topPoint.position,
                _moveSpeed * _realMoveModifier * Time.deltaTime);
            //Debug.Log($"Target Position {targetPosition}");
            //Debug.Log($"Self Position {transform.position}");
            yield return null;
        }
    }

    public IEnumerator MoveToPlayer()
    {
        Vector3 playerPosition = _player.transform.position;

        while (true)
        {
            float _bossBottomY = _bossCollider.bounds.min.y;
            Vector3 bossPosition = new Vector3(transform.position.x, _bossBottomY, transform.position.z);

            if (Vector3.Distance(transform.position, playerPosition) > 0.3f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    playerPosition,
                    _moveSpeed * _realMoveModifier * Time.deltaTime);
            }
            else
            {
                break;
            }

            yield return null;
        }
        Debug.Log("Движение закончилось");
    }

    public void ApplySpeedModifier()
    {
        _realMoveModifier = _moveSpeedModifier;
    }
    private IEnumerator OwlMove(Vector3 targetPosition, float speed)
    {
        //if (_moveToPlayer)
        //{
        //float _bossBottomY = _bossCollider.bounds.min.y;
        //Vector3 bossPosition = new Vector3(transform.position.x, _bossBottomY, transform.position.z);

        //while (Vector3.Distance(bossPosition, targetPosition) > 0.01f)
        //{
        //    transform.position = Vector3.MoveTowards(
        //        transform.position,
        //        targetPosition,
        //        speed * Time.deltaTime);
        //    Debug.Log($"Target Position {targetPosition}");
        //    Debug.Log($"Self Position {transform.position}");
        //    yield return null;
        //}
        //    _moveToPlayer = false;
        //}
        //else
        //{
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime);
            //Debug.Log($"Target Position {targetPosition}");
            //Debug.Log($"Self Position {transform.position}");
            yield return null;
        }
        //}
    }
}
