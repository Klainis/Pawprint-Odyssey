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
                    _particles = FindAnyObjectByType<InstantiateParticles>();//находится на сейвке
                    PlayerView.Instance.SetCheckPoint(_particles.transform.position - new Vector3(0, 2, 0)); //смещаю чуть вниз, чтобы на земле был игрок, лень настраивать 
                    _particles.InstantiatePollen();

                    _interactHealEvent.Invoke(); //PlayerView
                }
            }
        }
    }
}
