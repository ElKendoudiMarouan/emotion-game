using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConversationManager))]
public class EmotionSpriteDisplayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer desiredEmotionIcon;
    private ConversationManager conversationManager;
    [SerializeField] private float desiredSize = 1.2f;

    // Name of the folder inside  Resources base folder where emotion icons folder is located
    private const string ResourcesFolder = "EmotionSprites/";

    public void Start()
    {
        conversationManager = GetComponent<ConversationManager>();

        FindDesiredEmotionIconSpriteRenderer();
        UpdateIconsBasedOnEmotion(conversationManager.desiredEmotion);
    }

    private void FindDesiredEmotionIconSpriteRenderer()
    {
        if (desiredEmotionIcon == null)
        {
            GameObject targetObject = GameObject.Find("DesiredEmotionIcon");
            if (targetObject != null)
            {
                desiredEmotionIcon = targetObject.GetComponent<SpriteRenderer>();
                if (desiredEmotionIcon == null)
                {
                    Debug.LogError("SpriteRenderer component not found on the target object!");
                }
            }
            else
            {
                Debug.LogError("Target object with tag 'YourTag' not found!");
            }
        }
    } 

    private void UpdateDesiredEmotionIcon(EmotionType desiredEmotion)
    {
        Sprite emotionSprite = LoadSprite($"{desiredEmotion}Icon");
        // Set the sprite on the conversationManager component
        float originalWidth = emotionSprite.bounds.size.x;
        float originalHeight = emotionSprite.bounds.size.y;

        desiredEmotionIcon.sprite = emotionSprite;


        float scaleFactorX = originalWidth != 0 ? desiredSize / originalWidth : .1f;
        float scaleFactorY = originalHeight != 0 ? desiredSize / originalHeight : .1f;


        desiredEmotionIcon.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1f);

    }

    private Sprite LoadSprite(string spriteName)
    {
        // Load the sprite dynamically from the Resources folder
        return Resources.Load<Sprite>(ResourcesFolder + spriteName);
    }

    // Call this function whenever the desiredEmotion changes
    public void UpdateIconsBasedOnEmotion(EmotionType desiredEmotion)
    {
        UpdateDesiredEmotionIcon(desiredEmotion);
    }
}