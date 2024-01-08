using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static EmotionSystem;

[RequireComponent(typeof(ConversationManager))]
[RequireComponent(typeof(EmotionCounterDisplayer))]
public class EmotionSystem : MonoBehaviour
{
    private ConversationManager conversationManager;
    private EmotionCounterDisplayer emotionCounterDisplayer;
    private const int primaryEmotionalGain = 3;
    private const int secondaryEmotionalGain = 1;

    public void Start()
    {
        conversationManager = GetComponent<ConversationManager>();
        emotionCounterDisplayer = GetComponent<EmotionCounterDisplayer>();
    }
    public void HandlePlayerResponse(EmotionData responseEmotionData)
    {
        var desiredEmotionData = conversationManager.GetEmotionData(conversationManager.desiredEmotion);
        if (responseEmotionData.Type == conversationManager.desiredEmotion)
        {
            SameEmotionResponse(desiredEmotionData);
        }
        else if (responseEmotionData.Type == desiredEmotionData.OppositeType)
        {
            OpposingEmotionResponse(desiredEmotionData);
        }
        else if (responseEmotionData.Type == desiredEmotionData.SameGroupType)
        {
            SameGroupEmotionResponse(desiredEmotionData);
        } else
        {
            NeutralEmotionResponse(responseEmotionData);
        }
    }

    // Helper functions for different player responses
    private void SameEmotionResponse(EmotionData desiredEmotionData)
    {
        Debug.Log($"Selected the desired Emotion {desiredEmotionData.Type}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, +primaryEmotionalGain);

        var oppositeDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.OppositeType);
        oppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(oppositeDataToDesiredEmotion, -primaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.SameGroupType);
        sameGroupDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupDataToDesiredEmotion, +secondaryEmotionalGain);

        conversationManager.playerCloseness = ChangeCloseness(conversationManager.playerCloseness, +primaryEmotionalGain);

    }

    private void OpposingEmotionResponse(EmotionData desiredEmotionData)
    {
        Debug.Log($"Selected opposite to desired Emotion {desiredEmotionData.Type}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, -primaryEmotionalGain);

        var oppositeDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.OppositeType);
        oppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(oppositeDataToDesiredEmotion, +primaryEmotionalGain);

        var sameGroupToOppositeDataToDesiredEmotion = conversationManager.GetEmotionData(oppositeDataToDesiredEmotion.SameGroupType);
        sameGroupToOppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupToOppositeDataToDesiredEmotion, +secondaryEmotionalGain);

        conversationManager.playerCloseness = ChangeCloseness(conversationManager.playerCloseness, -primaryEmotionalGain);
    }

    private void SameGroupEmotionResponse(EmotionData desiredEmotionData)
    {
        Debug.Log($"Selected same group to desired Emotion {desiredEmotionData.Type}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, +secondaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.SameGroupType);
        sameGroupDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupDataToDesiredEmotion, +secondaryEmotionalGain);

        conversationManager.playerCloseness = ChangeCloseness(conversationManager.playerCloseness, +secondaryEmotionalGain);
    }

    private void NeutralEmotionResponse(EmotionData responseEmotionData)
    {
        Debug.Log($"Selected the neutral Emotion {responseEmotionData.Type}");
        responseEmotionData.Intensity = ChangeEmotionIntensity(responseEmotionData, +secondaryEmotionalGain);
        //todo draw card
    }

    private int ChangeEmotionIntensity(EmotionData emotionData, int variation)
    {
        var newVal = ChangeValue(emotionData.Intensity, variation);
        emotionCounterDisplayer.AddOrUpdateEmotionCounter(emotionData.Type, newVal);
        return newVal;
    }

    private int ChangeCloseness(int value, int variation)
    {
        var newVal = ChangeValue(value, variation);
        emotionCounterDisplayer.UpdateClosenessCounter(newVal);
        return newVal;

    }

    private int ChangeValue(int value, int variation)
    {
        return Mathf.Clamp(value + variation, 0, 100);
    }
}
