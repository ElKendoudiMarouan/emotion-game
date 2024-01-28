using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConversationManager))]
public class EmotionSystem : MonoBehaviour
{
    private ConversationManager cm;
    [Header("Emotion Gain")]
    private const int primaryEmotionalGain = 3;
    private const int secondaryEmotionalGain = 1;
    public List<int> emotionNegationShieldTurns = new List<int> { };
    public bool shieldEmotionFromNegationThisTurn { get; set; } = false;
    [Header("Combo Gain")]
    private const int comboGain = 1;
    private const int comboThreshold = 5;
    public List<int> comboProtectorTurns = new List<int> { };
    public bool protectComboThisTurn { get; set; } = false;
    [Header("Patience Gain")]
    private const int patienceVariation = 1;
    private const int patienceMaxVariation = 2;

    public void Start()
    {
        cm = Utils.GetComponentInObject<ConversationManager>(gameObject);
    }

    public void HandlePlayerResponse(EmotionDialogueChoice emotionDialogue)
    {
        updateEmotionComboAndLatestSelectedEmotion(emotionDialogue);

        var desiredEmotionData = cm.GetEmotionData(cm.desiredEmotion);
        switch (emotionDialogue.ResponseType)
        {
            case  ResponseType.Identical:
                IdenticalEmotionResponse(emotionDialogue, desiredEmotionData);
                break;
            case ResponseType.Close:
                CloseEmotionResponse(emotionDialogue, desiredEmotionData);
                break;
            case ResponseType.Opposite:
                OpposingEmotionResponse(emotionDialogue, desiredEmotionData);
                break;
            case ResponseType.Neutral:
                NeutralEmotionResponse(emotionDialogue);
                break;
        }
    }

    /**
     *  combo =/= last && resp = combo -> +1
        combo=/= last && resp = last -> change combo
        combo=/= last && resp =/= combo && resp =/= last -> remove
        combo == last && resp = combo -> +1
        combo == last && resp =/= combo ---> resp == combo.samegroup -> keep
        combo == last && resp =/= combo |--> resp =/= samegeoup -> remove combo
     */
    private void updateEmotionComboAndLatestSelectedEmotion(EmotionDialogueChoice emotionDialogue)
    {
        int comboVariation = 0;
        var responseEmotionData = emotionDialogue.EmotionData;
        if (cm.lastSelectedEmotion != null)
        {
            if (cm.lastComboEmotion != null && cm.lastComboEmotion.EmotionType == responseEmotionData.EmotionType)
            {
                if (cm.emotionCombo == 2 || cm.emotionCombo == 4)
                {
                    cm.patienceManager.UpdatePatience(cm.emotionCombo); //+2/4 patience two times when combo (in 3 and 5)
                }
                comboVariation = +comboGain;
                changeLastComboEmotion(responseEmotionData);
            } else if (cm.lastComboEmotion == cm.lastSelectedEmotion && cm.lastComboEmotion.SameGroupType != responseEmotionData.EmotionType && !protectComboThisTurn)
            {
                    comboVariation = -comboThreshold;
                    changeLastComboEmotion(null);

            } else if (cm.lastComboEmotion != cm.lastSelectedEmotion && !protectComboThisTurn)
            {
                comboVariation = -comboThreshold; //reset combo
                if (cm.lastSelectedEmotion == responseEmotionData) //case new combo emotion
                {
                    comboVariation = + comboGain;
                    changeLastComboEmotion(responseEmotionData);
                } else
                {
                    changeLastComboEmotion(null);
                }
            }   
            //keep combo if all conditions fail
        }
        ChangeEmotionCombo(comboVariation);
        cm.lastSelectedEmotion = responseEmotionData;
    }

    private void IdenticalEmotionResponse(EmotionDialogueChoice emotionDialogue, EmotionData desiredEmotionData)
    {
        Debug.Log($"Selected the desired Emotion {desiredEmotionData.EmotionType}");

        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, +primaryEmotionalGain +cm.emotionCombo);

        var oppositeDataToDesiredEmotion = cm.GetEmotionData(desiredEmotionData.OppositeType);
        oppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(oppositeDataToDesiredEmotion, -primaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = cm.GetEmotionData(desiredEmotionData.SameGroupType);
        sameGroupDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupDataToDesiredEmotion, +secondaryEmotionalGain);

