using Cinemachine;
using UnityEngine;

public class CameraSetOffset : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _transposer;

    private PlayerMove _playerMove;

    private Transform _player;

    private float _initialOffsetX;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = GetComponentInChildren<CinemachineFramingTransposer>();
    }
    private void Start()
    {
        _player = _virtualCamera.Follow;
        _playerMove = _player.gameObject.GetComponent<PlayerMove>();

        _initialOffsetX = _transposer.m_TrackedObjectOffset.x;
    }

    private void FixedUpdate()
    {
        if (_playerMove == null) return;
        if (_playerMove.WallCheck.localPosition.x < 0)
        {
            _transposer.m_TrackedObjectOffset.x = -_initialOffsetX;
        }
        else
        {
            _transposer.m_TrackedObjectOffset.x = _initialOffsetX;
        }
        
    }
}
