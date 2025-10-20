using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;

    public Image backgroundImage;    // 카드 배경 이미지
    public Image artworkImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public void RefreshUI()
    {
        if (cardData != null)
        {
            titleText.text = cardData.cardName;
            descriptionText.text = cardData.description;
            artworkImage.sprite = cardData.artwork;
            backgroundImage.sprite = cardData.background;
        }
    }

}
