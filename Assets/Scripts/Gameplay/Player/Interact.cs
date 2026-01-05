using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] private UnityEvent _interactHealEvent;

    [Space(5)]
    [SerializeField] private bool _fullHeal = false;
    [SerializeField] private bool _abilitiesTree = false;

    private InstantiateParticles _particles;

    public bool FullHeal { get { return _fullHeal; } set { _fullHeal = value; } }
    public bool AbilitiesTree { get { return _abilitiesTree; } set { _abilitiesTree = value; } }

    private void Update()
    {
        if (PlayerInput.Instance.InteractPressed)
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
