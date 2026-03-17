using Cinemachine;
using UnityEngine;

public class CameraSetOffset : MonoBehaviour
{
    // Наверное можно удалить скрипт
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _transposer;

    private PlayerMove _playerMove;
    private PlayerView _playerView;

    private Transform _player;

    private float _initialOffsetX;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        _playerMove = PlayerMove.Instance;
        _playerView = PlayerView.Instance;
    }
    private void Start()
    {
        _initialOffsetX = _transposer.m_TrackedObjectOffset.x;
    }

    private void FixedUpdate()
    {
        if (_playerMove == null) return;
        if (_playerMove.WallCheck.localPosition.x < 0)
        {
            if (_playerView.PlayerModel.FacingRight)
            {
                _transposer.m_TrackedObjectOffset.x = _initialOffsetX;
            }
            else
            {
                _transposer.m_TrackedObjectOffset.x = -_initialOffsetX;
            }
        }
        else
        {
            _transposer.m_TrackedObjectOffset.x = _initialOffsetX;
        }
        
    }
}
