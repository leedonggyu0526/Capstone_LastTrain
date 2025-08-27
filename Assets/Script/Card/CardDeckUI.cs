using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 인벤토리/덱 팝업 UI.
/// CardDeck(보유 목록)에서 cardID와 수량을 받아와
/// CardDatabase를 통해 상세 데이터를 조회하고
/// 슬롯 프리팹을 생성하여 UI로 표시한다.
/// </summary>
public class CardDeckUI : MonoBehaviour
{
    public CardDeck playerDeck;           // 보유 카드 관리
    public GameObject cardSlotPrefab;     // 카드 슬롯 프리팹 (CardImage + QuantityText 있어야 함)
    public Transform cardGrid;            // 카드들을 배치할 Grid Layout
    public GameObject deckPanel;          // 전체 패널 (활성/비활성)

    // 팝업 열기
    public void OpenDeck()
    {
        deckPanel.SetActive(true);
        RefreshDeckUI();
    }

    // 팝업 닫기
    public void CloseDeck()
    {
        deckPanel.SetActive(false);
    }

    // 덱 UI 갱신
    public void RefreshDeckUI()
    {
        // 기존 슬롯 제거
        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        // 보유 카드 순회
        foreach (var kv in playerDeck.GetAllOwned())
        {
            string cardID = kv.Key;
            int count = kv.Value;

            // 카드 데이터 조회
            CardData data = CardDatabase.Instance.Get(cardID);
            if (data == null) continue;

            // 슬롯 생성
            GameObject slot = Instantiate(cardSlotPrefab, cardGrid);

            // UI 갱신
            Image cardImage = slot.transform.Find("CardImage").GetComponent<Image>();
            TextMeshProUGUI quantityText = slot.transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();

            cardImage.sprite = data.artwork;
            quantityText.text = $"x{count}";
        }
    }
}
