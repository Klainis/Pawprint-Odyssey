using System;
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
    public string MnemirQuestObject { get; set; } = "";

    public event Action OnCompleteMnemirQuest;

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
                else if (PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects.Count == 3
                    && !PlayerView.Instance.PlayerModel.MnemirQuestRewarded)
                {
                    OnCompleteMnemirQuest?.Invoke();
                    PlayerView.Instance.PlayerModel.SetMnemirQuestRewarded();

                                var mnemirPos = FindAnyObjectByType<MnemirView>().gameObject.transform.position;
                    var save = new Vector3(mnemirPos.x, mnemirPos.y + 0.1f, mnemirPos.z);

                    PlayerView.Instance.SetCheckPoint(save);
                    PlayerView.Instance.PlayerModel.SetHasQuestMnemir();

                    SaveSystem.Save();
                    SaveSystem.AutoSave();
                }
            }

            else if (MnemirQuestObject.Length > 0)
            {
                if (!PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects.Contains(MnemirQuestObject))
                {
                    PlayerView.Instance.PlayerModel.AddObjectToMnemirQuestCollectedObjects(MnemirQuestObject);

                    SaveSystem.Save();
                    SaveSystem.AutoSave();
                }
            }
        }
    }
}
