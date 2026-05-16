using GlobalEnums;
using UnityEditor;
using UnityEngine;

public class MnemirTalk : MonoBehaviour, ITalkable
{
    //public CustomMnemirTalkInspectorObjects customWraithInspectorObjects;

    [SerializeField] private DialogueController dialogueController;

    [Header("Dialogues")]
    public DialogueText BeforeMnemirQuestDialogueText;
    public DialogueText DuringMnemirQuestDialogueText;
    public DialogueText AfterMnemirQuestDialogueText;

    private GameObject _player;
    private GameObject _pimen;

    private DialogueText _currentDialogue;

    public bool IsMnemirTalk {  get; set; }

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
        if (GameManager.Instance.GameState == GameState.DIALOGUE && IsMnemirTalk)
        {
            if (PlayerInput.Instance.AgreeButtonPressed)
            {
                Interact();
            }
        }
    }

    public void Interact()
    {
        Talk(_currentDialogue);
    }

    public void Talk(DialogueText diaogueText)
    {
        IsMnemirTalk = true;

        if (_pimen != null)
        {
            _pimen.GetComponent<PimenMove>().enabled = false;
            _pimen.GetComponent<PimenAnimation>().SetIsMove(false);
        }

        TurnPlayerToNPC();

        _currentDialogue = diaogueText;
        dialogueController.DisplayNextParagraph(diaogueText);
    }

    #region Mnemir Methods
    public void BeforeMnemirQuest()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(BeforeMnemirQuestDialogueText);
    }

    public void DuringMnemirQuest()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(DuringMnemirQuestDialogueText);
    }

    public void AfterMnemirQuest()
    {
        PlayerAnimation.Instance.ResetAnimatorParameters();
        Talk(AfterMnemirQuestDialogueText);
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

//[System.Serializable]
//public class CustomMnemirTalkInspectorObjects
//{
//    [HideInInspector] public DialogueText BeforeMnemirQuestDialogueText;
//    [HideInInspector] public DialogueText DuringMnemirQuestDialogueText;
//    [HideInInspector] public DialogueText AfterMnemirQuestDialogueText;
//}

//#if UNITY_EDITOR
//[CustomEditor(typeof(PimenTalk))]
//public class MyMnemirScriptEditor : Editor
//{
//    MnemirTalk mnemir;

//    private void OnEnable()
//    {
//        mnemir = (MnemirTalk)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        mnemir.customWraithInspectorObjects.BeforeMnemirQuestDialogueText = EditorGUILayout.ObjectField("Before Mnemir Quest", mnemir.customWraithInspectorObjects.BeforeMnemirQuestDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        mnemir.customWraithInspectorObjects.DuringMnemirQuestDialogueText = EditorGUILayout.ObjectField("During Mnemir Quest", mnemir.customWraithInspectorObjects.DuringMnemirQuestDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        mnemir.customWraithInspectorObjects.AfterMnemirQuestDialogueText = EditorGUILayout.ObjectField("After Mnemir Quest", mnemir.customWraithInspectorObjects.AfterMnemirQuestDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(mnemir);
//        }
//    }
//}
//#endif
