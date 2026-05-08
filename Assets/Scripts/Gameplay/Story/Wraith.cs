using UnityEditor.Rendering;
using UnityEngine;

public class Wraith : NPC, ITalkable
{
    [SerializeField] private DialogueText PimenMeetDialogueText;
    [SerializeField] private DialogueController dialogueController;

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
    }

    public override void Interact()
    {
        Talk(PimenMeetDialogueText);
    }

    public void Talk(DialogueText diaogueText)
    {
        dialogueController.DisplayNextParagraph(PimenMeetDialogueText);
    }

    public void FirstTalkWithPimen()
    {
        Talk(PimenMeetDialogueText);
    }
}