        ChangeCloseness(+primaryEmotionalGain);
        cm.patienceManager.UpdatePatience(patienceVariation);

        cm.textDisplaySystem.UpdateResponseBox(emotionDialogue.DialogueLineData.sameResponse);
    }


    private void CloseEmotionResponse(EmotionDialogueChoice emotionDialogue, EmotionData desiredEmotionData)
    {
        Debug.Log($"Selected same group to desired Emotion {desiredEmotionData.EmotionType}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, +secondaryEmotionalGain);

        var sameGroupDataToDesiredEmotion = cm.GetEmotionData(desiredEmotionData.SameGroupType);
        sameGroupDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupDataToDesiredEmotion, +secondaryEmotionalGain + cm.emotionCombo);

        ChangeCloseness(+secondaryEmotionalGain);

        cm.textDisplaySystem.UpdateResponseBox(emotionDialogue.DialogueLineData.sameGroupResponse);

    }

    private void OpposingEmotionResponse(EmotionDialogueChoice emotionDialogue, EmotionData desiredEmotionData)
    {
        Debug.Log($"Selected opposite to desired Emotion {desiredEmotionData.EmotionType}");
        desiredEmotionData.Intensity = ChangeEmotionIntensity(desiredEmotionData, -primaryEmotionalGain);

        var oppositeDataToDesiredEmotion = cm.GetEmotionData(desiredEmotionData.OppositeType);
        oppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(oppositeDataToDesiredEmotion, +primaryEmotionalGain + cm.emotionCombo);

        var sameGroupToOppositeDataToDesiredEmotion = cm.GetEmotionData(oppositeDataToDesiredEmotion.SameGroupType);
        sameGroupToOppositeDataToDesiredEmotion.Intensity = ChangeEmotionIntensity(sameGroupToOppositeDataToDesiredEmotion, +secondaryEmotionalGain);

        ChangeCloseness(-primaryEmotionalGain);
        cm.patienceManager.UpdatePatience(-patienceMaxVariation);

        cm.textDisplaySystem.UpdateResponseBox(emotionDialogue.DialogueLineData.oppositeResponse);
    }

    private void NeutralEmotionResponse(EmotionDialogueChoice emotionDialogue)
    {
        Debug.Log($"Selected the neutral Emotion {emotionDialogue.EmotionData.EmotionType}");
        emotionDialogue.EmotionData.Intensity = ChangeEmotionIntensity(emotionDialogue.EmotionData, +secondaryEmotionalGain);

        cm.patienceManager.UpdatePatience(-patienceVariation);

        cm.textDisplaySystem.UpdateResponseBox(emotionDialogue.DialogueLineData.neutralResponse);
        cm.deckManager.DrawCard();
    }

    private int ChangeEmotionIntensity(EmotionData emotionData, int variation)
    {
        var finalVariation = shieldEmotionFromNegationThisTurn && variation < 0 ?  0 : variation;

        var newVal = ChangeValue(emotionData.Intensity, finalVariation);
        cm.textDisplaySystem.UpdateEmotionCounter(emotionData.EmotionType, newVal);
        return newVal;
    }

    private void ChangeCloseness(int variation)
    {
        cm.currentCloseness = ChangeValue(cm.currentCloseness, variation);
        cm.textDisplaySystem.UpdateClosenessCounter(cm.currentCloseness);
    }

    private void changeLastComboEmotion(EmotionData emotion)
    {
        cm.lastComboEmotion = emotion;
        cm.spriteDisplaySystem.UpdateComboEmotionIcon(emotion?.EmotionType);
    }
     
    private void ChangeEmotionCombo(int variation)
    {
        int finalVariation = protectComboThisTurn && variation < 0  ? 0: variation; //toProtectCombo

        cm.emotionCombo = ChangeValue(cm.emotionCombo, finalVariation, comboThreshold);
        cm.textDisplaySystem.UpdateEmotionComboCounter(cm.emotionCombo);
    }

    private int ChangeValue(int value, int variation, int threshold = 100)
    {
        return Mathf.Clamp(value + variation, 0, threshold);
    }
}
