
using System.Collections.Generic;
using UnityEngine;

public enum EmotionType
{
    Happiness,
    Sadness,
    Admiration,
    Disgust,
    Anger,
    Fear,
}

public class EmotionData
{
    public EmotionType Type { get; set; }
    public EmotionType SameGroupType { get; set; }
    public EmotionType OppositeType { get; set; }
    public int Intensity { get; set; }
    public string HexColor { get; set; }

    public EmotionData(EmotionType emotion, EmotionType sameGroup, EmotionType opposite, string hexColor, int intensity = 0)
    {
        Type = emotion;
        SameGroupType = sameGroup;
        OppositeType = opposite;
        Intensity = intensity;
        HexColor = hexColor;
    }
}

[System.Serializable]
public class CharacterDialogueData
{
    public string characterName;
    public List<DialogueLinesByEmotionData> dialogueLinesByEmotion;
}

[System.Serializable]
public class DialogueLinesByEmotionData
{
    public EmotionType emotion;
    public List<DialogueLineData> dialogueLines;
}

[System.Serializable]
public class DialogueLineData
{
    public string playerLine;
    public string sameResponse;
    public string oppositeResponse;
    public string sameGroupResponse;
    public string neutralResponse;
}

