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
    private Vector2 lookVector;

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
        lookVector = PlayerInput.Instance.VectorLookAction;

        float desiredY = lookVector.y > 0? verticalRange : lookVector.y < 0 ? -verticalRange: 0;

        targetOffset = new Vector3(transposer.m_TrackedObjectOffset.x, desiredY, 0f);
        if (targetOffset != transposer.m_TrackedObjectOffset && !CameraManager.Instance.PanStarting)
        {
            transposer.m_TrackedObjectOffset = Vector3.MoveTowards(transposer.m_TrackedObjectOffset, targetOffset, 16 * Time.deltaTime);
        }
        //Debug.Log(transposer.m_TrackedObjectOffset);
        //Debug.Log(targetOffset);
    }
}
