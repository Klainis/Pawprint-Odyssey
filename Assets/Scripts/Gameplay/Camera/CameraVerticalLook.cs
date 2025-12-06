using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraVerticalLook : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private InputActionReference lookAction;

    [SerializeField] private float verticalRange = 2f;    
    //[SerializeField] private float smoothTime = 0.12f;    

    private CinemachineFramingTransposer transposer;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetOffset = Vector3.zero;
    private float targetCameraY;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();

        if (vcam == null)
        {
            enabled = false;
            return;
        }

        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (transposer == null)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (lookAction != null && lookAction.action != null)
        {
            lookAction.action.Enable();
            Debug.Log("Look enabled");
        }
    }

    private void OnDisable()
    {
        if (lookAction != null && lookAction.action != null)
            lookAction.action.Disable();
    }

    private void Update()
    {
        if (lookAction == null || lookAction.action == null) return;

        Vector2 look = lookAction.action.ReadValue<Vector2>();
        //Debug.Log(look);

        float desiredY = look.y * verticalRange;
        targetCameraY = desiredY > 0f ? verticalRange : desiredY < 0f ? -verticalRange : 0;
        targetOffset = new Vector3(transposer.m_TrackedObjectOffset.x, targetCameraY, 0f);

        float distance = Mathf.Abs(transposer.m_TrackedObjectOffset.y - targetOffset.y);
        float dynamicSmooth = Mathf.Lerp(0.05f, 0.2f, distance / verticalRange);

        //transposer.m_TrackedObjectOffset = Vector3.SmoothDamp(
        //    transposer.m_TrackedObjectOffset,
        //    targetOffset,
        //    ref velocity,
        //    dynamicSmooth
        //);

        //Debug.Log(transposer.m_TrackedObjectOffset);
    }
}
