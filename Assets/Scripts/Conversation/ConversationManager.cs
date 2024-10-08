using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EmotionSystem))]
[RequireComponent(typeof(SpriteDisplaySystem))]
[RequireComponent(typeof(ButtonCreationSystem))]
[RequireComponent(typeof(TextDisplaySystem))]
[RequireComponent(typeof(DialogueManager))]
[RequireComponent(typeof(PatienceManager))]
[RequireComponent(typeof(DeckManager))]
[RequireComponent(typeof(CardEffectManager))]
public class ConversationManager : MonoBehaviour
{
    public EmotionSystem emotionSystem;
    public SpriteDisplaySystem spriteDisplaySystem;
    public ButtonCreationSystem buttonCreationSystem;
    public TextDisplaySystem textDisplaySystem;
    public DialogueManager dialogueManager;
    public PatienceManager patienceManager;
    public DeckManager deckManager;
    public CardEffectManager cardEffectManager;

    [Header("Win condition")]
    public ConversationWinConditon winCondition;
    public int requiredCloseness = 30;
    public int requiredEmotionCombo = 5;
    public int requiredEmotionIntensity = 20;
    public EmotionType? requiredEmotion;
    public EmotionData requiredEmotionData;
    public int requiredEmotionGroupIntensity = 30;
    public EmotionGroup? requiredEmotionGroup;
    public List<EmotionData> requiredEmotionDataForGroup;
    public bool playerWin = false;

    [Header("Emotion")]
    public List<EmotionData> emotionDataList = new List<EmotionData>();
    public EmotionType[] shuffledEmotionTypesList;
    public EmotionType desiredEmotion;
    public List<EmotionDialogueChoice> emotionDialogueChoices = new List<EmotionDialogueChoice>();
    public int numberOfDialogueChoices = 3;
    public Dictionary<ResponseType, string> responseColorsByType = new Dictionary<ResponseType, string>();

    [Header("Meters")]
    public int turnCounterMeter = 1;
    public int currentCloseness = 20;

    [Header("Combo")]
    public int emotionCombo = 0;
    public EmotionData lastSelectedEmotion;
    public EmotionData lastComboEmotion;

    [Header("Saturation")]
    public EmotionType lastSaturatedEmotion;
    public bool isSaturated = false;



    public void Awake()
    {
        shuffledEmotionTypesList = new EmotionType[] { EmotionType.Happiness, EmotionType.Sadness, EmotionType.Admiration, EmotionType.Disgust, EmotionType.Anger, EmotionType.Fear };
        
        emotionDataList.Add(new EmotionData(EmotionType.Happiness, EmotionType.Admiration, EmotionType.Sadness, "#FDF022", EmotionGroup.Yellow));
        emotionDataList.Add(new EmotionData(EmotionType.Admiration, EmotionType.Happiness, EmotionType.Disgust, "#FF88DA", EmotionGroup.Yellow));
        emotionDataList.Add(new EmotionData(EmotionType.Sadness, EmotionType.Fear, EmotionType.Happiness, "#6993F5", EmotionGroup.Blue));
        emotionDataList.Add(new EmotionData(EmotionType.Fear, EmotionType.Sadness, EmotionType.Anger, "#C400FF", EmotionGroup.Blue));
        emotionDataList.Add(new EmotionData(EmotionType.Anger, EmotionType.Disgust, EmotionType.Fear, "#FF2800", EmotionGroup.Red));
        emotionDataList.Add(new EmotionData(EmotionType.Disgust, EmotionType.Anger, EmotionType.Admiration, "#74D41F", EmotionGroup.Red));

        responseColorsByType.Add(ResponseType.Identical, "#FF9800");
        responseColorsByType.Add(ResponseType.Close, "#5F8670");
        responseColorsByType.Add(ResponseType.Opposite, "#B80000");
        responseColorsByType.Add(ResponseType.Neutral, "#9EB8D9");
    }
    public void Start()
    {
        emotionSystem = Utils.GetComponentInObject<EmotionSystem>(gameObject);
        dialogueManager = Utils.GetComponentInObject<DialogueManager>(gameObject);
        buttonCreationSystem = Utils.GetComponentInObject<ButtonCreationSystem>(gameObject);
        textDisplaySystem = Utils.GetComponentInObject<TextDisplaySystem>(gameObject);
        spriteDisplaySystem = Utils.GetComponentInObject<SpriteDisplaySystem>(gameObject);
        patienceManager = Utils.GetComponentInObject<PatienceManager>(gameObject);
        deckManager = Utils.GetComponentInObject<DeckManager>(gameObject);
        cardEffectManager = Utils.GetComponentInObject<CardEffectManager>(gameObject);

        winCondition = Utils.GetRandomEnumValue<ConversationWinConditon>();//todo change this
        CheckWinCondition();
    }

