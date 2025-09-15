using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDeckUI : MonoBehaviour
{
    public CardDeck playerDeck;           // 카드 덱 정보
    public GameObject cardSlotPrefab;     // 카드 슬롯 프리팹
    public Transform cardGrid;            // 카드들을 담을 Grid
    public GameObject deckPanel;          // 전체 패널 (켜고 끄는 용도)

    // 덱 열기
    public void OpenDeck()
    {
        deckPanel.SetActive(true);
        RefreshDeckUI();
    }

    // 덱 닫기
    public void CloseDeck()
    {
        deckPanel.SetActive(false);
    }

    // 카드 덱 UI 갱신
    public void RefreshDeckUI()
    {
        // 기존 카드 슬롯 다 제거
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }

        // 현재 보유 중인 카드들 슬롯 생성
        foreach (CardInstance ci in playerDeck.deck)
        {
            GameObject slot = Instantiate(cardSlotPrefab, cardGrid);

            Image cardImage = slot.transform.Find("CardImage").GetComponent<Image>();
            TextMeshProUGUI quantityText = slot.transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();

            cardImage.sprite = ci.cardData.artwork;
            quantityText.text = $"x{ci.quantity}";
        }
    }
}
