using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private UnityEvent _interactHealEvent;

    private InstantiateParticles _particles;

    public bool FullHeal { get; set; }

    private void Update()
    {
        if (_interactAction != null && _interactAction.action != null)
        {
            if (_interactAction.action.WasPressedThisFrame())
            {
                if (FullHeal)
                {
                    _particles = FindAnyObjectByType<InstantiateParticles>();
                    _particles.InstantiatePollen();

                    _interactHealEvent.Invoke(); //PlayerView
                }
            }
        }
    }
}
