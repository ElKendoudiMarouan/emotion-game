using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ConversationManager))]
public class ButtonCreationSystem : MonoBehaviour
{
    private ConversationManager conversationManager;

    private Transform dialogButtonContainer;
    public GameObject dialogButtonPrefab;
    private Dictionary<EmotionType, GameObject> dialogButtonObjects = new Dictionary<EmotionType, GameObject>();
    private float dialogButtonSpacing = -135f;

    private Transform cardButtonContainer;
    public GameObject cardButtonPrefab;
    public Dictionary<Card, GameObject> cardButtonObjects = new Dictionary<Card, GameObject>();
    public float cardButtonSpacing = 200f;


    private void Start()
    {
        if(dialogButtonContainer == null)
        {
            dialogButtonContainer = Utils.RecursiveFindChild(gameObject.transform, "DialogButtonContainer");
        }
        if (cardButtonContainer == null)
        {
            cardButtonContainer = Utils.RecursiveFindChild(gameObject.transform, "CardContainer");
        }
        conversationManager = Utils.GetComponentInObject<ConversationManager>(gameObject);

        conversationManager.AssignRandomEmotions();
    }

    public void createDialogueButtonForEmotion(EmotionDialogueChoice emotionDialogue)
    {
        var buttonObject = CreateDialogButton(emotionDialogue);
        emotionDialogue.ButtonObject = buttonObject;
        Button buttonComponent = buttonObject.GetComponent<Button>();
        SetDialogButtonProperties(buttonComponent, emotionDialogue);
    }

