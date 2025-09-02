using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 인벤토리/덱 UI를 관리.
/// - 보유 카드 목록(CardDeck)에서 cardID와 수량을 가져옴
/// - CardDatabase로 카드 데이터를 조회해 UI 표시
/// - 슬롯 프리팹(CardImage + QuantityText)을 이용해 카드 배치
/// </summary>
public class CardDeckUI : MonoBehaviour
{
    public CardDeck playerDeck;           // 보유 카드 관리
    public GameObject cardSlotPrefab;     // 카드 슬롯 프리팹
    public Transform cardGrid;            // 카드들을 배치할 Grid Layout
    public GameObject deckPanel;          // 전체 패널 (활성/비활성 제어)

    /// <summary>덱 창 열기</summary>
    public void OpenDeck()
    {
        deckPanel.SetActive(true);
        RefreshDeckUI();
    }

    /// <summary>덱 창 닫기</summary>
    public void CloseDeck()
    {
        deckPanel.SetActive(false);
    }

    /// <summary>현재 덱 정보를 UI로 갱신</summary>
    public void RefreshDeckUI()
    {
        // 기존 슬롯 제거
        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        // 보유 카드 순회 후 슬롯 생성
        foreach (var kv in playerDeck.GetAllOwned())
        {
            string cardID = kv.Key;
            int count = kv.Value;

            // 카드 데이터 조회
            CardData data = CardDatabase.Instance.Get(cardID);
            if (data == null) continue;

            // 슬롯 생성 및 UI 세팅
            GameObject slot = Instantiate(cardSlotPrefab, cardGrid);
            Image cardImage = slot.transform.Find("CardImage").GetComponent<Image>();
            TextMeshProUGUI quantityText = slot.transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();

            cardImage.sprite = data.artwork;
            quantityText.text = $"x{count}";
        }
    }
}
