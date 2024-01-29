using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInformationsDisplay : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI durationText;

    private void Awake()
    {
        if (iconImage == null)
        {
            iconImage = Utils.RecursiveGetComponentByObjectName<Image>("CardEffectIcon", gameObject.transform);
        }
        if (durationText == null)
        {
            durationText = Utils.RecursiveGetComponentByObjectName<TextMeshProUGUI>("CardEffectDuration", gameObject.transform);
        }
    }

    // Set the icon sprite
    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    // Set the duration text
    public void SetDurationText(int duration)
    {
        if (duration == 0)
        {
            durationText.text = "";
        } else
        {
            durationText.text = duration.ToString();
        }
    }
}
