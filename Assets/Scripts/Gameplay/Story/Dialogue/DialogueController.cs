using UnityEngine;
using TMPro;
using System.Collections.Generic;
//using UnityEditor.Rendering;
using GlobalEnums;
using static DialogueText;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;

    public bool isFinalDialogue = false;

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

    private void Awake()
    {
        InitializeManager.Instance.dialogueController = this;
        gameObject.SetActive(false);
    }

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        //заполняем Queue, если ничего нет в очереди
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

        //Печатаем то, что есть в очереди
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

        PlayerView.Instance.FreezePlayerWithDisableMove(true);
        PlayerAnimation.Instance.ResetAnimatorParameters();

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

        var pimen = GameObject.FindGameObjectWithTag("Pimen");
        if (pimen != null && !isFinalDialogue)
        {
            pimen.GetComponent<PimenMove>().enabled = true;
            pimen.GetComponent<PimenTalk>().IsPimenTalk = false;
        }

        var mnemir = GameObject.FindGameObjectWithTag("NPC");
        if (mnemir != null)
        {
            mnemir.GetComponent<MnemirTalk>().IsMnemirTalk = false;
        }
        //GameManager.Instance.SetGameState(GameState.PLAYING);
        //PlayerView.Instance.FreezePlayerWithDisableMove(false);
        if (isFinalDialogue)
        {
            isFinalDialogue = false;
            StartArtifactScene();
        }
        else
        {
            GameManager.Instance.SetGameState(GameState.PLAYING);
            PlayerView.Instance.FreezePlayerWithDisableMove(false);

            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

    }

    private void StartArtifactScene()
    {
        if (!PlayerView.Instance.PlayerModel.FacingRight)
        {
            PlayerMove.Instance.CallTurn();
        }

        EndGameManager.Instance.GiveArtifactScene();

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
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
