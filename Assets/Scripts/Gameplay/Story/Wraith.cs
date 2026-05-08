using UnityEditor.Rendering;
using UnityEngine;

public class Wraith : NPC, ITalkable
{
    [SerializeField] private DialogueText PimenMeetDialogueText;
    [SerializeField] private DialogueText BeforeFirstBossDialogueText;
    [SerializeField] private DialogueText WinFirstBossDialogueText;
    [SerializeField] private DialogueText SawClawDialogueText;
    [SerializeField] private DialogueText BeforeFirstFightRoomDialogueText;
    [SerializeField] private DialogueText AfterFirstFightRoomDialogueText;
    [SerializeField] private DialogueText TakeSecondArtifactDialogueText;
    [SerializeField] private DialogueText BeforeFinalBossDialogueText;
    [SerializeField] private DialogueText WinLastBossDialogueText;
    [SerializeField] private DialogueText LastRoomDialogueText;
    [SerializeField] private DialogueController dialogueController;

    private GameObject _player;
    private GameObject _pimen;
    //переопределение Interact()

    private void Awake()
    {
        if (dialogueController == null)
        {
            dialogueController = FindAnyObjectByType<DialogueController>();

            if (dialogueController == null)
            {
                Debug.LogError($"DialogueController is {dialogueController}");
            }
        }
        
        if (dialogueController != null)
        {
            dialogueController.gameObject.SetActive(false);
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        _pimen = GameObject.FindGameObjectWithTag("Pimen");
    }

    public override void Interact()
    {
        Talk(PimenMeetDialogueText);
    }

    public void Talk(DialogueText diaogueText)
    {
        _pimen.gameObject.GetComponent<PimenMove>().enabled = false;
        TurnPlayerToNPC();
        dialogueController.DisplayNextParagraph(diaogueText);
    }

    public void FirstTalkWithPimen()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(PimenMeetDialogueText);
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

    public void LastRoom()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(LastRoomDialogueText);
    }

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
