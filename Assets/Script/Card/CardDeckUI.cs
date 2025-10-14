using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 인벤토리/덱 UI를 관리.
/// </summary>
public class CardDeckUI : MonoBehaviour
{
    public static CardDeckUI Instance { get; private set; } // ⬅️ 싱글턴 인스턴스

    // public CardDeck playerDeck;         // ⬅️ 제거: 이제 Instance를 통해 접근
    public GameObject cardSlotPrefab;     // 카드 슬롯 프리팹
    public Transform cardGrid;            // 카드들을 배치할 Grid Layout
    public GameObject deckPanel;          // 전체 패널 (활성/비활성 제어)

    void Awake()
    {
        // 싱글턴 구현 (UI는 보통 씬과 함께 파괴되지만, ShopManager가 찾을 수 있도록 함)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
        if (CardDeck.Instance == null) return; // 덱 데이터가 로드되지 않았으면 무시

        // 기존 슬롯 제거
        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        // 보유 카드 순회 후 슬롯 생성
        foreach (var kv in CardDeck.Instance.GetAllOwned()) // ⬅️ Instance를 통해 접근
        {
            string cardID = kv.Key;
            int count = kv.Value;

            // 카드 데이터 조회
            CardData data = CardDatabase.Instance.Get(cardID);
            if (data == null) continue;

            // 슬롯 생성 및 UI 세팅
            GameObject slot = Instantiate(cardSlotPrefab, cardGrid);

            // CardDisplay와 QuantityText를 찾아 데이터 설정
            Transform cardTransform = slot.transform.Find("Card");
            if (cardTransform != null)
            {
                CardDisplay display = cardTransform.GetComponent<CardDisplay>();
                if (display != null)
                {
                    display.cardData = data;
                    display.RefreshUI();
                }
            }

            TextMeshProUGUI quantityText = slot.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
            if (quantityText != null)
            {
                quantityText.gameObject.SetActive(true);
                quantityText.text = $"x{count}";
            }
        }
    }
}