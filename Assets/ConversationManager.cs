using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EmotionSystem))]
[RequireComponent(typeof(EmotionSpriteDisplayer))]
[RequireComponent(typeof(ButtonShuffleSystem))]
[RequireComponent(typeof(TextDisplaySystem))]
[RequireComponent(typeof(DialogueManager))]
[RequireComponent(typeof(PatienceManager))]
[RequireComponent(typeof(DeckManager))]
[RequireComponent(typeof(CardEffectManager))]
public class ConversationManager : MonoBehaviour
{
    public EmotionSystem emotionSystem;
    public EmotionSpriteDisplayer emotionSpriteDisplayer;
    public ButtonShuffleSystem buttonShuffleSystem;
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

    [Header("Meters")]
    public int turnCounterMeter = 1;
    public int currentCloseness = 20;

    [Header("Combo")]
    public int emotionCombo = 0;
    public EmotionData lastSelectedEmotion;
    public EmotionData lastComboEmotion;

    public void Awake()
    {
        shuffledEmotionTypesList = new EmotionType[] { EmotionType.Happiness, EmotionType.Sadness, EmotionType.Admiration, EmotionType.Disgust, EmotionType.Anger, EmotionType.Fear };
        
        emotionDataList.Add(new EmotionData(EmotionType.Happiness, EmotionType.Admiration, EmotionType.Sadness, "#FDF022", EmotionGroup.Yellow));
        emotionDataList.Add(new EmotionData(EmotionType.Admiration, EmotionType.Happiness, EmotionType.Disgust, "#FF88DA", EmotionGroup.Yellow));
        emotionDataList.Add(new EmotionData(EmotionType.Sadness, EmotionType.Fear, EmotionType.Happiness, "#6993F5", EmotionGroup.Blue));
        emotionDataList.Add(new EmotionData(EmotionType.Fear, EmotionType.Sadness, EmotionType.Anger, "#C400FF", EmotionGroup.Blue));
        emotionDataList.Add(new EmotionData(EmotionType.Anger, EmotionType.Disgust, EmotionType.Fear, "#FF2800", EmotionGroup.Red));
        emotionDataList.Add(new EmotionData(EmotionType.Disgust, EmotionType.Anger, EmotionType.Admiration, "#74D41F", EmotionGroup.Red));
    }
    public void Start()
    {
        emotionSystem = Utils.GetComponent<EmotionSystem>(gameObject);
        buttonShuffleSystem = Utils.GetComponent<ButtonShuffleSystem>(gameObject);
        textDisplaySystem = Utils.GetComponent<TextDisplaySystem>(gameObject);
        emotionSpriteDisplayer = Utils.GetComponent<EmotionSpriteDisplayer>(gameObject);
        dialogueManager = Utils.GetComponent<DialogueManager>(gameObject);
        patienceManager = Utils.GetComponent<PatienceManager>(gameObject);
        deckManager = Utils.GetComponent<DeckManager>(gameObject);
        cardEffectManager = Utils.GetComponent<CardEffectManager>(gameObject);

        winCondition = Utils.GetRandomEnumValue<ConversationWinConditon>();
        CheckWinCondition();
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
    public void HandlePlayerResponse(EmotionData responseEmotionData, DialogueLineData dialogue)
    {
        emotionSystem.HandlePlayerResponse(responseEmotionData, dialogue);
        CheckWinCondition();
        buttonShuffleSystem.AssignRandomEmotions();
        AdvanceTurn();
        emotionSystem.checkCardEffectsThisTurn();
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void AdvanceTurn()
    {
        turnCounterMeter++;
        textDisplaySystem.UpdateTurnCounter(turnCounterMeter);
    }
    public void UpdateDesiredEmotion(EmotionType desiredEmotion)
    {
        this.desiredEmotion = desiredEmotion;
        emotionSpriteDisplayer.UpdateDesiredEmotionIcon(desiredEmotion);
    }
    public EmotionData GetEmotionData(EmotionType emotion) //todo : to extension class
    {
        return emotionDataList.Find(x => x.Type == emotion)!;
    }
    public List<EmotionData> GetEmotionDataByGroup(EmotionGroup group)
    {
        return emotionDataList.FindAll(x => x.Group == group);
    }
    public EmotionType SelectRandomEmotion()
    {
        int randomNumber = UnityEngine.Random.Range(0, 6);
        return shuffledEmotionTypesList[randomNumber];
    }
}
