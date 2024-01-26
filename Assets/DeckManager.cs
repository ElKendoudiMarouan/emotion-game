using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConversationManager))]
public class DeckManager : MonoBehaviour
{
    private ConversationManager cm;

    private List<Card> cardsInHand = new List<Card> {};
    public List<Card> initialDeck { get; private set; }
    private List<Card> playedCards = new List<Card> { };

    private Dictionary<Card, GameObject> cardObjects = new Dictionary<Card, GameObject>();

    public GameObject cardButtonPrefab;
    public Transform cardButtonContainer;

    [SerializeField] private int maxCardsInHand = 3;
    private float cardsSpacing = 200f;
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
        cm = Utils.GetComponent<ConversationManager>(gameObject);

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
        CreateCardButton(drawnCard);
    }

    void CreateCardButton(Card card)
    {
        // Instantiate the button prefab
        Vector3 position = cardButtonContainer.transform.position;
        GameObject cardObject = Instantiate(cardButtonPrefab, cardButtonContainer);
        cardObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(position.x + (cardsInHand.Count - 1) * cardsSpacing, position.y);

        cardObject.gameObject.name = $"Card{card.Type}";
        cardObjects.Add(card, cardObject);
       
        Button buttonComponent = cardObject.GetComponent<Button>();

        ButtonShuffleSystem.SetButtonText(buttonComponent, card.Name);
        ButtonShuffleSystem.SetButtonColor(buttonComponent, "#7695bf");

        buttonComponent.onClick.AddListener(() => PlayCard(card));
    }

    public void PlayCard(Card card)
    {
        if (lastCardPlayedOnTurn != cm.turnCounterMeter)
        {
            lastCardPlayedOnTurn = cm.turnCounterMeter;
            cardsInHand.Remove(card);
            playedCards.Add(card);

            if (cardObjects.ContainsKey(card))
            {
                Destroy(cardObjects[card]);
            }
            RepositionCards();
            cm.cardEffectManager.ApplyCardEffect(card);
        }
    }

    private void RepositionCards()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector3 position = cardButtonContainer.transform.position;
            RectTransform cardTransform = cardObjects[cardsInHand[i]].GetComponent<RectTransform>();
            cardTransform.anchoredPosition = new Vector2(position.x + cardsSpacing * i, position.y);
        }
    }

}
