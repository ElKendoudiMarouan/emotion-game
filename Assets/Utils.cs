using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
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
    public static T GetRandomEnumValue<T>()
    {
        // Get a random value from an enum
        var values = System.Enum.GetValues(typeof(T));
        int randomIndex = Random.Range(0, values.Length);
        return (T)values.GetValue(randomIndex);
    }

    public static List<int> CreateIncrementingList(int startValue, int count)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < count; i++)
        {
            result.Add(startValue + i);
        }
        return result;
    }
}