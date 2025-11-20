using UnityEngine;
using Cinemachine;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] private float _shakeForce = 0.5f;

    private CinemachineImpulseSource _impusleSource;

    private void Awake()
    {
        _impusleSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(/*Vector2 direction*/)
    {
        Debug.Log("SHAKE");
        _impusleSource.GenerateImpulse(/*-direction * */_shakeForce);
    }
}
