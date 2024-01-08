
public enum EmotionType
{
    Happy,
    Sad,
    Admiring,
    Disgusted,
    Angry,
    Fearful,
}

public class EmotionData
{
    public EmotionType Emotion { get; set; }
    public EmotionType SameGroup { get; set; }
    public EmotionType Opposite { get; set; }
    public int Intensity { get; set; }
    public string HexColor { get; set; }

    public EmotionData(EmotionType emotion, EmotionType sameGroup, EmotionType opposite, string hexColor, int intensity = 0)
    {
        Emotion = emotion;
        SameGroup = sameGroup;
        Opposite = opposite;
        Intensity = intensity;
        HexColor = hexColor;
    }
}