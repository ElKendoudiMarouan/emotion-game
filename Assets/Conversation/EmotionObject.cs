
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
    Admiration,
    Fear,
    Sadness,
    Anger,
    Disgust,
}

public enum EmotionGroup
{
    Yellow,
    Red,
    Blue
}

public enum ResponseType
{
    Identical,
    Close,
    Neutral,
    Opposite
}

public class EmotionData
{
    public EmotionType EmotionType { get; set; }
    public EmotionType SameGroupType { get; set; }
    public EmotionType OppositeType { get; set; }
    public string HexColor { get; set; }
    public EmotionGroup Group { get; set; }
    public int Intensity { get; set; }

    public EmotionData(EmotionType emotion, EmotionType sameGroup, EmotionType opposite, string hexColor, EmotionGroup group, int intensity = 0)
    {
        EmotionType = emotion;
        SameGroupType = sameGroup;
        OppositeType = opposite;
        HexColor = hexColor;
        Group = group;
        Intensity = intensity;
    }
}

public class EmotionDialogueChoice
{
    public EmotionData EmotionData { get; set; }
    public ResponseType ResponseType { get; set; }

    public GameObject ButtonObject { get; set; }

    public DialogueLineData DialogueLineData { get; set; }

    public EmotionDialogueChoice(EmotionData emotionData, ResponseType responseType)
    {
        EmotionData = emotionData;
        ResponseType = responseType;
    }
}

public class Card
{
    public string Name { get; set; }
    public CardType Type { get; set; }
    // Add more properties as needed
    public string Description { get; set; }
    public Sprite Icon { get; set; }
    public int Duration { get; set; }
    public int DurationLeft { get; set; }

    public Card(string name, CardType type, string description, int duration)
    {
        Name = name;
        Type = type;
        Description = description;
        Duration = duration;
        DurationLeft = 0;
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

public enum ColorField
{
    Normal,
    Highlighted,
    Pressed,
    Selected,
    Disabled
}