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
    public bool Mnemir { get; set; } = false;

    private void FixedUpdate()
    {
        if (PlayerInput.Instance.InteractPressed)
        {
            if (FullHeal)
            {
                _particles = FindAnyObjectByType<InstantiateParticles>(); //находится на сейвке
                PlayerView.Instance.SetCheckPoint(_particles.transform.position);
                _particles.InstantiatePollen();

                _interactHealEvent.Invoke(); //PlayerView
            }

            else if (Mnemir)
            {
                if (!PlayerView.Instance.PlayerModel.HasQuestMnemir)
                {
                    var mnemirPos = FindAnyObjectByType<MnemirView>().gameObject.transform.position;
                    var save = new Vector3(mnemirPos.x, mnemirPos.y + 0.1f, mnemirPos.z);

                    PlayerView.Instance.SetCheckPoint(save);
                    PlayerView.Instance.PlayerModel.SetHasQuestMnemir();
                    
                    SaveSystem.Save();
                    SaveSystem.AutoSave();
                }
            }
        }
    }
}
