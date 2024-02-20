using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConversationManager))]
public class DeckManager : MonoBehaviour
{
    private ConversationManager cm;

    public List<Card> cardsInHand = new List<Card> {};
    public List<Card> deck { get; private set; } = new List<Card>();
    public List<Card> initialDeck { get; private set; } = new List<Card>();
    private List<Card> playedCards = new List<Card> { };

    private List<Card> activeCards = new List<Card>();

    [SerializeField] private int maxCardsInHand = 3;
    [SerializeField] private int lastCardPlayedOnTurn = 0;

    private void Awake()
    {
        initialDeck = new List<Card>
        {
            new Card("Emotion Switch",CardType.EmotionSwitch, "", 0),
            new Card("Combo Protector 2 Turns", CardType.ComboKeeper, "",2),
            new Card("Negation Shield 2 Turns", CardType.NegationShield, "",2),
            new Card("Increase Patience +2", CardType.IncreasePatience, "",0),
            // Add more initial cards as needed
        };
        deck.AddRange(initialDeck);
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
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public void DrawCard()
    {
        if (cardsInHand.Count >= maxCardsInHand || deck.Count == 0)
        {
            Debug.LogWarning("Deck is empty. Unable to draw a card.");
            return;
        }

        // Draw a card from the deck
        Card drawnCard = deck[0];
        deck.RemoveAt(0);
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
            card.DurationLeft = card.Duration;
            cm.buttonCreationSystem.RepositionCardButtons();
            cm.buttonCreationSystem.ChangeCardButtonsInteractibility(false);
            cm.cardEffectManager.ApplyCardEffect(card);
        }
    }

    public Card FindCard(CardType cardType)
    {
        return initialDeck.Find(e => e.Type == cardType);
    }
}
