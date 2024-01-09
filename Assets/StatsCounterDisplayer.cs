using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ConversationManager))]
public class StatsCounterDisplayer : MonoBehaviour
{
    private ConversationManager conversationManager;

    [SerializeField] private TextMeshProUGUI turnCounter;
    [SerializeField] private TextMeshProUGUI closenessCounter;
    private Dictionary<EmotionType, TextMeshProUGUI> emotionCounters = new Dictionary<EmotionType, TextMeshProUGUI>();

    void Start()
    {
        conversationManager = Utils.GetComponent<ConversationManager>(gameObject);
        Debug.Log($"conversationManager{conversationManager}");

        if (turnCounter ==  null)
        {
            turnCounter = GetCounterByName("TurnCounter");
        }
        UpdateTurnCounter(conversationManager.turnCounterMeter);

        if (closenessCounter == null)
        {
            closenessCounter = GetCounterByName("ClosenessCounter");
        }
        UpdateClosenessCounter(conversationManager.playerCloseness);


        foreach (EmotionData emotionData in conversationManager.emotionDataList)
        {
            AddOrUpdateEmotionCounter(emotionData.Type, emotionData.Intensity);
        }
    }

    public void UpdateTurnCounter(int newVal)
    {
        turnCounter.text = $"Turn : {newVal}";
    }

    public void UpdateClosenessCounter(int newVal)
    {
        closenessCounter.text = $"Closeness: {newVal}";
    }

    public TextMeshProUGUI GetCounterByName(string counterName)
    {
        GameObject counterObject = GameObject.Find(counterName);
        if (counterObject != null)
        {
            TextMeshProUGUI counterText = counterObject.GetComponent<TextMeshProUGUI>();
            return counterText;
        }
        else
        {
            Debug.LogError("Counter Text with name " + counterName + " not found!");
            return null;
        }
    }
 
    // Method to add or update an emotion counter by name
    public void AddOrUpdateEmotionCounter(EmotionType emotionType, int newValue)
    {
        if (emotionCounters.ContainsKey(emotionType))
        {
            UpdateEmotionCounter(emotionCounters[emotionType], newValue);
        }
        else
        {
            TextMeshProUGUI emotionCounter = GetCounterByName($"{emotionType}Counter");
            if (emotionCounter != null)
            {
                AddEmotionCounter(emotionType, emotionCounter);
                UpdateEmotionCounter(emotionCounter, newValue);  
            }
            else
            {
                Debug.LogError($"Emotion Counter with name {emotionType}Counter not found!");
            }
        }
    }

    private void UpdateEmotionCounter(TextMeshProUGUI emotionCounter, int newValue)
    {
        emotionCounter.text = newValue.ToString();
    }

    private void AddEmotionCounter(EmotionType emotionType, TextMeshProUGUI emotionCounter)
    {
        emotionCounters.Add(emotionType, emotionCounter);
    }
}
