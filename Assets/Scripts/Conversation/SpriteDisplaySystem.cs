using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConversationManager))]
public class SpriteDisplaySystem : MonoBehaviour
{
    private ConversationManager conversationManager;

    private Image desiredEmotionImage;
    private Image comboEmotionImage;
    public Image PortraitContainerImage;

    public Transform cardEffectBarContainer;
    public GameObject cardEffectIconPrefab;

    [SerializeField] private string desiredEmotionIconName = "DesiredEmotionIcon";
    [SerializeField] private string comboEmotionIconName = "ComboEmotionIcon";
    [SerializeField] private string buttonEmotionIconName = "ButtonEmotionIcon";
    [SerializeField] private string portraitContainerName = "PortraitContainer";

    // Name of the folder inside  Resources base folder where emotion icons folder is located
    private const string emotionSpritesFolder = "Sprites/Emotions";
    private const string portraitSpritesFolder = "Sprites/Portraits";
    private const string cardIconsSpritesFolder = "Sprites/CardIcons";

    public float cardEffectBarSpacing = 50f;

    public void Start()
    {
        conversationManager = Utils.GetComponentInObject<ConversationManager>(gameObject);

        if (cardEffectBarContainer == null)
        {
            cardEffectBarContainer = Utils.RecursiveFindChild(gameObject.transform, "CardEffectBarContainer");
        }

        UpdateNPCPortrait(null);
        UpdateComboEmotionIcon(null);
    }

    public void UpdateDesiredEmotionIcon(EmotionType emotionType)
    {

        if (desiredEmotionImage == null)
        {
            desiredEmotionImage = Utils.RecursiveGetComponentByObjectName<Image>(desiredEmotionIconName, gameObject.transform);
        }

        UpdateEmotionIcon(emotionType, desiredEmotionImage);
    }

    public void UpdateComboEmotionIcon(EmotionType? emotionType)
    {
        if (comboEmotionImage == null)
        {
            comboEmotionImage = Utils.RecursiveGetComponentByObjectName<Image>(comboEmotionIconName, gameObject.transform);
        }

        UpdateEmotionIcon(emotionType, comboEmotionImage);
    }

    public void UpdateNPCPortrait(EmotionType? emotion)
    {
        if (PortraitContainerImage == null)
        {
            PortraitContainerImage = Utils.RecursiveGetComponentByObjectName<Image>(portraitContainerName, gameObject.transform);
        }

        var spriteSheet = LoadSpriteSheet($"{portraitSpritesFolder}/portrait-policeman"); //TODO make it dynamic
        var spriteIndex = 0;
        if (emotion != null)
        {
            spriteIndex = Utils.GetPositionByEnumValue<EmotionType>((EmotionType)emotion) + 1;
        }

        //PortraitContainerImage.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1f);
        PortraitContainerImage.sprite = spriteSheet[spriteIndex];
    }

    public Image FindAndUpdateButtonEmotionIcon(Transform buttonTranform, EmotionType emotionType)
    {
        var iconRenderer = Utils.RecursiveGetComponentByObjectName<Image>(buttonEmotionIconName, buttonTranform);
        iconRenderer.enabled = false;

        UpdateEmotionIcon(emotionType, iconRenderer);

        return iconRenderer;
    }

    private void UpdateEmotionIcon(EmotionType? desiredEmotion, Image emotionSpriteImage)
    {
        if (desiredEmotion != null)
        {
            Sprite emotionSprite = LoadSprite($"{emotionSpritesFolder}/{desiredEmotion}Icon");

            emotionSpriteImage.sprite = emotionSprite;
        } else
        {
            emotionSpriteImage.sprite = null;
        }
    }

    public void UpdateActiveCardEffectIcons(List<Card> activeCards)
    {
        // Clear existing icons
        foreach (Transform child in cardEffectBarContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < activeCards.Count; i++)
        {
            GameObject sumamryObject = Instantiate(cardEffectIconPrefab, cardEffectBarContainer);
            Vector3 position = cardEffectBarContainer.transform.position;
            sumamryObject.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(position.x + i * cardEffectBarSpacing, position.y);

            sumamryObject.gameObject.name = $"CardSummary{activeCards[i].Type}";

            CardInformationsDisplay cardInfoDisplay = sumamryObject.GetComponent<CardInformationsDisplay>();

            UpdateCardIcon(activeCards[i], cardInfoDisplay);
            cardInfoDisplay.SetDurationText(activeCards[i].DurationLeft);
        }
    }

    public void UpdateCardIcon(Card card, CardInformationsDisplay cardDisplay)
    {
        if (card.Icon == null)
        {
            card.Icon = LoadSprite($"{cardIconsSpritesFolder}/{card.Type}");
        }
        cardDisplay.SetIcon(card.Icon);
    }


    private Sprite LoadSprite(string pathInResources)
    {
        // Load the sprite dynamically from the Resources folder
        Sprite sprite = Resources.Load<Sprite>(pathInResources);
        if(sprite != null)
        {
            return sprite;
        } else
        {
            Debug.LogError($"No sprite found in  path: {pathInResources}.");
            return null;
        }
    }

    private Sprite[] LoadSpriteSheet(string pathInResources)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(pathInResources);
        if (sprites.Length > 0)
        {
            return sprites;
        }
        else
        {
            Debug.LogError($"No sprite sheet found in path: {pathInResources}.");
            return new Sprite[0]; // Return an empty Sprite array
        }
    }
}