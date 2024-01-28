using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConversationManager))]
public class DeckManager : MonoBehaviour
{
    private ConversationManager cm;

    public List<Card> cardsInHand = new List<Card> {};
    public List<Card> initialDeck { get; private set; }
    private List<Card> playedCards = new List<Card> { };

    [SerializeField] private int maxCardsInHand = 3;
    [SerializeField] private int lastCardPlayedOnTurn = 0;

    private void Awake()
    {
        initialDeck = new List<Card>
        {
            new Card("Emotion Switch",CardType.EmotionSwitch),
            new Card("Combo Protector 2 Turns", CardType.ComboKeeper),
            new Card("Negation Shield 2 Turns", CardType.NegationShield),
            new Card("Increase Patience +2", CardType.IncreasePatience),
            // Add more initial cards as needed
        };
    }

    void Start()
    {
        cm = Utils.GetComponentInObject<ConversationManager>(gameObject);

        //deckSystem = gameObject.AddComponent<DeckManager>();
        ShuffleDeck();
        DrawCard();
    }

    public void ShuffleDeck()
    {
        // Implement card shuffling logic
        for (int i = 0; i < initialDeck.Count; i++)
        {
            int randomIndex = Random.Range(i, initialDeck.Count);
            Card temp = initialDeck[i];
            initialDeck[i] = initialDeck[randomIndex];
            initialDeck[randomIndex] = temp;
        }
    }

    public void DrawCard()
    {
        if (cardsInHand.Count >= maxCardsInHand || initialDeck.Count == 0)
        {
            Debug.LogWarning("Deck is empty. Unable to draw a card.");
            return;
        }

        // Draw a card from the deck
        Card drawnCard = initialDeck[0];
        initialDeck.RemoveAt(0);
        cardsInHand.Add(drawnCard);
        cm.buttonCreationSystem.CreateCardButton(drawnCard, cardsInHand.Count);
    }

    public void PlayCard(Card card)
    {
        if (lastCardPlayedOnTurn != cm.turnCounterMeter)
        {
            lastCardPlayedOnTurn = cm.turnCounterMeter;
            cardsInHand.Remove(card);
            playedCards.Add(card);

            if (cm.buttonCreationSystem.cardButtonObjects.ContainsKey(card))
            {
                cm.buttonCreationSystem.DestroyCard(card);
            }
            cm.buttonCreationSystem.RepositionCardButtons();
            cm.buttonCreationSystem.ChangeCardButtonsInteractibility(false);
            cm.cardEffectManager.ApplyCardEffect(card);
        }
    }
}
