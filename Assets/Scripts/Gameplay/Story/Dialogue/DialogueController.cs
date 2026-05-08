using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering;
using GlobalEnums;
using static DialogueText;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;

    private Queue<LineData> paragraphs = new Queue<LineData>();

    private bool conversationEnded;

    private struct LineData
    {
        public string name;
        public string text;
    }

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                StartConversation(dialogueText);
            }
            else
            {
                EndConversation();
                return;
            }
        }

        LineData curentLine = paragraphs.Dequeue();

        NPCNameText.text = curentLine.name;
        NPCDialogueText.text = curentLine.text;

        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(DialogueText dialogueText)
    {
        GameManager.Instance.SetGameState(GameState.DIALOGUE);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        paragraphs.Clear();

        //for (int i = 0; i < dialogueText.dialogue.Count; i++)
        //{
        //    NPCNameText.text = dialogueText.dialogue[i].speakerName;

        //    for (int j = 0; j < dialogueText.dialogue[i].paragraphs.Length; j++)
        //    {
        //        paragraphs.Enqueue(dialogueText.dialogue[i].paragraphs[j]);
        //    }
        //}

        foreach (var speakerLine in dialogueText.dialogue)
        {
            foreach (string paragraphText in speakerLine.paragraphs)
            {
                paragraphs.Enqueue(new LineData
                {
                    name = speakerLine.speakerName,
                    text = paragraphText
                });
            }
        }
    }

    private void EndConversation()
    {
        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        GameManager.Instance.SetGameState(GameState.PLAYING);
    }
}
