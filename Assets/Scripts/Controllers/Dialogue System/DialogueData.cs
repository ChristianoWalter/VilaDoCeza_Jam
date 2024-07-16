using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Dialogue
{
    public string characterName;
    [TextArea (5, 10)]
    public string dialogueTxt;
    public Sprite characterImage;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObject/TalkScript", order = 1)]
public class DialogueData : ScriptableObject
{
    public List<Dialogue> talkScript;
}
