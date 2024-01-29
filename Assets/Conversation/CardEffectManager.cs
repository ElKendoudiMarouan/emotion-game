using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ConversationManager))]
public class CardEffectManager : MonoBehaviour
{
    private ConversationManager cm;

    public Dictionary<CardType, Card>  cardEffectsActiveThisTurn = new Dictionary<CardType, Card>();

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
        CheckCardEffectsThisTurn(false);
    }

    public void CheckCardEffectsThisTurn(bool onPassTurn)
    {
        cm.emotionSystem.protectComboThisTurn = cm.emotionSystem.comboProtectorTurns.Contains(cm.turnCounterMeter); //TODO change this logic
        AddOrRemoveFromActiveCardsList(CardType.ComboKeeper, cm.emotionSystem.protectComboThisTurn, onPassTurn);

        cm.emotionSystem.shieldEmotionFromNegationThisTurn = cm.emotionSystem.emotionNegationShieldTurns.Contains(cm.turnCounterMeter); //TODO change this logic
        AddOrRemoveFromActiveCardsList(CardType.NegationShield, cm.emotionSystem.shieldEmotionFromNegationThisTurn, onPassTurn);

        UpdateActiveCards(cardEffectsActiveThisTurn.Values.ToList());
    }

    private void AddOrRemoveFromActiveCardsList(CardType cardType, bool isActiveThisTurn, bool onPassTurn)
    {
        if (isActiveThisTurn)
        {
            Card card = cm.deckManager.FindCard(cardType);
            if (!cardEffectsActiveThisTurn.ContainsKey(cardType))
            {
                cardEffectsActiveThisTurn.Add(cardType, card);
            }
            if (onPassTurn)
            {
                card.DurationLeft--;
            }
        }
        else if (cardEffectsActiveThisTurn.ContainsKey(cardType))
        {
            cardEffectsActiveThisTurn.Remove(cardType);
        }
    }

    public void UpdateActiveCards(List<Card> activeCards)
    {
        // Update the icons in the UI
        cm.spriteDisplaySystem.UpdateActiveCardEffectIcons(activeCards);
    }

    public void ApplyOppositeDesiredCardEffect()
    {
        EmotionData desiredEmotionData = cm.GetEmotionData(cm.desiredEmotion);

        cm.UpdateDesiredEmotion(desiredEmotionData.OppositeType);
        cm.emotionDialogueChoices.ForEach(emotionDialogue =>
        {
            emotionDialogue.ResponseType = cm.GetResponseType(emotionDialogue.EmotionData);
            cm.buttonCreationSystem.UpdateDialogButtonHoverColor(emotionDialogue);
        });
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