    public GameObject CreateDialogButton(EmotionDialogueChoice emotionDialogue)
    {
        GameObject dialogButtonObject = Instantiate(dialogButtonPrefab, dialogButtonContainer);
        var count = dialogButtonObjects.Count;
        dialogButtonObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, count * dialogButtonSpacing);
        dialogButtonObjects.Add(emotionDialogue.EmotionData.EmotionType, dialogButtonObject);
        dialogButtonObject.gameObject.name = $"DialogButton{count + 1}";
        return dialogButtonObject;
    }

    public void destroyDialogueButtons()
    {
        if (dialogButtonObjects.Count > 0)
        {
            foreach (GameObject buttonObject in dialogButtonObjects.Values)
            {
                Destroy(buttonObject);
            }
            dialogButtonObjects.Clear();
        }
    }

    private void SetDialogButtonProperties(Button button, EmotionDialogueChoice emotionDialogue)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            DialogueLineData dialogue = conversationManager.dialogueManager.GetRandomDialogueForEmotion(emotionDialogue.EmotionData.EmotionType);
            emotionDialogue.DialogueLineData = dialogue;

            SetButtonText(button, dialogue.playerLine, Color.black);

            SetButtonColor(button, /*emotionDialogue.EmotionData.HexColor*/ Utils.hexToColor("#AC87C5"), ColorField.Normal);
            SetButtonColor(button, Utils.hexToColor(conversationManager.responseColorsByType[emotionDialogue.ResponseType]), ColorField.Highlighted);

            var iconRenderer = conversationManager.spriteDisplaySystem.FindAndUpdateButtonEmotionIcon(button.transform, emotionDialogue.EmotionData.EmotionType);

            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            AddEventTriggerListener(trigger, EventTriggerType.PointerEnter, () => OnDialogPointerEnter(iconRenderer));
            AddEventTriggerListener(trigger, EventTriggerType.PointerExit, () => OnDialogPointerExit(iconRenderer));
            AddEventTriggerListener(trigger, EventTriggerType.PointerClick, () => HandleButtonClick(emotionDialogue));
           // button.onClick.AddListener(() => HandleButtonClick(emotionDialogue));
        }
    }

    public void UpdateDialogButtonHoverColor(EmotionDialogueChoice emotionDialogue)
    {
        var button = dialogButtonObjects[emotionDialogue.EmotionData.EmotionType].GetComponent<Button>();
        SetButtonColor(button, Utils.hexToColor(conversationManager.responseColorsByType[emotionDialogue.ResponseType]), ColorField.Highlighted);
    }

    public void CreateCardButton(Card card, int numberOfCardsInHands)
    {
        // Instantiate the button prefab
        Vector3 position = cardButtonContainer.transform.position;
        GameObject cardObject = Instantiate(cardButtonPrefab, cardButtonContainer);
        cardObject.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(position.x + (numberOfCardsInHands - 1) * cardButtonSpacing, position.y);

        cardObject.gameObject.name = $"Card{card.Type}";
        cardButtonObjects.Add(card, cardObject);

        Button buttonComponent = cardObject.GetComponent<Button>();
        buttonComponent.onClick.RemoveAllListeners();

        var textComponent = SetButtonText(buttonComponent, card.Name, Color.white);
        SetButtonColor(buttonComponent, Utils.hexToColor("#0C2D57"), ColorField.Normal);
        SetButtonColor(buttonComponent, Utils.hexToColor("#FC6736"), ColorField.Highlighted);
        SetButtonColor(buttonComponent, Utils.hexToColor("#FFB0B0"), ColorField.Pressed);
        SetButtonColor(buttonComponent, Utils.hexToColor("#EFECEC"), ColorField.Disabled);

        EventTrigger trigger = buttonComponent.gameObject.AddComponent<EventTrigger>();

        AddEventTriggerListener(trigger, EventTriggerType.PointerEnter, () => OnCardPointerEnter(textComponent));
        AddEventTriggerListener(trigger, EventTriggerType.PointerExit, () => OnCardPointerExit(textComponent));
        AddEventTriggerListener(trigger, EventTriggerType.PointerClick, () => conversationManager.deckManager.PlayCard(card));
        //buttonComponent.onClick.AddListener(() => conversationManager.deckManager.PlayCard(card));
    }

    public void DestroyCard(Card card)
    {
        Destroy(cardButtonObjects[card]);
        cardButtonObjects.Remove(card);
    }

    public void RepositionCardButtons()
    {
        var index = 0;
        foreach (GameObject buttonObject in cardButtonObjects.Values)
        {
            Vector3 position = cardButtonContainer.transform.position;
            RectTransform cardTransform = buttonObject.GetComponent<RectTransform>();
            cardTransform.anchoredPosition = new Vector2(position.x + cardButtonSpacing * index, position.y);
            index++;
        }
    }

    public void ChangeCardButtonsInteractibility(bool enabled)
    {
        foreach (GameObject buttonObject in cardButtonObjects.Values)
        {
            buttonObject.GetComponent<Button>().interactable = enabled;
        }
    }

    private void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(_ => callback.Invoke());
        trigger.triggers.Add(entry);
    }

    public void OnDialogPointerEnter(SpriteRenderer renderer)
    {
        if (renderer != null)
        {
            renderer.enabled = true;
        }
    }

    public void OnDialogPointerExit(SpriteRenderer renderer)
    {
        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }

    public void OnCardPointerEnter(TextMeshProUGUI textComponent)
    {
        textComponent.color = Color.black;
    }

    public void OnCardPointerExit(TextMeshProUGUI textComponent)
    {
        textComponent.color = Color.white;
    }

    public static TextMeshProUGUI SetButtonText(Button button, string text, Color color)
    {
        TextMeshProUGUI textComponent = Utils.extractMeshTextFromButton(button);
        textComponent.text = text;
        textComponent.color = color;//Todo change this
        return textComponent;
    }

    public static void SetButtonColor(Button button, Color color, ColorField colorField)
    {
        ColorBlock buttonColors = button.colors;

        switch (colorField)
        {
            case ColorField.Normal:
                buttonColors.normalColor = color;
                break;
            case ColorField.Highlighted:
                buttonColors.highlightedColor = color;
                break;
            case ColorField.Pressed:
                buttonColors.pressedColor = color;
                break;
            case ColorField.Selected:
                buttonColors.selectedColor = color;
                break;
            case ColorField.Disabled:
                buttonColors.disabledColor = color;
                break;
            default:
                Debug.LogError("Invalid ColorField: " + colorField);
                return;
        }

        button.colors = buttonColors;
    }

    private void HandleButtonClick(EmotionDialogueChoice emotionDialogue)
    {
        conversationManager.HandlePlayerResponse(emotionDialogue);
    }
}
