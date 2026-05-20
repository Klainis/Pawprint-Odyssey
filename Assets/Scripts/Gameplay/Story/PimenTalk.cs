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
    public DialogueText BeforeFirstFightRoomDialogueText;
    public DialogueText AfterFirstFightRoomDialogueText;
    public DialogueText TakeSecondArtifactDialogueText;
    public DialogueText BeforeFinalBossDialogueText;
    public DialogueText WinLastBossDialogueText;
    public DialogueText LastRoomDialogueText;

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
        IsPimenTalk = true;

        if (_pimen != null)
        {
            _pimen.GetComponent<PimenMove>().enabled = false;
            _pimen.GetComponent<PimenAnimation>().SetIsMove(false);
        }

        TurnPlayerToNPC();

        _currentDialogue = diaogueText;
        dialogueController.DisplayNextParagraph(diaogueText);
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
        PlayerView.Instance.FreezePlayer(true);
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
        PlayerView.Instance.FreezePlayer(false);

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

//[System.Serializable]
//public class CustomPimenTalkInspectorObjects
//{
//    [HideInInspector] public DialogueText PimenMeetDialogueText;
//    [HideInInspector] public DialogueText BeforeFirstBossDialogueText;
//    [HideInInspector] public DialogueText WinFirstBossDialogueText;
//    [HideInInspector] public DialogueText SawClawDialogueText;
//    [HideInInspector] public DialogueText BeforeFirstFightRoomDialogueText;
//    [HideInInspector] public DialogueText AfterFirstFightRoomDialogueText;
//    [HideInInspector] public DialogueText TakeSecondArtifactDialogueText;
//    [HideInInspector] public DialogueText BeforeFinalBossDialogueText;
//    [HideInInspector] public DialogueText WinLastBossDialogueText;
//    [HideInInspector] public DialogueText LastRoomDialogueText;
//}

//#if UNITY_EDITOR
//[CustomEditor(typeof(PimenTalk))]
//public class MyPimenScriptEditor : Editor
//{
//    PimenTalk pimen;

//    private void OnEnable()
//    {
//        pimen = (PimenTalk)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        pimen.customWraithInspectorObjects.PimenMeetDialogueText = EditorGUILayout.ObjectField("Meet with Pimen", pimen.customWraithInspectorObjects.PimenMeetDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.BeforeFirstBossDialogueText = EditorGUILayout.ObjectField("Before First Boss", pimen.customWraithInspectorObjects.BeforeFirstBossDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.WinFirstBossDialogueText = EditorGUILayout.ObjectField("After First Boss", pimen.customWraithInspectorObjects.WinFirstBossDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.SawClawDialogueText = EditorGUILayout.ObjectField("Saw Claw", pimen.customWraithInspectorObjects.SawClawDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.BeforeFirstFightRoomDialogueText = EditorGUILayout.ObjectField("Before First Fight Room", pimen.customWraithInspectorObjects.BeforeFirstFightRoomDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.AfterFirstFightRoomDialogueText = EditorGUILayout.ObjectField("After First Fight Room", pimen.customWraithInspectorObjects.AfterFirstFightRoomDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.TakeSecondArtifactDialogueText = EditorGUILayout.ObjectField("Take Second Artifact", pimen.customWraithInspectorObjects.TakeSecondArtifactDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.BeforeFinalBossDialogueText = EditorGUILayout.ObjectField("Before Final Boss", pimen.customWraithInspectorObjects.BeforeFinalBossDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.WinLastBossDialogueText = EditorGUILayout.ObjectField("After Final Boss", pimen.customWraithInspectorObjects.WinLastBossDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        pimen.customWraithInspectorObjects.LastRoomDialogueText = EditorGUILayout.ObjectField("Last Room", pimen.customWraithInspectorObjects.LastRoomDialogueText,
//            typeof(DialogueText), true) as DialogueText;

//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(pimen);
//        }
//    }
//}
//#endif
