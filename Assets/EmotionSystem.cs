using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static EmotionSystem;

[RequireComponent(typeof(ConversationManager))]
public class EmotionSystem : MonoBehaviour
{
    private ConversationManager cm;
    private const int primaryEmotionalGain = 3;
    private const int secondaryEmotionalGain = 1;
    private const int comboGain = 1;
    private const int comboThreshold = 3;
    private const int patienceVariation = 1;
    private const int patienceMaxVariation = 2;

    public void Start()
    {
        cm = Utils.GetComponent<ConversationManager>(gameObject);
    }
    public void HandlePlayerResponse(EmotionData responseEmotionData, DialogueLineData dialogue)
    {
        updateEmotionComboAndLatestSelectedEmotion(responseEmotionData);

        var desiredEmotionData = cm.GetEmotionData(cm.desiredEmotion);
        if (responseEmotionData.Type == cm.desiredEmotion)
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

    private void updateEmotionComboAndLatestSelectedEmotion(EmotionData responseEmotionData)
    {
        if (cm.lastSelectedEmotion != null)
        {
            if (cm.lastComboEmotion != null && cm.lastComboEmotion.Type == responseEmotionData.Type)
            {
                if (cm.emotionCombo == 2)
                {
                    cm.patienceManager.UpdatePatience(patienceMaxVariation); //+2 patience when combo
                }
                ChangeEmotionCombo(+comboGain);
                changeLastComboEmotion(responseEmotionData);
            } else if (cm.lastComboEmotion == cm.lastSelectedEmotion && cm.lastComboEmotion.SameGroupType != responseEmotionData.Type)
            {
                    ChangeEmotionCombo(-comboThreshold);
                    changeLastComboEmotion(null);

            } else if (cm.lastComboEmotion != cm.lastSelectedEmotion)
            {
                ChangeEmotionCombo(-comboThreshold); //reset combo
                if (cm.lastSelectedEmotion == responseEmotionData)
                {
                    ChangeEmotionCombo(+comboGain);
                    changeLastComboEmotion(responseEmotionData);
                } else
                {
                    changeLastComboEmotion(null);
                }
            }              
        }
        cm.lastSelectedEmotion = responseEmotionData;
    }

    // Helper functions for different player responses
    private void SameEmotionResponse(EmotionData desiredEmotionData, DialogueLineData dialogue)
    {
        Debug.Log($"Selected the desired Emotion {desiredEmotionData.Type}");

        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, +primaryEmotionalGain +cm.emotionCombo);

        var oppositeDataToDesiredEmotion = cm.GetEmotionData(desiredEmotionData.OppositeType);
        oppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(oppositeDataToDesiredEmotion, -primaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = cm.GetEmotionData(desiredEmotionData.SameGroupType);
        sameGroupDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupDataToDesiredEmotion, +secondaryEmotionalGain);

        ChangeCloseness(+primaryEmotionalGain);
        cm.patienceManager.UpdatePatience(patienceVariation);

        cm.statsCounterDisplayer.UpdateResponseBox(dialogue.sameResponse);
    }

    private void OpposingEmotionResponse(EmotionData desiredEmotionData, DialogueLineData dialogue)
    {
        Debug.Log($"Selected opposite to desired Emotion {desiredEmotionData.Type}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, -primaryEmotionalGain);

        var oppositeDataToDesiredEmotion = cm.GetEmotionData(desiredEmotionData.OppositeType);
        oppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(oppositeDataToDesiredEmotion, +primaryEmotionalGain + cm.emotionCombo);

        var sameGroupToOppositeDataToDesiredEmotion = cm.GetEmotionData(oppositeDataToDesiredEmotion.SameGroupType);
        sameGroupToOppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupToOppositeDataToDesiredEmotion, +secondaryEmotionalGain);

        ChangeCloseness(-primaryEmotionalGain);
        cm.patienceManager.UpdatePatience(-patienceMaxVariation);

        cm.statsCounterDisplayer.UpdateResponseBox(dialogue.oppositeResponse);
    }

    private void SameGroupEmotionResponse(EmotionData desiredEmotionData, DialogueLineData dialogue)
    {
        Debug.Log($"Selected same group to desired Emotion {desiredEmotionData.Type}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, +secondaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = cm.GetEmotionData(desiredEmotionData.SameGroupType);
        sameGroupDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupDataToDesiredEmotion, +secondaryEmotionalGain + cm.emotionCombo);

        ChangeCloseness(+secondaryEmotionalGain);

        cm.statsCounterDisplayer.UpdateResponseBox(dialogue.sameGroupResponse);

    }

    private void NeutralEmotionResponse(EmotionData responseEmotionData, DialogueLineData dialogue)
    {
        Debug.Log($"Selected the neutral Emotion {responseEmotionData.Type}");
        responseEmotionData.Intensity = ChangeEmotionIntensity(responseEmotionData, +secondaryEmotionalGain);

        cm.patienceManager.UpdatePatience(-patienceVariation);

        cm.statsCounterDisplayer.UpdateResponseBox(dialogue.neutralResponse);
        //todo draw card
    }

    private int ChangeEmotionIntensity(EmotionData emotionData, int variation)
    {
        var newVal = ChangeValue(emotionData.Intensity, variation);
        cm.statsCounterDisplayer.AddOrUpdateEmotionCounter(emotionData.Type, newVal);
        return newVal;
    }

    private void ChangeCloseness(int variation)
    {
        cm.playerCloseness = ChangeValue(cm.playerCloseness, variation);
        cm.statsCounterDisplayer.UpdateClosenessCounter(cm.playerCloseness);
    }

    private void changeLastComboEmotion(EmotionData emotion)
    {
        cm.lastComboEmotion = emotion;
        cm.emotionSpriteDisplayer.UpdateComboEmotionIcon(emotion?.Type);
    }
     
    private void ChangeEmotionCombo(int variation)
    {
        cm.emotionCombo = ChangeValue(cm.emotionCombo, variation, comboThreshold);
        cm.statsCounterDisplayer.UpdateEmotionComboCounter(cm.emotionCombo);
    }

    private int ChangeValue(int value, int variation, int threshold = 100)
    {
        return Mathf.Clamp(value + variation, 0, threshold);
    }
}
