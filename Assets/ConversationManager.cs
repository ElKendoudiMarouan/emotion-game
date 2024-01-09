using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EmotionSystem))]
[RequireComponent(typeof(ButtonShuffleSystem))]
[RequireComponent(typeof(EmotionSpriteDisplayer))]
[RequireComponent(typeof(StatsCounterDisplayer))]
public class ConversationManager : MonoBehaviour
{
    public EmotionSystem emotionSystem;
    public ButtonShuffleSystem buttonShuffleSystem;
    public StatsCounterDisplayer statsCounterDisplayer;
    public EmotionSpriteDisplayer emotionSpriteDisplayer;

    public List<EmotionData> emotionDataList = new List<EmotionData>();
    public EmotionType[] shuffledEmotionTypesList;
    public EmotionType desiredEmotion;

    public int turnCounterMeter = 1;
    public int playerCloseness = 20;

    public void Awake()
    {
        shuffledEmotionTypesList = new EmotionType[] { EmotionType.Happiness, EmotionType.Sadness, EmotionType.Admiration, EmotionType.Disgust, EmotionType.Anger, EmotionType.Fear };
        emotionDataList.Add(new EmotionData(EmotionType.Happiness, EmotionType.Admiration, EmotionType.Sadness, "#FDF022"));
        emotionDataList.Add(new EmotionData(EmotionType.Sadness, EmotionType.Fear, EmotionType.Happiness, "#6993F5"));
        emotionDataList.Add(new EmotionData(EmotionType.Admiration, EmotionType.Happiness, EmotionType.Disgust, "#FF88DA"));
        emotionDataList.Add(new EmotionData(EmotionType.Disgust, EmotionType.Anger, EmotionType.Admiration, "#74D41F"));
        emotionDataList.Add(new EmotionData(EmotionType.Anger, EmotionType.Disgust, EmotionType.Fear, "#FF2800"));
        emotionDataList.Add(new EmotionData(EmotionType.Fear, EmotionType.Sadness, EmotionType.Anger, "#C400FF"));
    }
    public void Start()
    {
        emotionSystem = Utils.GetComponent<EmotionSystem>(gameObject);
        buttonShuffleSystem = Utils.GetComponent<ButtonShuffleSystem>(gameObject);
        statsCounterDisplayer = Utils.GetComponent<StatsCounterDisplayer>(gameObject);
        emotionSpriteDisplayer = Utils.GetComponent<EmotionSpriteDisplayer>(gameObject);

    }
    public void AdvanceTurn()
    {
        turnCounterMeter++;
        statsCounterDisplayer.UpdateTurnCounter(turnCounterMeter);
        emotionSpriteDisplayer.UpdateDesiredEmotionIcon(desiredEmotion);
    }
    public void HandlePlayerResponse(EmotionData responseEmotionData)
    {
        emotionSystem.HandlePlayerResponse(responseEmotionData);
        buttonShuffleSystem.AssignRandomEmotions();
        AdvanceTurn();
        EventSystem.current.SetSelectedGameObject(null);
    }
    public EmotionData GetEmotionData(EmotionType emotion)
    {
        return emotionDataList.Find(x => x.Type == emotion)!;
    }
}
