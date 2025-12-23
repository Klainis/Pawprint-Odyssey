using Cinemachine;
using UnityEngine;

public class CameraSetOffset : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _transposer;

    private PlayerMove _playerMove;

    private Transform _player;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = GetComponentInChildren<CinemachineFramingTransposer>();
    }
    private void Start()
    {
        _player = _virtualCamera.Follow;
        _playerMove = _player.gameObject.GetComponent<PlayerMove>();
        
    }

    private void FixedUpdate()
    {
        if (_playerMove.WallCheck.localPosition.x < 0)
        {
            Debug.Log("На стене");
            _transposer.m_TrackedObjectOffset.x = -1;
        }
        else
        {
            _transposer.m_TrackedObjectOffset.x = 1;
        }
        
    }
}
