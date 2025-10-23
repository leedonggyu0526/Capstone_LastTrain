using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 상점 UI를 관리하고 랜덤 카드를 고정 슬롯에 진열하는 관리자.
/// </summary>
public class ShopManager : MonoBehaviour
{
    public CardDatabase database;
    public GameObject cardSlotPrefab;     // CardSlotPrefab (Root에 Button이 있어야 함)
    public List<Transform> shopSlots;
    public int cardCount = 6;
    private List<CardData> currentShopCards = new List<CardData>();

    void Awake()
    {
        // CardDatabase 인스턴스 자동 연결 시도
        if (database == null)
        {
            database = CardDatabase.Instance;
        }
    }

    void Start()
    {
        // Start()에서 RefreshShopUI를 호출하여 초기 진열 시작
        RefreshShopUI();
    }

    /// <summary>상점 UI를 갱신하고 랜덤 카드를 진열합니다.</summary>
    public void RefreshShopUI()
    {
        if (database == null) return;
        currentShopCards.Clear();

        // 1. 기존 슬롯 제거 로직 (생략)
        foreach (Transform slot in shopSlots)
        {
            foreach (Transform child in slot) Destroy(child.gameObject);
        }

        // 2. 고정된 슬롯 개수만큼 카드 진열 (생략)
        int count = Mathf.Min(cardCount, shopSlots.Count);
        for (int i = 0; i < count; i++)
        {
            Transform slot = shopSlots[i];
            CardData data = database.GetRandom();
            if (data == null) continue;
            currentShopCards.Add(data);

            GameObject slotInstance = Instantiate(cardSlotPrefab, slot);

            // 3. UI 및 데이터 할당 (CardDisplay, PriceText 로직 생략)
            // ... (CardDisplay, PriceText 할당 로직) ...
            TextMeshProUGUI priceText = slotInstance.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
            if (priceText != null)
            {
                priceText.gameObject.SetActive(true);
                string costString = $"연료:{data.fuelCost} / 식량:{data.foodCost} / 부품:{data.partsCost}";
                priceText.text = costString;
            }

            // 4. 버튼 컴포넌트 연결
            Button buyButton = slotInstance.GetComponent<Button>();
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(() => BuyCard(data));
            }
        }
    }

    /// <summary>카드 구매 처리: 비용 지불, 카드 덱에 추가, UI 갱신</summary>
    public void BuyCard(CardData cardData)
    {
        if (cardData == null) return;

        // 1. [Placeholder] 비용 지불 로직 (생략)
        bool canAfford = true;
        if (!canAfford) { /* ... warning log ... */ return; }

        // 2. 카드 덱에 추가 (싱글턴 인스턴스 안전 접근)
        CardDeck playerDeckInstance = CardDeck.Instance;

        // ⬇️ 🚨🚨🚨 싱글턴 복구 로직 (오류 최종 해결책) 🚨🚨🚨
        if (playerDeckInstance == null)
        {
            playerDeckInstance = FindObjectOfType<CardDeck>();
            if (playerDeckInstance != null)
            {
                CardDeck.RegisterInstance(playerDeckInstance);
                playerDeckInstance = CardDeck.Instance; // 등록된 인스턴스로 업데이트
                Debug.LogWarning("[ShopManager] CardDeck.Instance를 FindObjectOfType으로 찾아 등록했습니다.");
            }
        }
        // ⬆️🚨🚨🚨 복구 로직 끝 🚨🚨🚨

        if (playerDeckInstance != null)
        {
            playerDeckInstance.Add(cardData.cardID, 1);
            Debug.Log($"[ShopManager] {cardData.cardName} 구매 성공. 덱에 추가됨.");
        }
        else
        {
            // ⬅️ 이 로그가 콘솔에 나타나는 경우, 싱글턴 복구까지 실패한 것입니다.
            Debug.LogError("[ShopManager] CardDeck.Instance를 찾을 수 없습니다. (싱글턴 설정/실행 순서 문제)");
            return;
        }

        // 3. UI 갱신: 상점 UI 갱신, 덱 패널 UI 갱신
        RefreshShopUI();
        CardDeckUI deckUIInstance = CardDeckUI.Instance;
        if (deckUIInstance != null)
        {
            deckUIInstance.RefreshDeckUI();
        }
    }
}