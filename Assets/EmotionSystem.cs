using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static EmotionSystem;

[RequireComponent(typeof(ConversationManager))]
public class EmotionSystem : MonoBehaviour
{
    private ConversationManager conversationManager;
    public int playerCloseness = 50;
    private const int primaryEmotionalGain = 3;
    private const int secondaryEmotionalGain = 1;

    public void Start()
    {
        conversationManager = GetComponent<ConversationManager>();
    }
    public void HandlePlayerResponse(EmotionData responseEmotionData)
    {
        var desiredEmotionData = conversationManager.GetEmotionData(conversationManager.desiredEmotion);
        if (responseEmotionData.Emotion == conversationManager.desiredEmotion)
        {
            SameEmotionResponse(desiredEmotionData);
        }
        else if (responseEmotionData.Emotion == desiredEmotionData.Opposite)
        {
            OpposingEmotionResponse(desiredEmotionData);
        }
        else if (responseEmotionData.Emotion == desiredEmotionData.SameGroup)
        {
            SameGroupEmotionResponse(desiredEmotionData);
        } else
        {
            NeutralEmotionResponse(desiredEmotionData);
        }
    }

    // Helper functions for different player responses
    private void SameEmotionResponse(EmotionData desiredEmotionData)
    {
        desiredEmotionData.Intensity += primaryEmotionalGain;

        var oppositeDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.Opposite);
        oppositeDataToDesiredEmotion.Intensity = ChangeIntensity(oppositeDataToDesiredEmotion.Intensity, -primaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.SameGroup);
        sameGroupDataToDesiredEmotion.Intensity = ChangeIntensity(sameGroupDataToDesiredEmotion.Intensity, +secondaryEmotionalGain);

        playerCloseness = ChangeIntensity(playerCloseness, +primaryEmotionalGain);

    }

    private void OpposingEmotionResponse(EmotionData desiredEmotionData)
    {
        desiredEmotionData.Intensity = ChangeIntensity(desiredEmotionData.Intensity, -primaryEmotionalGain);

        var oppositeDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.Opposite);
        oppositeDataToDesiredEmotion.Intensity = ChangeIntensity(oppositeDataToDesiredEmotion.Intensity, +primaryEmotionalGain);

        var sameGroupToOppositeDataToDesiredEmotion = conversationManager.GetEmotionData(oppositeDataToDesiredEmotion.SameGroup);
        sameGroupToOppositeDataToDesiredEmotion.Intensity = ChangeIntensity(sameGroupToOppositeDataToDesiredEmotion.Intensity, +secondaryEmotionalGain);

        playerCloseness = ChangeIntensity(playerCloseness, -primaryEmotionalGain);
    }

    private void SameGroupEmotionResponse(EmotionData desiredEmotionData)
    {
        desiredEmotionData.Intensity = ChangeIntensity(desiredEmotionData.Intensity, +secondaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = conversationManager.GetEmotionData(desiredEmotionData.SameGroup);
        sameGroupDataToDesiredEmotion.Intensity = ChangeIntensity(sameGroupDataToDesiredEmotion.Intensity, +secondaryEmotionalGain);

        playerCloseness = ChangeIntensity(playerCloseness, +secondaryEmotionalGain);
    }

    private void NeutralEmotionResponse(EmotionData responseEmotionData)
    {
        responseEmotionData.Intensity += secondaryEmotionalGain;
        //todo draw card
    }

    private int ChangeIntensity(int value, int variation)
    {
        return Mathf.Clamp(value + variation, 0, 100);
    }
}
