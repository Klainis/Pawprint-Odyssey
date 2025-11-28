using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private UnityEvent interactHealEvent;

    public bool Heal { get; set; }

    private void Update()
    {
        if (interactAction != null && interactAction.action != null)
            if (interactAction.action.WasPressedThisFrame())
                if (Heal)
                    interactHealEvent.Invoke();
    }
}
