
using System.Collections.Generic;
using UnityEngine;

public enum ConversationWinConditon
{
    Emotion,
    Group,
    Combo,
    Closeness
}
public enum EmotionType
{
    Happiness,
    Sadness,
    Admiration,
    Disgust,
    Anger,
    Fear,
}

public enum EmotionGroup
{
    Yellow,
    Red,
    Blue
}

public class EmotionData
{
    public EmotionType Type { get; set; }
    public EmotionType SameGroupType { get; set; }
    public EmotionType OppositeType { get; set; }
    public string HexColor { get; set; }
    public EmotionGroup Group { get; set; }
    public int Intensity { get; set; }

    public EmotionData(EmotionType emotion, EmotionType sameGroup, EmotionType opposite, string hexColor, EmotionGroup group, int intensity = 0)
    {
        Type = emotion;
        SameGroupType = sameGroup;
        OppositeType = opposite;
        HexColor = hexColor;
        Group = group;
        Intensity = intensity;
    }
}

public class Card
{
    public string Name { get; set; }
    public CardType Type { get; set; }
    // Add more properties as needed

    public Card(string name, CardType type)
    {
        Name = name;
        Type = type;
    }
}

public enum CardType
{
    EmotionSwitch,
    ComboKeeper,
    NegationShield,
    IncreasePatience
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

