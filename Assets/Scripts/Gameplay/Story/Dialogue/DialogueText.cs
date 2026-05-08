using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiaogueText", menuName = "Dialogue/New Dialogue Container")]
public class DialogueText : ScriptableObject
{
    [SerializeField] private float defaultTypeSpeed = 10f; 

    public float DefaultTypeSpeed { get { return defaultTypeSpeed; } }

    [Serializable]
    public class DialogueLine
    {
        public string speakerName;

        //public float typeSpeed = -1; //флаг для дефолтного значения

        [TextArea(5, 10)]
        public string[] paragraphs;
    }

    public List<DialogueLine> dialogue;
}
