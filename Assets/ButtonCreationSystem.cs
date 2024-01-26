using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConversationManager))]

public class ButtonCreationSystem : MonoBehaviour
{
    private ConversationManager conversationManager;

    public GameObject dialogButtonPrefab;
    public Transform dialogButtonContainer;
    private List<GameObject> dialogButtonObjects = new List<GameObject>();
    private float dialogButtonSpacing = -135f;
    private int numberOfDialogButtons = 3;

    private void Start()
    {
        conversationManager = Utils.GetComponentInObject<ConversationManager>(gameObject);
        CreateDialogButtons();
        // Assign the initial random emotions to buttons
        AssignRandomEmotions();
    }

    void CreateDialogButtons()
    {
        // Instantiate the button prefab
        Vector3 position = dialogButtonContainer.transform.position;
        for (int i = 0; i < numberOfDialogButtons; i++)
        {
            GameObject dialogButtonObject = Instantiate(dialogButtonPrefab, dialogButtonContainer);
            dialogButtonObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,  i * dialogButtonSpacing);
            dialogButtonObject.gameObject.name = $"DialogButton{i+1}";
            dialogButtonObjects.Add(dialogButtonObject);
        }
    }

    public void AssignRandomEmotions()
    {
        conversationManager.UpdateDesiredEmotion(conversationManager.SelectRandomEmotion());

        for (int i = 0; i < dialogButtonObjects.Count; i++)
        {
            Button buttonComponent = dialogButtonObjects[i].GetComponent<Button>();
            SetButtonProperties(buttonComponent, conversationManager.GetEmotionData(conversationManager.shuffledEmotionTypesList[i]));
        }
    }

    private void SetButtonProperties(Button button, EmotionData emotionData)
    {
        if (button != null)
        {
            DialogueLineData dialogue = conversationManager.dialogueManager.GetRandomDialogueForEmotion(emotionData.Type);
            SetButtonText(button, dialogue.playerLine);
            SetButtonColor(button, emotionData.HexColor);
            conversationManager.spriteDisplaySystem.FindAndUpdateButtonEmotionIcon(button.transform, emotionData.Type);

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
}
