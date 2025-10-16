using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI; // Button 사용을 위해 추가

/// <summary>
/// 상점 UI를 관리하고 랜덤 카드를 고정 슬롯에 진열하는 관리자.
/// </summary>
public class ShopManager : MonoBehaviour
{
    public CardDatabase database;
    // public CardDeck playerDeck;         // ⬅️ 제거 (싱글턴으로 대체)
    // public CardDeckUI cardDeckUI;       // ⬅️ 제거 (싱글턴으로 대체)
    // public ResourceManager resourceManager; // [PLACEHOLDER] 자원 관리 시스템

    public GameObject cardSlotPrefab;     // 연결할 CardSlotPrefab (Root에 Button이 있어야 함)

    // 6개의 고정 슬롯을 연결할 목록 (유니티 인스펙터에서 수동 연결)
    public List<Transform> shopSlots;

    // 상점에 진열할 카드의 개수를 유연하게 제어 
    public int cardCount = 6;

    // 상점에 현재 진열된 카드 데이터 목록
    private List<CardData> currentShopCards = new List<CardData>();

    void Awake()
    {
        // CardDatabase 인스턴스 자동 연결 시도 (싱글턴)
        if (database == null)
        {
            database = CardDatabase.Instance;
        }

        RefreshShopUI();
    }

    /// <summary>
    /// 상점 UI를 갱신하고 랜덤 카드를 진열합니다.
    /// </summary>
    public void RefreshShopUI()
    {
        if (database == null) return;
        currentShopCards.Clear();

        // 1. 기존 슬롯 제거
        foreach (Transform slot in shopSlots)
        {
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }
        }

        // 2. 고정된 슬롯 개수만큼만 카드 진열
        int count = Mathf.Min(cardCount, shopSlots.Count);
        for (int i = 0; i < count; i++)
        {
            Transform slot = shopSlots[i];

            // 1. 무작위 카드 데이터 조회
            CardData data = database.GetRandom();
            if (data == null) continue;
            currentShopCards.Add(data);

            // 2. 카드 슬롯 프리팹 생성 (고정 슬롯의 자식으로)
            GameObject slotInstance = Instantiate(cardSlotPrefab, slot);

            // 프리팹의 크기 조절
            RectTransform rt = slotInstance.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                rt.localScale = Vector3.one;
            }


            // 3. CardDisplay에 데이터 할당
            Transform cardTransform = slotInstance.transform.Find("Card");
            if (cardTransform != null)
            {
                CardDisplay display = cardTransform.GetComponent<CardDisplay>();
                if (display != null)
                {
                    display.cardData = data;
                    display.RefreshUI();
                }
            }

            // 4. QuantityText(PriceText)에 가격 정보 표시
            TextMeshProUGUI priceText = slotInstance.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();

            if (priceText != null)
            {
                priceText.gameObject.SetActive(true);
                // ⬅️ 수정된 가격 표시: "GF"
                priceText.text = $"{data.price} GF";
            }

            // 5. 버튼 컴포넌트 연결 (Root인 slotInstance에서 Button을 찾습니다)
            // CardSlotPrefab 전체를 눌러 구매하는 로직 반영
            Button buyButton = slotInstance.GetComponent<Button>();
            if (buyButton != null)
            {
                // BuyCard 메서드에 현재 카드 데이터(data)를 인자로 동적 연결
                buyButton.onClick.AddListener(() => BuyCard(data));
            }
        }
    }

    /// <summary>
    /// 카드 구매 처리: 비용 지불, 카드 덱에 추가, UI 갱신
    /// </summary>
    /// <param name="cardData">구매할 카드의 데이터</param>
    public void BuyCard(CardData cardData)
    {
        if (cardData == null) return;

        // 1. [Placeholder] 비용 지불 로직
        int cost = cardData.price;
        // bool canAfford = resourceManager != null ? resourceManager.CanAfford(cost) : true;
        bool canAfford = true; // 자원 시스템 구현 전까지는 항상 구매 가능 (임시)

        if (!canAfford)
        {
            Debug.LogWarning($"[ShopManager] 자원 부족: {cardData.cardName} 구매 실패 (필요: {cost} GF)");
            return;
        }

        // 2. 카드 덱에 추가 (싱글턴 인스턴스 사용)
        CardDeck playerDeckInstance = CardDeck.Instance;
        if (playerDeckInstance != null)
        {
            playerDeckInstance.Add(cardData.cardID, 1); // ⬅️ Instance를 통해 접근
            Debug.Log($"[ShopManager] {cardData.cardName} 구매 성공. 덱에 추가됨. (비용: {cost} GF)");
        }
        else
        {
            Debug.LogError("[ShopManager] CardDeck.Instance를 찾을 수 없습니다. (CardDeck 오브젝트에 DontDestroyOnLoad 확인 필요)");
            return;
        }

        // 3. UI 갱신: 상점 UI 갱신, 덱 패널 UI 갱신
        RefreshShopUI();

        CardDeckUI deckUIInstance = CardDeckUI.Instance;
        if (deckUIInstance != null)
        {
            deckUIInstance.RefreshDeckUI(); // ⬅️ Instance를 통해 접근
        }
        else
        {
            Debug.LogWarning("[ShopManager] CardDeckUI.Instance를 찾을 수 없습니다. (덱 패널 씬이 로드되지 않았을 수 있음)");
        }
    }
}