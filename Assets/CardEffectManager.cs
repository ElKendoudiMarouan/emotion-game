using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConversationManager))]
public class CardEffectManager : MonoBehaviour
{
    private ConversationManager cm;

    void Start()
    {
        cm = Utils.GetComponentInObject<ConversationManager>(gameObject);
    }

    public void ApplyCardEffect(Card card)
    {
        switch (card.Type)
        {
            case CardType.EmotionSwitch:
                ApplyOppositeDesiredCardEffect();
                break;
            case CardType.ComboKeeper:
                ApplyComboKeeperCardEffect();
                break;
            case CardType.NegationShield:
                ApplyNegationShieldCardEffect();
                break;
            case CardType.IncreasePatience:
                ApplyIncreasePatienceCardEffect();
                break;
                // Handle other card types
        }
        CheckCardEffectsThisTurn();
    }
    public void CheckCardEffectsThisTurn()
    {
        cm.emotionSystem.protectComboThisTurn = cm.emotionSystem.comboProtectorTurns.Contains(cm.turnCounterMeter);
        cm.emotionSystem.shieldEmotionFromNegationThisTurn = cm.emotionSystem.emotionNegationShieldTurns.Contains(cm.turnCounterMeter);
    }

    public void ApplyOppositeDesiredCardEffect()
    {
        EmotionData desiredEmotionData = cm.GetEmotionData(cm.desiredEmotion);

        cm.UpdateDesiredEmotion(desiredEmotionData.OppositeType);
    }

    public void ApplyComboKeeperCardEffect()
    {
        cm.emotionSystem.comboProtectorTurns = Utils.CreateIncrementingList(cm.turnCounterMeter, 2);
    }

    public void ApplyNegationShieldCardEffect()
    {
        cm.emotionSystem.emotionNegationShieldTurns = Utils.CreateIncrementingList(cm.turnCounterMeter, 2);

    }

    public void ApplyIncreasePatienceCardEffect()
    {

        cm.patienceManager.UpdatePatience(2);
    }
}
