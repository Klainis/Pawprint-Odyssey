using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering;
using GlobalEnums;
using static DialogueText;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;

    private Queue<LineData> paragraphs = new Queue<LineData>();
    private LineData _currentLine;

    private float _typeSpeed;

    private bool _conversationEnded;
    private bool _isTyping;

    private struct LineData
    {
        public string name;
        public string text;
    }

    private Coroutine _typeDialogueCoroutine;

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        //заполняем Queue, если ничего не в очереди
        if (paragraphs.Count == 0)
        {
            if (!_conversationEnded)
            {
                StartConversation(dialogueText);
            }
            else if (_conversationEnded && !_isTyping)
            {
                EndConversation();
                return;
            }
        }

        //Печатаем то, что если в очереди
        if (!_isTyping)
        {
            _currentLine = paragraphs.Dequeue();

            NPCNameText.text = _currentLine.name;
            _typeDialogueCoroutine = StartCoroutine(TypeDialogueText(_currentLine.text));
        }
        else
        {
            FinishParagraphEarly();
        }

        if (paragraphs.Count == 0)
        {
            _conversationEnded = true;
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

        foreach (var speakerLine in dialogueText.dialogue)
        {
            //if (speakerLine.typeSpeed < 0) //Error value как дфлаг для дефолтного значения
            //{
            //    _typeSpeed = dialogueText.DefaultTypeSpeed;
            //}
            //else
            //{
            //    _typeSpeed = speakerLine.typeSpeed;
            //}

            _typeSpeed = dialogueText.DefaultTypeSpeed;

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
        _conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        GameManager.Instance.SetGameState(GameState.PLAYING);
    }

    private IEnumerator TypeDialogueText(string p)
    {
        _isTyping = true;

        NPCDialogueText.text = "";

        string originalText = p;
        string displayedText = "";
        int alphaINdex = 0;

        foreach (char c in p.ToCharArray())
        {
            alphaINdex++;
            NPCDialogueText.text = originalText;

            displayedText = NPCDialogueText.text.Insert(alphaINdex, HTML_ALPHA);
            NPCDialogueText.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_TIME / _typeSpeed);
        }

        _isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        StopCoroutine(_typeDialogueCoroutine);

        NPCDialogueText.text = _currentLine.text;

        _isTyping = false;
    }
}
