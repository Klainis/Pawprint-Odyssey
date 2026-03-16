using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetCameraInCameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera _camera;
    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
        if (_camera != null)
        {
            CameraManager.Instance.SetCurrentCinemachinecamera(_camera);
            Debug.Log(_camera.gameObject.name);
        }
        else
        {
            Debug.Log($"Не найдена CinemachineVirtualcamera на сцене {SceneManager.GetActiveScene().name}");
        }
    }
}
