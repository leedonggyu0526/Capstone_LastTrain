using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 인벤토리/덱 UI를 관리합니다.
/// - CardDeck에서 보유 카드 목록과 수량을 가져옵니다.
/// - CardDatabase로 데이터를 조회해 CardDisplay가 붙은 완성된 카드 프리팹을 배치합니다.
/// </summary>
public class CardDeckUI : MonoBehaviour
{
    public CardDeck playerDeck;           // 보유 카드 관리
    public GameObject cardSlotPrefab;     // [슬롯 프리팹]을 연결해야 합니다. (자식에 Card, QuantityText 포함)
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
        // 1. 기존 슬롯 제거
        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        // 2. 보유 카드 순회 후 슬롯 생성 (CardDeck에서 삽입 순서를 보장하는 메서드 사용 가정)
        // CardDeck.cs에 GetOwnedInInsertionOrder()가 구현되어 있어야 합니다.
        foreach (var kv in playerDeck.GetOwnedInInsertionOrder())
        {
            string cardID = kv.Key;
            int count = kv.Value;

            // 3. 카드 데이터 조회
            CardData data = CardDatabase.Instance.Get(cardID);
            if (data == null) continue; // 데이터베이스 로드 실패 시 건너뜀

            // 4. Root 슬롯 프리팹 생성 (CardSlotPrefab)
            GameObject slotInstance = Instantiate(cardSlotPrefab, cardGrid);

            // 5. Card 오브젝트를 찾습니다. (Slot의 자식에 붙어있는 Card 프리팹)
            Transform cardTransform = slotInstance.transform.Find("Card");

            if (cardTransform == null) continue;

            // 6. CardDisplay를 찾아 데이터 설정 및 UI 갱신 (Card 오브젝트에 붙어 있음)
            CardDisplay display = cardTransform.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.cardData = data;
                display.RefreshUI(); // 배경, 제목, 설명 등 모든 데이터 채우기
            }

            // 7. QuantityText를 찾습니다. (Slot의 자식에 붙어있는 텍스트 오브젝트)
            // QuantityText가 Slot의 직계 자식이라고 가정합니다.
            TextMeshProUGUI quantityText = slotInstance.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();

            if (quantityText != null)
            {
                // 찾은 Text 오브젝트를 활성화
                quantityText.gameObject.SetActive(true);

                // 텍스트 설정
                quantityText.text = $"x{count}";
            }
        }
    }
}