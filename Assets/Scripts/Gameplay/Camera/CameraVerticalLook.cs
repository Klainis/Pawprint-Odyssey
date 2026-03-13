using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraVerticalLook : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private InputActionReference lookAction;

    [SerializeField] private float verticalRange = 2f;
    [SerializeField] private float maxDistancedelta = 16f; 

    private CinemachineFramingTransposer transposer;
    private Vector3 targetOffset = Vector3.zero;
    private Vector2 look;

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
        }
    }

    private void OnDisable()
    {
        if (lookAction != null && lookAction.action != null)
            lookAction.action.Disable();
    }

    private void Update()
    {
        look = PlayerInput.Instance.VectorLookAction;

        float desiredY = look.y > 0? verticalRange : look.y < 0 ? -verticalRange: 0f;

        targetOffset = new Vector3(transposer.m_TrackedObjectOffset.x, desiredY, 0f);
        transposer.m_TrackedObjectOffset = Vector3.MoveTowards(transposer.m_TrackedObjectOffset, targetOffset, 16 * Time.deltaTime);
        //Debug.Log(transposer.m_TrackedObjectOffset);
        //Debug.Log(targetOffset);
    }
}
