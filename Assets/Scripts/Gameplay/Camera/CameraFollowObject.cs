using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    private static CameraFollowObject _instance;
    public static CameraFollowObject Instance {  get { return _instance; } }

    [SerializeField] private float _flipRotationTime = 0.5f;

    private CinemachineVirtualCamera _followCamera;
    private CinemachineFramingTransposer _transpoer;

    private Transform _playerTransfom;
    public Transform playerTransfom { get { return _playerTransfom; } set { _playerTransfom = value; } }
    
    private bool _isFacingRight;
    private bool _isFlipping;

    private float _initialOffsetX;
    private float _wallOffsetX = 0f;
    private float _currentOffsetX;

    private Coroutine _flipCoroutine;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;

        _playerTransfom = GameObject.FindGameObjectWithTag("Player").transform;
        _isFacingRight = PlayerView.Instance.PlayerModel.FacingRight;
    }

    private void Update()
    {
        if (_playerTransfom != null)
        {
            transform.position = _playerTransfom.position;

            if (PlayerMove.Instance != null)
            {
                if (PlayerMove.Instance.IsWall || PlayerMove.Instance.IsWallJumping || PlayerMove.Instance.IsWallRunJumping)
                {
                    _currentOffsetX = _wallOffsetX;
                }
                else
                {
                    _currentOffsetX = _initialOffsetX;
                }
            }
        }
        else
        {
            Debug.Log("Не удалось получить Transform Player для Follow Camera Object");
        }
    }

    public void SetFollowCamera(CinemachineVirtualCamera followCamera)
    {
        _followCamera = followCamera;
        _transpoer = _followCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _initialOffsetX = _transpoer.m_TrackedObjectOffset.x;
        _currentOffsetX = _initialOffsetX;
    }

    public void CallCameraTurn()
    {
        if (_flipCoroutine != null)
            StopCoroutine(_flipCoroutine);

        _flipCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        _isFlipping = true;

        float startXOffset = _transpoer.m_TrackedObjectOffset.x;
        float endXOffset = DetermineRotation();
        float XOffset = 0f;

        float elapsedTime = 0f;
        while(elapsedTime < _flipRotationTime)
        {
            elapsedTime += Time.deltaTime;

            XOffset = Mathf.SmoothStep(startXOffset, endXOffset, (elapsedTime / _flipRotationTime));
            _transpoer.m_TrackedObjectOffset.x = XOffset;

            yield return null;
        }

        _transpoer.m_TrackedObjectOffset.x = endXOffset;
        _isFlipping = false;
    }

    private float DetermineRotation()
    {
        //Debug.Log(PlayerView.Instance.PlayerModel.FacingRight);
        //Debug.Log(_currentOffsetX);
        return PlayerView.Instance.PlayerModel.FacingRight ? _currentOffsetX : -_currentOffsetX;
    }
}
