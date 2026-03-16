using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance {  get { return _instance; } }

    [SerializeField] private List<CinemachineVirtualCamera> _allConemachineCameras;
    [SerializeField] private List<CinemachineVirtualCamera> _playerConemachineCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    [SerializeField] private float _fallSpeedYDampingChangeThreshold = -15f;

    public float FallSpeedYDampingChangeTreshold { get { return _fallSpeedYDampingChangeThreshold; } }
    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _cinemachineTransposer;

    public CinemachineVirtualCamera CinemachineCamera { get { return _currentCamera; } set { _currentCamera = value; } }

    private float _normYPanAmount;

    private Coroutine _lerpYPanCoroutine;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;

        //foreach(var cam in _playerConemachineCameras)
        //{
        //    if (cam.enabled)
        //    {
        //        _currentCamera = cam;

        //        _cinemachineTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        //        _normYPanAmount = _cinemachineTransposer.m_YDamping;
        //    }
        //}
    }
    #region Add Cinemachine Camera in List
    //public void AddCinemachineCameraInList()
    //{
    //    //var virtualCameras = GameObject.FindGameObjectsWithTag("CinemachineFollowCamera");
    //    var virtualCameras = FindObjectsOfType<CinemachineVirtualCamera>();

    //    foreach (var cam in virtualCameras)
    //    {
    //        _allConemachineCameras.Add(cam);

    //        if (cam.CompareTag("CinemachineFollowCamera"))
    //        {
    //            _playerConemachineCameras.Add(cam);
    //        }
    //    }
    //}

    public void SetCurrentCinemachinecamera(CinemachineVirtualCamera cam)
    {
        if (cam != null)
        {
            Debug.Log(cam.gameObject.name);
            _currentCamera = cam;

            _cinemachineTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _normYPanAmount = _cinemachineTransposer.m_YDamping;
        }
        else
        {
            Debug.Log("Передана Virtualcamera null");
        }
    }
    #endregion

    #region Lerp Y Damping
    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampamount = _fallPanAmount;
        float endDampamount = 0f;

        if (isPlayerFalling)
        {
            endDampamount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampamount = _normYPanAmount;
        }

        float elapsedTime = 0f;
        while(elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampamount, endDampamount, (elapsedTime / _fallYPanTime));
            _cinemachineTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }
    #endregion
}
