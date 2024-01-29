using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static T GetComponentInObject<T>(GameObject gameObject) where T : Component
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

    public static T RecursiveGetComponentByObjectName<T>(string componentName, Transform parentTransform) where T : Component
    {
        Transform transform = Utils.RecursiveFindChild(parentTransform, componentName);

        if (transform != null)
        {
            return GetComponentInObject<T>(transform.gameObject);
        }
        else
        {
            Debug.LogError($"Target object with name '{componentName}' not found!");
            return null;
        }
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

    /**
     * Buttons utils
     */
    public static Color HexToColor(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Invalid color format: " + hexColor);
            return Color.white;
        }
    }

    public static TextMeshProUGUI ExtractMeshTextFromButton(Button button)
    {
        Transform textTransform = button.transform.Find("Text"); //to change name
        if (textTransform != null)
        {
            TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();

            if (textComponent != null)
            {
                return textComponent;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the button or its children.");
                return null;
            }
        }
        else
        {
            Debug.LogError("Text child not found on the button .");
            return null;
        }
    }

    /**
     * Display Utils
     */
    public static void UpdateSpriteAndAdaptSize(Sprite sprite, float iconSize, SpriteRenderer spriteRenderer)
    {
        float originalWidth = sprite.bounds.size.x;
        float originalHeight = sprite.bounds.size.y;

        float scaleFactorX = originalWidth != 0 ? iconSize / originalWidth : .1f;
        float scaleFactorY = originalHeight != 0 ? iconSize / originalHeight : .1f;

        spriteRenderer.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1f);
        spriteRenderer.sprite = sprite;
    }

    /**
     * List and enum utils
     */
    public static T GetRandomEnumValue<T>()
    {
        // Get a random value from an enum
        var values = System.Enum.GetValues(typeof(T));
        int randomIndex = UnityEngine.Random.Range(0, values.Length);
        return (T)values.GetValue(randomIndex);
    }

    public static int GetPositionByEnumValue<T>(T enumValue) where T : Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));

        for (int i = 0; i < values.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(values[i], enumValue))
            {
                return i;
            }
        }
        Debug.LogError($"Enum value {enumValue} not found in enum {typeof(T)}");
        return 0;
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

    public static void ShuffleList<T>(T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + UnityEngine.Random.Range(0, n - i);
            T temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }
    }
}