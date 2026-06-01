using NUnit.Framework;
using System;
using System.Collections;
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

    private ShowEducation _showEducation;

    public bool FullHeal { get { return _fullHeal; } set { _fullHeal = value; } }
    public bool AbilitiesTree { get { return _abilitiesTree; } set { _abilitiesTree = value; } }
    public bool Mnemir { get; set; } = false;
    public string MnemirQuestObject { get; set; } = "";
    public bool Artefact { get; set; } = false;
    public bool DoubleJumpItem { get; set; } = false;
    public bool ClawItem { get; set; } = false;
    public bool MnemirKey { get; set; } = false;
    public bool MnemirKeyLock { get; set; } = false;
    public bool CanInteractWithMnemir { get; set; } = true;
    public bool CanInteractWithKeyLock { get; set; } = true;
    public GameObject artefactObject { get; set; }
    public GameObject doubleJumpItemObject { get; set; }
    public GameObject clawItemObject { get; set; }
    public GameObject mnemirKeyObject { get; set; }



    public event Action OnCompleteMnemirQuest;

    private Coroutine _takeMnemirQuestCoroutine;
    private Coroutine _takeRewardCoroutine;

    //private void Awake()
    //{
    //    showEducation = FindAnyObjectByType<ShowEducation>();
    //}

    private void Update()
    {

        if (GameManager.Instance.GameState == GlobalEnums.GameState.DIALOGUE)
        {
            return;
        }

        if (PlayerInput.Instance.InteractPressed)
        {
            if (FullHeal)
            {
                _particles = FindAnyObjectByType<InstantiateParticles>(); //находится на сейвке
                PlayerView.Instance.SetCheckPoint(_particles.transform.position);
                _particles.InstantiatePollen();

                _interactHealEvent.Invoke(); //PlayerView
            }

            else if (Artefact)
            {
                TakeArtefact();
            }

            else if (DoubleJumpItem)
            {
                EnableDoubleJump();
            }

            else if (ClawItem)
            {
                EnableClaw();
            }

            else if (MnemirKey)
            {
                TakeMnemirKey();
            }

            else if (MnemirKeyLock)
            {
                if (PlayerView.Instance.PlayerModel.HasMnemirKey)
                {
                    if (CanInteractWithKeyLock)
                    {
                        var keyLock = FindAnyObjectByType<KeyLockInteract>();

                        if (keyLock != null)
                        {
                            Debug.Log("Открываем дверь ключом");
                            CanInteractWithKeyLock = false;
                            keyLock.InstantiateParticle();

                            keyLock.DestroyKeyDoor();
                        }
                        else
                        {
                            Debug.LogError($"keyLock is NULL: {keyLock}");
                        }
                    }
                }
                else
                {
                    var talk = FindAnyObjectByType<PimenTalk>();
                    talk.CantOpenKeyDoor();
                }
            }

            else if (Mnemir)
            {
                if (!PlayerView.Instance.PlayerModel.HasQuestMnemir && CanInteractWithMnemir)
                {
                    Debug.Log("Подошли в первый раз");

                    var talk = FindAnyObjectByType<MnemirTalk>();
                    talk.BeforeMnemirQuest();

                    _takeMnemirQuestCoroutine = StartCoroutine(TakeMnemirQuest());

                    return;
                }
                else if (PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects.Count == 3
                    && !PlayerView.Instance.PlayerModel.MnemirQuestRewarded
                    && CanInteractWithMnemir)
                {
                    Debug.Log("Подошли после выполнения квеста");

                    var talk = FindAnyObjectByType<MnemirTalk>();
                    talk.AfterMnemirQuest();

                    _takeRewardCoroutine = StartCoroutine(TakeReward());

                    return;
                }
                else if (PlayerView.Instance.PlayerModel.HasQuestMnemir
                    && CanInteractWithMnemir)
                {
                    Debug.Log("Подошли еще раз");
                    var talk = FindAnyObjectByType<MnemirTalk>();
                    talk.DuringMnemirQuest();

                    return;

                }
            }

            else if (MnemirQuestObject.Length > 0)
            {
                if (!PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects.Contains(MnemirQuestObject))
                {
                    MapManager.Instance.RemoveMnemirQuestObjectsIcon(MnemirQuestObject);

                    PlayerView.Instance.PlayerModel.AddObjectToMnemirQuestCollectedObjects(MnemirQuestObject);

                    //SaveSystem.Save();
                    //SaveSystem.AutoSave();
                    SaveSystem.AutoSaveSimple();
                }
            }
        }
    }

    private void TakeArtefact()
    {
        PlayerView.Instance.PlayerModel.AddArtefact();
        Destroy(artefactObject);

        if (PlayerView.Instance.PlayerModel.ArtefactCollected == 2)
        {
            var talk = FindAnyObjectByType<PimenTalk>();
            talk.TakeSecondArtifact();
        }
    }

    private void TakeMnemirKey()
    {
        PlayerView.Instance.PlayerModel.SetHasMnemirKey();
        Debug.Log(PlayerView.Instance.PlayerModel.HasMnemirKey);
        Destroy(mnemirKeyObject);
        SaveSystem.AutoSave();
    }

    private void EnableDoubleJump()
    {
        PlayerView.Instance.PlayerModel.SetHasDoubleJump();
        Destroy(doubleJumpItemObject);
        SaveSystem.AutoSave();
    }

    private void EnableClaw()
    {
        var receivingClaw = FindAnyObjectByType<ReceivingClaw>();
        receivingClaw.EnableClaw(clawItemObject);
    }

    private IEnumerator TakeMnemirQuest()
    {
        while (GameManager.Instance.GameState == GlobalEnums.GameState.DIALOGUE)
        {
            yield return null;
        }

        if (PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects.Count == 0)
        {
            MapManager.Instance.SetMnemirQuestObjectsIcons(_mnemirObjectsRoomNames);
        }
        else
        {
            MapManager.Instance.SetMnemirQuestObjectsIcons(PlayerView.Instance.PlayerModel.MnemirQuestCollectedObjects);
        }

        var mnemirPos = FindAnyObjectByType<MnemirView>().gameObject.transform.position;
        var save = new Vector3(mnemirPos.x, mnemirPos.y + 0.1f, mnemirPos.z);

        PlayerView.Instance.SetCheckPoint(save);
        PlayerView.Instance.PlayerModel.SetHasQuestMnemir();

        SaveSystem.Save();
        SaveSystem.AutoSave();
        //SaveSystem.AutoSaveSimple();
    }

    private IEnumerator TakeReward()
    {
        yield return new WaitForSeconds(0.5f);

        while (GameManager.Instance.GameState == GlobalEnums.GameState.DIALOGUE)
        {
            yield return null;
        }

        OnCompleteMnemirQuest?.Invoke();
        PlayerView.Instance.PlayerModel.SetMnemirQuestRewarded();

        CanInteractWithMnemir = false;

        var mnemirView = FindAnyObjectByType<MnemirView>();
        mnemirView.zoneCheck.GetComponent<BoxCollider2D>().enabled = false;
        mnemirView.educationCanvas.SetActive(false);

        var mnemirPos = mnemirView.gameObject.transform.position;
        var save = new Vector3(mnemirPos.x, mnemirPos.y + 0.1f, mnemirPos.z);

        PlayerView.Instance.SetCheckPoint(save);

        SaveSystem.Save();
        SaveSystem.AutoSave();
        //SaveSystem.AutoSaveSimple();
    }
}
