using Cinemachine;
using UnityEngine;

public class CinemachineSetFollow : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachine;
    private Transform player;

    void Awake()
    {
        cinemachine = GetComponent<CinemachineVirtualCamera>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        cinemachine.Follow = player;
    }
}
