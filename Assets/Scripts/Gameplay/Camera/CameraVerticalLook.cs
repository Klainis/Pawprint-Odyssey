using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using GlobalEnums;

public class CameraVerticalLook : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private InputActionReference lookAction;

    [SerializeField] private float verticalRange = 2f;
    [SerializeField] private float maxDistancedelta = 16f; 

    private CinemachineFramingTransposer transposer;
    private Vector3 targetOffset = Vector3.zero;
    private Vector2 lookVector;
    private Vector3 _initialOffset;

    private float _initialOffsetY = 0f;

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

        _initialOffset = transposer.m_TrackedObjectOffset;
        _initialOffsetY = transposer.m_TrackedObjectOffset.y;
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
        if (GameManager.Instance.GameState == GameState.DIALOGUE && transposer.m_TrackedObjectOffset != _initialOffset)
        {
            transposer.m_TrackedObjectOffset = Vector3.MoveTowards(transposer.m_TrackedObjectOffset, _initialOffset, 16 * Time.deltaTime);
            return;
        }

        lookVector = PlayerInput.Instance.VectorLookAction;

        float desiredY = lookVector.y > 0.5? verticalRange : lookVector.y < -0.5 ? -verticalRange: _initialOffsetY;

        targetOffset = new Vector3(transposer.m_TrackedObjectOffset.x, desiredY, transposer.m_TrackedObjectOffset.z);
        if (targetOffset != transposer.m_TrackedObjectOffset && !CameraManager.Instance.PanStarting)
        {
            transposer.m_TrackedObjectOffset = Vector3.MoveTowards(transposer.m_TrackedObjectOffset, targetOffset, 16 * Time.deltaTime);
        }
        
    }
}
