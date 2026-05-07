using UnityEngine;
using GlobalEnums;
using UnityEngine.Rendering;

public class PimenMove : MonoBehaviour
{
    [SerializeField] private Vector3 _baseOffset = new Vector3(-3, 1);
    [SerializeField] private float _smoothMovement = 0.5f;

    private Transform _player;

    private PimenAnimation _pimenAnimation;

    private Vector3 _playerPosition;
    private Vector3 _currentOffset;

    private float _turnCoefficient = 1;

    private bool _isFacingRight = false;

    private void Awake()
    {
        _pimenAnimation = GetComponent<PimenAnimation>();

        _player = InitializeManager.Instance.player.transform;
    }

    private void Start()
    {
        _currentOffset = _baseOffset;
        _playerPosition = _player.position + _currentOffset;
    }

    private void Update()
    {
        if (_player == null) return;

        HandleRotation();
        MoveWithPlayer();

        if (Vector3.Distance(transform.position, _playerPosition) > 0.1)
        {
            _pimenAnimation.SetIsMove(true);
        }
        else
        {
            _pimenAnimation.SetIsMove(false);
        }

        if (GameManager.Instance.GameState == GameState.IN_FIGHT_ROOM)
        {

            transform.rotation = _isFacingRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);

            if (transform.rotation.y == 0)
            {
                _turnCoefficient = 1;
            }
            else if (transform.rotation.y == 180)
            {
                _turnCoefficient = -1;
            }

            Vector3 leavePosition = new Vector3(transform.position.x + (50 * _turnCoefficient), transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, leavePosition, 10 * Time.deltaTime);
        }

    }

    private void MoveWithPlayer()
    {
        _currentOffset.x *= _turnCoefficient;
        _playerPosition = _player.position + _currentOffset;

        var step = _smoothMovement * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, _playerPosition, step);
    }

    private void HandleRotation()
    {
        if (PlayerMove.Instance.IsWall) return;

        _isFacingRight = PlayerView.Instance.PlayerModel.FacingRight;

        Vector3 targetOffset = _baseOffset;
        if (!_isFacingRight)
        {
            targetOffset.x = -_baseOffset.x;
        }

        if (_currentOffset != targetOffset)
        {
            _currentOffset = targetOffset;

            transform.rotation = _isFacingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
    }
}
