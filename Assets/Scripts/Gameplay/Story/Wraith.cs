using UnityEditor.Rendering;
using UnityEngine;

public class Wraith : NPC, ITalkable
{
    [SerializeField] private DialogueText PimenMeetDialogueText;
    [SerializeField] private DialogueController dialogueController;


    private GameObject _player;
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
    }

    public override void Interact()
    {
        Talk(PimenMeetDialogueText);
    }

    public void Talk(DialogueText diaogueText)
    {
        TurnPlayerToNPC();
        dialogueController.DisplayNextParagraph(diaogueText);
    }

    public void FirstTalkWithPimen()
    {
        Talk(PimenMeetDialogueText);
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
