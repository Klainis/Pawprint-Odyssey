using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiaogueText", menuName = "Dialogue/New Dialogue Container")]
public class DialogueText : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        public string speakerName;

        [TextArea(5, 10)]
        public string[] paragraphs;
    }

    public List<DialogueLine> dialogue;
}
