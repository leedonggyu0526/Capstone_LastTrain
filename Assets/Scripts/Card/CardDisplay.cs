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

    private void Awake()
    {
        
    }

    public void RefreshUI()
    {
        //if (cardData != null)
        //{
        //    titleText.text = cardData.cardName;
        //    descriptionText.text = cardData.description;
        //    artworkImage.sprite = cardData.artwork;
        //    backgroundImage.sprite = cardData.background;
        //}
        if (cardData != null)
        {
            // 1. 데이터가 있으면 보이게 하고 UI를 채웁니다.
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true); // 혹시 비활성화 상태였다면 활성화
            }

            titleText.text = cardData.cardName;
            descriptionText.text = cardData.description;
            artworkImage.sprite = cardData.artwork;
            backgroundImage.sprite = cardData.background;
        }
        else
        {
            // 2. ⬅️ [핵심 수정] CardData가 null이면 카드를 숨기고 UI를 초기화합니다.
            gameObject.SetActive(false);

            titleText.text = "";
            descriptionText.text = "";
            artworkImage.sprite = null;      // 스프라이트 제거
            backgroundImage.sprite = null;   // 배경 스프라이트 제거
        }
    }

}
