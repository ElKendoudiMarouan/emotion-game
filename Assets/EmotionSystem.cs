using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static EmotionSystem;

[RequireComponent(typeof(ConversationManager))]
public class EmotionSystem : MonoBehaviour
{
    private ConversationManager conversationManager;
    private const int primaryEmotionalGain = 3;
    private const int secondaryEmotionalGain = 1;

    public void Start()
    {
        conversationManager = Utils.GetComponent<ConversationManager>(gameObject);
    }
    public void HandlePlayerResponse(EmotionData responseEmotionData, DialogueLineData dialogue)
    {
        var desiredEmotionData = conversationManager.GetEmotionData(conversationManager.desiredEmotion);
        if (responseEmotionData.Type == conversationManager.desiredEmotion)
        {
            SameEmotionResponse(desiredEmotionData, dialogue);
        }
        else if (responseEmotionData.Type == desiredEmotionData.OppositeType)
        {
            OpposingEmotionResponse(desiredEmotionData, dialogue);
        }
        else if (responseEmotionData.Type == desiredEmotionData.SameGroupType)
        {
            SameGroupEmotionResponse(desiredEmotionData, dialogue);
        } else
        {
            NeutralEmotionResponse(responseEmotionData, dialogue);
        }
    }

    // Helper functions for different player responses
    private void SameEmotionResponse(EmotionData desiredEmotionData, DialogueLineData dialogue)
    {
        Debug.Log($"Selected the desired Emotion {desiredEmotionData.Type}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, +primaryEmotionalGain);

        var oppositeDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.OppositeType);
        oppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(oppositeDataToDesiredEmotion, -primaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.SameGroupType);
        sameGroupDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupDataToDesiredEmotion, +secondaryEmotionalGain);

        conversationManager.playerCloseness = ChangeCloseness(conversationManager.playerCloseness, +primaryEmotionalGain);
        conversationManager.statsCounterDisplayer.UpdateResponseBox(dialogue.sameResponse);
    }

    private void OpposingEmotionResponse(EmotionData desiredEmotionData, DialogueLineData dialogue)
    {
        Debug.Log($"Selected opposite to desired Emotion {desiredEmotionData.Type}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, -primaryEmotionalGain);

        var oppositeDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.OppositeType);
        oppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(oppositeDataToDesiredEmotion, +primaryEmotionalGain);

        var sameGroupToOppositeDataToDesiredEmotion = conversationManager.GetEmotionData(oppositeDataToDesiredEmotion.SameGroupType);
        sameGroupToOppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupToOppositeDataToDesiredEmotion, +secondaryEmotionalGain);

        conversationManager.playerCloseness = ChangeCloseness(conversationManager.playerCloseness, -primaryEmotionalGain);
        conversationManager.statsCounterDisplayer.UpdateResponseBox(dialogue.oppositeResponse);

    }

    private void SameGroupEmotionResponse(EmotionData desiredEmotionData, DialogueLineData dialogue)
    {
        Debug.Log($"Selected same group to desired Emotion {desiredEmotionData.Type}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, +secondaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.SameGroupType);
        sameGroupDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupDataToDesiredEmotion, +secondaryEmotionalGain);

        conversationManager.playerCloseness = ChangeCloseness(conversationManager.playerCloseness, +secondaryEmotionalGain);
        conversationManager.statsCounterDisplayer.UpdateResponseBox(dialogue.sameGroupResponse);

    }

    private void NeutralEmotionResponse(EmotionData responseEmotionData, DialogueLineData dialogue)
    {
        Debug.Log($"Selected the neutral Emotion {responseEmotionData.Type}");
        responseEmotionData.Intensity = ChangeEmotionIntensity(responseEmotionData, +secondaryEmotionalGain);
        conversationManager.statsCounterDisplayer.UpdateResponseBox(dialogue.neutralResponse);
        //todo draw card
    }

    private int ChangeEmotionIntensity(EmotionData emotionData, int variation)
    {
        var newVal = ChangeValue(emotionData.Intensity, variation);
        conversationManager.statsCounterDisplayer.AddOrUpdateEmotionCounter(emotionData.Type, newVal);
        return newVal;
    }

    private int ChangeCloseness(int value, int variation)
    {
        var newVal = ChangeValue(value, variation);
        conversationManager.statsCounterDisplayer.UpdateClosenessCounter(newVal);
        return newVal;
    }

    private int ChangeValue(int value, int variation)
    {
        return Mathf.Clamp(value + variation, 0, 100);
    }
}
