using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ConversationManager))]
public class TextDisplaySystem : MonoBehaviour
{
    private ConversationManager conversationManager;

    [SerializeField] private TextMeshProUGUI turnCounter;
    [SerializeField] private TextMeshProUGUI closenessCounter;
    [SerializeField] private TextMeshProUGUI comboCounter;
    [SerializeField] private TextMeshProUGUI objectiveCounter;
    [SerializeField] private TextMeshProUGUI responseBox;
    private Dictionary<EmotionType, TextMeshProUGUI> emotionCounters = new Dictionary<EmotionType, TextMeshProUGUI>();

    void Start()
    {
        conversationManager = Utils.GetComponentInObject<ConversationManager>(gameObject);

        UpdateTurnCounter(conversationManager.turnCounterMeter);

        UpdateClosenessCounter(conversationManager.currentCloseness);

        UpdateEmotionComboCounter(conversationManager.emotionCombo);

        UpdateResponseBox("Well Hello There!"); //todo make dynamic

        foreach (EmotionData emotionData in conversationManager.emotionDataList)
        {
            UpdateEmotionCounter(emotionData.EmotionType, emotionData.Intensity);
        }
    }

    public void UpdateTurnCounter(int newVal)
    {
        if (turnCounter == null)
        {
            turnCounter = GetTextFieldByName("TurnCounter");
        }
        turnCounter.text = $"Turn : {newVal}";
    }
    public void UpdateClosenessCounter(int newVal)
    {
        if (closenessCounter == null)
        {
            closenessCounter = GetTextFieldByName("ClosenessCounter");
        }
        closenessCounter.text = $"Closeness: {newVal}";
    }
    public void UpdateEmotionComboCounter(int newVal)
    {
        if (comboCounter == null)
        {
            comboCounter = GetTextFieldByName("ComboCounter");
        }
        comboCounter.text = $"Emotion Combo : +{newVal}";
    }
    public void UpdateObjectiveCounter(string text)
    {
        if (objectiveCounter == null)
        {
            objectiveCounter = GetTextFieldByName("ObjectiveCounter");
        }
        objectiveCounter.text = text;
    }
    public void UpdateResponseBox(string text)
    {
        if (responseBox == null)
        {
            responseBox = GetTextFieldByName("ResponseBox");
        }
        responseBox.text = text;
    }

    public TextMeshProUGUI GetTextFieldByName(string counterName) //TODO update this to recieve the field   
    {
        GameObject counterObject = GameObject.Find(counterName);
        if (counterObject != null)
        {
            TextMeshProUGUI counterText = counterObject.GetComponent<TextMeshProUGUI>();
            return counterText;
        }
        else
        {
            Debug.LogError($"Counter Text with name {counterName} not found!");
            return null;
        }
    }
 
    public void UpdateEmotionCounter(EmotionType emotionType, int newValue)
    {
        if (emotionCounters.ContainsKey(emotionType))
        {
            UpdateEmotionCounter(emotionCounters[emotionType], newValue);
        }
        else
        {
            TextMeshProUGUI emotionCounter = GetTextFieldByName($"{emotionType}Counter");
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
