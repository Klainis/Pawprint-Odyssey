using Cinemachine;
using UnityEngine;

public class CinemachineSetFollow : MonoBehaviour
{
    private CinemachineVirtualCamera _cinemachine;
    private Transform _folowObjectPlayer;

    private void Awake()
    {
        _cinemachine = GetComponent<CinemachineVirtualCamera>();
        _folowObjectPlayer = GameObject.FindWithTag("FollowObjectPlayer").GetComponent<Transform>();
        _cinemachine.Follow = _folowObjectPlayer;
        //_cinemachine.LookAt = _folowObjectPlayer;
    }
}
