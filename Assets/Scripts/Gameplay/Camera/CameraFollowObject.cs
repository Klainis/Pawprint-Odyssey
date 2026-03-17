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
    private float _initialOffsetX;

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

        _followCamera = CameraManager.Instance.CinemachineCamera;
        _transpoer = _followCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _initialOffsetX = _transpoer.m_TrackedObjectOffset.x;
    }

    private void Update()
    {
        transform.position = _playerTransfom.position;
    }

    public void CallTurn()
    {
        //LeanTween.rotateY(gameObject, DetermineRotation(), _flipRotationTime);
        _flipCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startXOffset = _transpoer.m_TrackedObjectOffset.x;
        float endXOffset = DetermineRotation();
        float XOffset = 0f;

        float elapsedTime = 0f;
        while(elapsedTime < _flipRotationTime)
        {
            elapsedTime += Time.deltaTime;

            XOffset = Mathf.Lerp(startXOffset, endXOffset, (elapsedTime / _flipRotationTime));
            _transpoer.m_TrackedObjectOffset.x = XOffset;

            yield return null;
        }
    }

    private float DetermineRotation()
    {
        _isFacingRight = !_isFacingRight;
        if (PlayerMove.Instance.WallCheck.localPosition.x < 0)
        {
            return _isFacingRight ? -_initialOffsetX : _initialOffsetX;
        }
        else
        {
            return _isFacingRight ? _initialOffsetX : -_initialOffsetX;
        }
    }
}
