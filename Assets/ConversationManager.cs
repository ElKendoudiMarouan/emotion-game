using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EmotionSystem))]
[RequireComponent(typeof(ButtonShuffleSystem))]
[RequireComponent(typeof(EmotionCounterDisplayer))]

public class ConversationManager : MonoBehaviour
{
    private EmotionSystem emotionSystem;
    private ButtonShuffleSystem buttonShuffleSystem;
    private EmotionCounterDisplayer emotionCounterDisplayer;

    public List<EmotionData> emotionDataList = new List<EmotionData>();
    public EmotionType[] shuffledEmotionTypesList;
    public EmotionType desiredEmotion;

    public int turnCounterMeter = 0;
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
        emotionSystem = GetComponent<EmotionSystem>();
        buttonShuffleSystem = GetComponent<ButtonShuffleSystem>();
        emotionCounterDisplayer = GetComponent<EmotionCounterDisplayer>();
    }


    public void HandlePlayerResponse(EmotionData responseEmotionData)
    {
        turnCounterMeter++;
        emotionSystem.HandlePlayerResponse(responseEmotionData);
        buttonShuffleSystem.AssignRandomEmotions();
        EventSystem.current.SetSelectedGameObject(null);
    }
    public EmotionData GetEmotionData(EmotionType emotion)
    {
        return emotionDataList.Find(x => x.Type == emotion)!;
    }
}
