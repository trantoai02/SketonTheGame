using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public string name;
    [TextArea(5, 10)]
    public string[] lines;
}

[Serializable]
public class DialogueList
{
    public Dialogue[] dialogues;
}
