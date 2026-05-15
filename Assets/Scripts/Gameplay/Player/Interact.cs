using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    private readonly List<string> _mnemirObjectsRoomNames = new() { "F_Room_06", "F_Room_31", "F_FightRoom_02" };

    public bool FullHeal { get { return _fullHeal; } set { _fullHeal = value; } }
    public bool AbilitiesTree { get { return _abilitiesTree; } set { _abilitiesTree = value; } }
    public bool Mnemir { get; set; } = false;
    public string MnemirQuestObject { get; set; } = "";
    public bool Artefact { get; set; } = false;
    public GameObject artefactObject { get; set; }

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
                    MapManager.Instance.SetMnemirQuestObjectsIcons(_mnemirObjectsRoomNames);

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

                    SaveSystem.Save();
                    SaveSystem.AutoSave();
                }
            }

            else if (MnemirQuestObject.Length > 0)
            {
                if (!PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects.Contains(MnemirQuestObject))
                {
                    MapManager.Instance.RemoveMnemirQuestObjectsIcon(MnemirQuestObject);

                    PlayerView.Instance.PlayerModel.AddObjectToMnemirQuestCollectedObjects(MnemirQuestObject);

                    SaveSystem.Save();
                    SaveSystem.AutoSave();
                }
            }

            else if (Artefact)
            {
                TakeArtefact();
            }
        }
    }

    private void TakeArtefact()
    {
        PlayerView.Instance.PlayerModel.AddArtefact();
        Destroy(artefactObject);
    }
}
