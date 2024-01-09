using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(ConversationManager))]
public class EmotionSpriteDisplayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer desiredEmotionSpriteRenderer;//todo remove this
    private ConversationManager conversationManager;
    private float desiredIconSize = 1.2f;
    private float buttonIconSize = 100f;
    [SerializeField] private string desiredEmotionIconName = "DesiredEmotionIcon";
    [SerializeField] private string buttonEmotionIconName = "ButtonEmotionIcon";

    // Name of the folder inside  Resources base folder where emotion icons folder is located
    private const string ResourcesFolder = "EmotionSprites/";

    public void Start()
    {
        conversationManager = GetComponent<ConversationManager>();

        if (desiredEmotionSpriteRenderer == null)
        {
            Debug.Log($"looking for desired emotion in {gameObject.transform}");
            desiredEmotionSpriteRenderer = FindEmotionIconSpriteRendererByName(desiredEmotionIconName, gameObject.transform);
        }

        UpdateDesiredEmotionIcon(conversationManager.desiredEmotion);
    }
    public void UpdateDesiredEmotionIcon(EmotionType emotionType)
    {
        UpdateEmotionIcon(conversationManager.desiredEmotion, desiredIconSize, desiredEmotionSpriteRenderer);
    }

    public void FindAndUpdateButtonEmotionIcon(Transform buttonTranform, EmotionType emotionType)
    {
        var spriteRenderer = FindEmotionIconSpriteRendererByName(buttonEmotionIconName, buttonTranform);

        UpdateEmotionIcon(emotionType, buttonIconSize, spriteRenderer);
    }
    public SpriteRenderer FindEmotionIconSpriteRendererByName(string iconObjectName,Transform parentTransform)
    {
        Transform iconTransform = Utils.RecursiveFindChild(parentTransform, iconObjectName);

        if (iconTransform != null)
        {
            return FindSpriteRendererComponentInObject(iconObjectName, iconTransform);
        }
        else
        {
            Debug.LogError($"Target object with name '{iconObjectName}' not found!");
            return null;
        }
    }
    private void UpdateEmotionIcon(EmotionType desiredEmotion, float iconSize, SpriteRenderer emotionSpriteRenderer)
    {
        Sprite emotionSprite = LoadSprite($"{desiredEmotion}Icon");

        float originalWidth = emotionSprite.bounds.size.x;
        float originalHeight = emotionSprite.bounds.size.y;

        float scaleFactorX = originalWidth != 0 ? iconSize / originalWidth : .1f;
        float scaleFactorY = originalHeight != 0 ? iconSize / originalHeight : .1f;

        emotionSpriteRenderer.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1f);
        emotionSpriteRenderer.sprite = emotionSprite;
    }
    private SpriteRenderer FindSpriteRendererComponentInObject(string iconObjectName, Transform transform)
    {
        var spriteRenderer = transform.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer component not found on the object with name '{iconObjectName}'");
        }
        return spriteRenderer;
    }
    private Sprite LoadSprite(string spriteName)
    {
        // Load the sprite dynamically from the Resources folder
        return Resources.Load<Sprite>(ResourcesFolder + spriteName);
    }
}