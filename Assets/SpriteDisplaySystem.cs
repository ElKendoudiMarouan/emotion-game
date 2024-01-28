using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ConversationManager))]
public class SpriteDisplaySystem : MonoBehaviour
{
    private SpriteRenderer desiredEmotionSpriteRenderer;
    private SpriteRenderer comboEmotionSpriteRenderer;
    public Image PortraitContainerImage;

    private ConversationManager conversationManager;
    private float desiredIconSize = 100f;
    private float buttonIconSize = 100f;
    [SerializeField] private string desiredEmotionIconName = "DesiredEmotionIcon";
    [SerializeField] private string comboEmotionIconName = "ComboEmotionIcon";
    [SerializeField] private string buttonEmotionIconName = "ButtonEmotionIcon";
    [SerializeField] private string portraitContainerName = "PortraitContainer";

    // Name of the folder inside  Resources base folder where emotion icons folder is located
    private const string emotionSpritesFolder = "Sprites/Emotions";
    private const string portraitSpritesFolder = "Sprites/Portraits";

    public void Start()
    {
        conversationManager = Utils.GetComponentInObject<ConversationManager>(gameObject);

        UpdateNPCPortrait(null);
        UpdateComboEmotionIcon(null);
    }

    public void UpdateDesiredEmotionIcon(EmotionType emotionType)
    {

        if (desiredEmotionSpriteRenderer == null)
        {
            desiredEmotionSpriteRenderer = Utils.RecursiveGetComponentByObjectName<SpriteRenderer>(desiredEmotionIconName, gameObject.transform);
        }

        UpdateEmotionIcon(emotionType, desiredIconSize, desiredEmotionSpriteRenderer);
    }

    public void UpdateComboEmotionIcon(EmotionType? emotionType)
    {
        if (comboEmotionSpriteRenderer == null)
        {
            comboEmotionSpriteRenderer = Utils.RecursiveGetComponentByObjectName<SpriteRenderer>(comboEmotionIconName, gameObject.transform);
        }

        UpdateEmotionIcon(emotionType, buttonIconSize, comboEmotionSpriteRenderer);
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

    public SpriteRenderer FindAndUpdateButtonEmotionIcon(Transform buttonTranform, EmotionType emotionType)
    {
        var iconRenderer = Utils.RecursiveGetComponentByObjectName<SpriteRenderer>(buttonEmotionIconName, buttonTranform);
        iconRenderer.enabled = false;

        UpdateEmotionIcon(emotionType, buttonIconSize, iconRenderer);

        return iconRenderer;
    }

    private void UpdateEmotionIcon(EmotionType? desiredEmotion, float iconSize, SpriteRenderer emotionSpriteRenderer)
    {
        if (desiredEmotion != null)
        {
            Sprite emotionSprite = LoadSprite($"{emotionSpritesFolder}/{desiredEmotion}Icon");

            float originalWidth = emotionSprite.bounds.size.x;
            float originalHeight = emotionSprite.bounds.size.y;

            float scaleFactorX = originalWidth != 0 ? iconSize / originalWidth : .1f;
            float scaleFactorY = originalHeight != 0 ? iconSize / originalHeight : .1f;

            emotionSpriteRenderer.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1f);
            emotionSpriteRenderer.sprite = emotionSprite;
        } else
        {
            emotionSpriteRenderer.sprite = null;
        }
    }

    private Sprite LoadSprite(string pathInResources)
    {
        // Load the sprite dynamically from the Resources folder
        return Resources.Load<Sprite>(pathInResources);
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
            Debug.LogError("No sprites found in the sprite sheet.");
            return new Sprite[0]; // Return an empty Sprite array
        }
    }
}