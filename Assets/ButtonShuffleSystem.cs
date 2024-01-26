using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EmotionSystem;
using static TMPro.Examples.ObjectSpin;

[RequireComponent(typeof(ConversationManager))]

public class ButtonShuffleSystem : MonoBehaviour
{
    private ConversationManager conversationManager;
    public Button button1;
    public Button button2;
    public Button button3;

    private void Start()
    {
        conversationManager = Utils.GetComponent<ConversationManager>(gameObject);

        // Assign the initial random emotions to buttons
        AssignRandomEmotions();
    }

    public void AssignRandomEmotions()
    {
        // Shuffle the array to get random emotions
        ShuffleEmotionList(conversationManager.shuffledEmotionTypesList);
        conversationManager.UpdateDesiredEmotion(conversationManager.SelectRandomEmotion());

        SetButtonProperties(button1, conversationManager.GetEmotionData(conversationManager.shuffledEmotionTypesList[0]));
        SetButtonProperties(button2, conversationManager.GetEmotionData(conversationManager.shuffledEmotionTypesList[1]));
        SetButtonProperties(button3, conversationManager.GetEmotionData(conversationManager.shuffledEmotionTypesList[2]));
    }

    private void SetButtonProperties(Button button, EmotionData emotionData)
    {
        if (button != null)
        {
            DialogueLineData dialogue = conversationManager.dialogueManager.GetRandomDialogueForEmotion(emotionData.Type);
            SetButtonText(button, dialogue.playerLine);
            SetButtonColor(button, emotionData.HexColor);
            conversationManager.emotionSpriteDisplayer.FindAndUpdateButtonEmotionIcon(button.transform, emotionData.Type);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => HandleButtonClick(emotionData, dialogue));
        }
    }
    public static void SetButtonText(Button button, string text)
    {
        Transform textTransform = button.transform.Find("Text"); //to change name
        if (textTransform != null)
        {
            TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();

            textComponent.text = text;
            if (textComponent != null)
            {
                textComponent.text = text;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on the button or its children.");
            }
        }
        else
        {
            Debug.LogError("Text child not found on the button .");
        }
    }
    public static void SetButtonColor(Button button, string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = color;
            button.colors = buttonColors;
        }
        else
        {
            Debug.LogError("Invalid color format: " + hexColor);
        }
    }
    private void HandleButtonClick(EmotionData responseEmotionData, DialogueLineData dialogue)
    {
        conversationManager.HandlePlayerResponse(responseEmotionData, dialogue);
    }

    // Helper method to shuffle an array
    private void ShuffleEmotionList<T>(T[] array)
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
