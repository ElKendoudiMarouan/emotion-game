
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

public class Utils
{
    public static T GetComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
        {
            Debug.LogError($"Failed to get component of type {typeof(T)} on GameObject {gameObject.name} and component {GetCallingMethodName()}");
        }
        return component;
    }
    private static string GetCallingMethodName()
    {
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        System.Diagnostics.StackFrame frame = stackTrace.GetFrame(2); // Adjust the frame index as needed
        return frame.GetMethod().DeclaringType.Name;
    }
    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }
}
