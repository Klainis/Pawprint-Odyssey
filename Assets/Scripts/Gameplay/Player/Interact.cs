using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private UnityEvent interactHealEvent;

    private Gamepad gamepad;
    private InteractArea interactArea;

    [HideInInspector] public bool heal;

    void Start()
    {
        gamepad = Gamepad.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (interactAction != null && interactAction.action != null)
        {
            if (interactAction.action.WasPressedThisFrame())
            {
                if (heal)
                {
                    interactHealEvent.Invoke();
                }
            }
        }
        
    }

}
