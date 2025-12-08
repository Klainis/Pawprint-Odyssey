using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private UnityEvent _interactHealEvent;
    [SerializeField] private ParticleSystem _pollenExplosionParticle;

    private ParticleSystem _pollenExplosionInstance;

    public bool FullHeal { get; set; }

    private void Update()
    {
        if (_interactAction != null && _interactAction.action != null)
        {
            if (_interactAction.action.WasPressedThisFrame())
            {
                if (FullHeal)
                {
                    InstantiateParticles();
                    _interactHealEvent.Invoke(); //PlayerView
                }
            }
        }
    }

    private void InstantiateParticles()
    {
        Quaternion _pollenRotation = Quaternion.identity;
        _pollenExplosionInstance = Instantiate(_pollenExplosionParticle, transform.position, _pollenRotation);
    }
}
