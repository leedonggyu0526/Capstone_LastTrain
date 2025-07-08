using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDeckUI : MonoBehaviour
{
    public CardDeck playerDeck;           // 플레이어 덱
    public GameObject cardSlotPrefab;     // 카드 슬롯 프리펩
    public Transform cardGrid;            // 카드 그리드
    public GameObject deckPanel;          // 덱 패널

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

    // 덱 UI 새로고침
    public void RefreshDeckUI()
    {
        // 플레이어 덱 초기화
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }

        // 플레이어 덱 카드 생성
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
