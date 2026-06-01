using Cinemachine;
using GlobalEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.Rendering;
using UnityEngine;

public class PimenTalk : MonoBehaviour, ITalkable
{
    //public CustomPimenTalkInspectorObjects customWraithInspectorObjects;

    [SerializeField] private DialogueController dialogueController;

    [Header("Dialogues")]
    public DialogueText PimenMeetDialogueText;
    public DialogueText BeforeFirstBossDialogueText;
    public DialogueText WinFirstBossDialogueText;
    public DialogueText SawClawDialogueText;
    public DialogueText SawMnemirItemDialogueText;
    public DialogueText BeforeFirstFightRoomDialogueText;
    public DialogueText AfterFirstFightRoomDialogueText;
    public DialogueText TakeSecondArtifactDialogueText;
    public DialogueText BeforeFinalBossDialogueText;
    public DialogueText WinLastBossDialogueText;
    public DialogueText LastRoomDialogueText;
    public DialogueText CanNotOpenKeyDoorDialogueText;
    public DialogueText DontHaveAllArtifactDialogueText;
    public DialogueText EndingDialogueText;

    private GameObject _player;
    private GameObject _pimen;

    private DialogueText _currentDialogue;

    public bool IsPimenTalk {  get; set; }

    private void Awake()
    {
        if (dialogueController == null)
        {
            dialogueController = InitializeManager.Instance.dialogueController;

            if (dialogueController == null)
            {
                Debug.LogError($"DialogueController is null {dialogueController}");
            }
        }
        
        if (dialogueController != null)
        {
            dialogueController.gameObject.SetActive(false);
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        _pimen = GameObject.FindGameObjectWithTag("Pimen");
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.DIALOGUE && IsPimenTalk)
        {
            if (PlayerInput.Instance.AgreeButtonPressed)
            {
                Debug.Log($"[Update Pimen] AgreeButtonPressed={PlayerInput.Instance.AgreeButtonPressed}");
                dialogueController.DisplayNextParagraph(_currentDialogue); //Кнопкой Enter для пролистывания
            }
        }
    }

    public void Interact()
    {
        Talk(_currentDialogue);
    }

    public void Talk(DialogueText diaogueText) //Кнопкой E для взаимодействия
    {
        IsPimenTalk = true;

        if (_pimen != null)
        {
            _pimen.GetComponent<PimenMove>().enabled = false;
            _pimen.GetComponent<PimenAnimation>().SetIsMove(false);
        }

        TurnPlayerToNPC();

        _currentDialogue = diaogueText;
        dialogueController.StartDialogue(_currentDialogue);
    }

    #region Pimen Talk Methods

    public void FirstTalkWithPimen()
    {
        StartCoroutine(WalkAwayFromNPC(_player.transform.position, PimenMeetDialogueText));
        //PlayerAnimation.Instance.ResetAnimatorParameters();
        //Talk(PimenMeetDialogueText);
    }

    public void BeforeFirstBoss()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(BeforeFirstBossDialogueText);
    }

    public void WinFirstBoss()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(WinFirstBossDialogueText);
    }

    public void SawClaw()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(SawClawDialogueText);
    }

    public void SawMnemirItem()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(SawMnemirItemDialogueText);
    }

    public void BeforeFirstFightRoom()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(BeforeFirstFightRoomDialogueText);
    }

    public void AfterFirstFightRoom()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(AfterFirstFightRoomDialogueText);
    }

    public void TakeSecondArtifact()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(TakeSecondArtifactDialogueText);
    }

    public void BeforeFinalBoss()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(BeforeFinalBossDialogueText);
    }

    public void WinLastBoss()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(WinLastBossDialogueText);
    }

    public void CantOpenKeyDoor()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(CanNotOpenKeyDoorDialogueText);
    }

    public void LastRoom()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        dialogueController.isGivenArtifact = true;
        Talk(LastRoomDialogueText);
    }

    public void Ending()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        dialogueController.isFinalDialogue = true;
        Talk(EndingDialogueText);
    }

    public void DontHaveAllArtifact()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(DontHaveAllArtifactDialogueText);
    }

    private IEnumerator WalkAwayFromNPC(Vector3 initialPlayerPosition, DialogueText diaogueText)
    {
        while (!PlayerMove.Instance.IsGrounded)
        {
            yield return null;
        }

        float startX = initialPlayerPosition.x;
        float targetX = transform.position.x - 1.5f;
        float currentX = startX;


        float distance = Mathf.Abs(startX - targetX);
        float time = distance / 3f;
        float elapsedTime = 0f;

        PlayerView.Instance.StopPlayer();
        PlayerView.Instance.FreezePlayerWithDisableMove(true);
        PlayerAnimation.Instance.SetFloatSpeed(0.8f);

        if (PlayerView.Instance.PlayerModel.FacingRight)
        {
            PlayerMove.Instance.CallTurn();
        }

        while (elapsedTime < time && Mathf.Abs(startX - targetX) > 0.1)
        {
            elapsedTime += Time.deltaTime;
            currentX = Mathf.Lerp(startX, targetX, elapsedTime / time);
            _player.transform.position = new Vector3(currentX, _player.transform.position.y, _player.transform.position.z);
            yield return null;
        }
        PlayerView.Instance.FreezePlayerWithDisableMove(false);

        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(diaogueText);
    }

    #endregion

    private void TurnPlayerToNPC()
    {
        if (_player.transform.position.x < transform.position.x && !PlayerView.Instance.PlayerModel.FacingRight)
        {
            PlayerMove.Instance.CallTurn();
        }
        else if (_player.transform.position.x > transform.position.x && PlayerView.Instance.PlayerModel.FacingRight)
        {
            PlayerMove.Instance.CallTurn();
        }
    }
}