    public void AssignRandomEmotions()
    {
        Utils.ShuffleList(shuffledEmotionTypesList);
        UpdateDesiredEmotion(SelectRandomEmotion());

        buttonCreationSystem.DestroyDialogueButtons();
        emotionDialogueChoices.Clear();

        for (int i = 0; i < numberOfDialogueChoices; i++)
        {
            var emotionData = GetEmotionData(shuffledEmotionTypesList[i]);
            var responseType = GetResponseType(emotionData);
            var emotionDialogueChoice = new EmotionDialogueChoice(emotionData, responseType);
            emotionDialogueChoices.Add(emotionDialogueChoice);
            buttonCreationSystem.CreateDialogueButtonForEmotion(emotionDialogueChoice);
        }
    }

    public void CheckWinCondition()
    {
        if (patienceManager.currentPatience == 0)
        {
            textDisplaySystem.UpdateObjectiveCounter("YOU LOST!!");
            return;
        }
        var objectiveTextDetails = "";
        switch (winCondition)
        {
            case ConversationWinConditon.Emotion:
                if (requiredEmotion == null)
                {
                    requiredEmotion = SelectRandomEmotion();
                    requiredEmotionData = GetEmotionData((EmotionType) requiredEmotion);
                }
                objectiveTextDetails =  $"Emotion {requiredEmotion} : {requiredEmotionData.Intensity} / {requiredEmotionIntensity}";
                if (requiredEmotionData.Intensity >= requiredEmotionIntensity)
                {
                    playerWin = true;
                    Debug.Log($"Win condition Achieved: Emotion more or equal to {requiredEmotionIntensity}");
                }
                break;

            case ConversationWinConditon.Combo:
                objectiveTextDetails = $"Combo : {emotionCombo} / {requiredEmotionCombo}";
                if (emotionCombo >= requiredEmotionCombo)
                {
                    playerWin = true;
                    Debug.Log($"Win condition Achieved: Emotion Combo more or equal to {requiredEmotionCombo}");
                }
                break;

            case ConversationWinConditon.Group:
                if (requiredEmotionGroup == null)
                {
                    requiredEmotionGroup = Utils.GetRandomEnumValue<EmotionGroup>();
                    requiredEmotionDataForGroup = GetEmotionDataByGroup((EmotionGroup)requiredEmotionGroup);
                }
                var groupIntensity = requiredEmotionDataForGroup.Sum(e => e.Intensity);
                objectiveTextDetails = $" Group {requiredEmotionGroup} : {groupIntensity} / {requiredEmotionGroupIntensity}";
                if (groupIntensity >= requiredEmotionGroupIntensity)
                {
                    playerWin = true;
                    Debug.Log($"Win condition Achieved: Emotion Group more or equal to {requiredEmotionGroupIntensity}");
                }
                break;

            case ConversationWinConditon.Closeness:
                objectiveTextDetails = $"Closness : {currentCloseness} / {requiredCloseness}";
                if (currentCloseness >= requiredCloseness)
                {
                    playerWin = true;
                    Debug.Log($"Win condition Achieved: Closeness more or equal to {requiredCloseness}");
                }
                break;
        }
        textDisplaySystem.UpdateObjectiveCounter($"Objective : {objectiveTextDetails}{(playerWin ? " : Success" : "")}");
    }
    public void HandlePlayerResponse(EmotionDialogueChoice emotionDialogue)
    {
        emotionSystem.HandlePlayerResponse(emotionDialogue);
        spriteDisplaySystem.UpdateNPCPortrait(emotionDialogue.EmotionData.EmotionType);

        CheckWinCondition();

        Utils.ShuffleList(shuffledEmotionTypesList);
        AssignRandomEmotions();

        AdvanceTurn();

        buttonCreationSystem.ChangeCardButtonsInteractibility(true);
        cardEffectManager.CheckCardEffectsThisTurn(true);
        EventSystem.current.SetSelectedGameObject(null); //for removing select from button
    }
    public void AdvanceTurn()
    {
        turnCounterMeter++;
        textDisplaySystem.UpdateTurnCounter(turnCounterMeter);
    }
    public void UpdateDesiredEmotion(EmotionType desiredEmotion)
    {
        this.desiredEmotion = desiredEmotion;
        spriteDisplaySystem.UpdateDesiredEmotionIcon(desiredEmotion);
    }
    public EmotionData GetEmotionData(EmotionType emotion) //todo : to extension class
    {
        return emotionDataList.Find(x => x.EmotionType == emotion)!;
    }
    public List<EmotionData> GetEmotionDataByGroup(EmotionGroup group)
    {
        return emotionDataList.FindAll(x => x.Group == group);
    }
    public ResponseType GetResponseType(EmotionData emotionData)
    {
        var desiredEmotionData = GetEmotionData(desiredEmotion);
        if (emotionData.EmotionType == desiredEmotion)
        {
            return ResponseType.Identical;
        }
        else if (emotionData.EmotionType == desiredEmotionData.SameGroupType)
        {
            return ResponseType.Close;
        }
        else if (emotionData.EmotionType == desiredEmotionData.OppositeType)
        {
            return ResponseType.Opposite;
        }
        else
        {
            return ResponseType.Neutral;
        }
    }
    public EmotionType SelectRandomEmotion()
    {
        int randomNumber = UnityEngine.Random.Range(0, 6);
        return shuffledEmotionTypesList[randomNumber];
    }
}
