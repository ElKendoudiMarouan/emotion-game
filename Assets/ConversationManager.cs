using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EmotionSystem))]
[RequireComponent(typeof(ButtonShuffleSystem))]
public class ConversationManager : MonoBehaviour
{
    private EmotionSystem emotionSystem;
    private ButtonShuffleSystem buttonShuffleSystem;

    public List<EmotionData> emotionDataList = new List<EmotionData>();
    public EmotionType[] shuffledEmotionTypesList;
    public EmotionType desiredEmotion;

    [SerializeField] public int turn = 0;

    public void Awake()
    {
        turn++;
        shuffledEmotionTypesList = new EmotionType[] { EmotionType.Happy, EmotionType.Sad, EmotionType.Admiring, EmotionType.Disgusted, EmotionType.Angry, EmotionType.Fearful };
        emotionDataList.Add(new EmotionData(EmotionType.Happy, EmotionType.Admiring, EmotionType.Sad, "#FDF022"));
        emotionDataList.Add(new EmotionData(EmotionType.Sad, EmotionType.Fearful, EmotionType.Happy, "#6993F5"));
        emotionDataList.Add(new EmotionData(EmotionType.Admiring, EmotionType.Happy, EmotionType.Disgusted, "#FF88DA"));
        emotionDataList.Add(new EmotionData(EmotionType.Disgusted, EmotionType.Angry, EmotionType.Admiring, "#74D41F"));
        emotionDataList.Add(new EmotionData(EmotionType.Angry, EmotionType.Disgusted, EmotionType.Fearful, "#FF2800"));
        emotionDataList.Add(new EmotionData(EmotionType.Fearful, EmotionType.Sad, EmotionType.Angry, "#C400FF"));
    }
    public void Start()
    {
        emotionSystem = GetComponent<EmotionSystem>();
        buttonShuffleSystem = GetComponent<ButtonShuffleSystem>();
    }
    public EmotionData GetEmotionData(EmotionType emotion)
    {
        return emotionDataList.Find(x => x.Emotion == emotion)!;
    }

    public void HandlePlayerResponse(EmotionData responseEmotionData)
    {
        turn++;
        emotionSystem.HandlePlayerResponse(responseEmotionData);
        buttonShuffleSystem.AssignRandomEmotions();
    }
}
