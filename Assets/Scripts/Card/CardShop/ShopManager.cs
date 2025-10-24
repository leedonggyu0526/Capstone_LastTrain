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

                // 🚨 핵심 수정: 3가지 자원 가격을 조합하여 표시합니다.
                string priceDisplay = "";

                // 1. 연료 가격 추가
                if (data.Fuel_price > 0)
                {
                    priceDisplay += $"{data.Fuel_price} Fuel";
                }

                // 2. 식량 가격 추가
                if (data.Food_price > 0)
                {
                    if (!string.IsNullOrEmpty(priceDisplay)) priceDisplay += "\n"; // 줄바꿈
                    priceDisplay += $"{data.Food_price} Food";
                }

                // 3. 부품 가격 추가
                if (data.MachinePart_price > 0)
                {
                    if (!string.IsNullOrEmpty(priceDisplay)) priceDisplay += "\n"; // 줄바꿈
                    priceDisplay += $"{data.MachinePart_price} Parts";
                }

                // 텍스트 필드에 적용
                priceText.text = priceDisplay;

                // 텍스트 필드 크기가 충분하지 않을 경우, 줄바꿈 대신 콤마를 사용할 수 있습니다.
                // (예시: priceDisplay = priceDisplay.Replace("\n", ", ");)
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
        // 필수 인스턴스 확인
        if (cardData == null || CardDeck.Instance == null || ResourceManager.Instance == null)
            return;

        // 1. 초기 자원 확인 (Check Phase)
        // GetResource를 사용하여 비용 지불 없이 충분한지 미리 확인합니다.

        // 필요한 비용
        int fuelCost = cardData.Fuel_price;
        int foodCost = cardData.Food_price;
        int machinePartCost = cardData.MachinePart_price;

        // 현재 보유 자원
        int currentFuel = ResourceManager.Instance.GetResource(ResourceType.Fuel);
        int currentFood = ResourceManager.Instance.GetResource(ResourceType.Food);
        int currentMachinePart = ResourceManager.Instance.GetResource(ResourceType.MachinePart);

        // 세 가지 비용 모두 지불 가능한지 확인
        bool canAffordFuel = currentFuel >= fuelCost;
        bool canAffordFood = currentFood >= foodCost;
        bool canAffordMachinePart = currentMachinePart >= machinePartCost;

        // 2. 구매 조건 검사: 세 자원 모두 지불 가능해야 구매 가능
        if (!canAffordFuel || !canAffordFood || !canAffordMachinePart)
        {
            // 🚨 자원 부족: 구매 실패
            Debug.LogWarning($"[ShopManager] 자원 부족으로 {cardData.cardName} 구매 실패. " +
                             $"(Fuel: {canAffordFuel}, Food: {canAffordFood}, MachinePart: {canAffordMachinePart})");
            return;
        }

        // --- 3. 비용 지불 (Spend Phase) ---
        // Check Phase를 통과했으므로 ConsumeResources를 호출하여 자원을 소비합니다.

        bool spentFuel = ResourceManager.Instance.ConsumeResources(ResourceType.Fuel, fuelCost);
        bool spentFood = ResourceManager.Instance.ConsumeResources(ResourceType.Food, foodCost);
        bool spentMachinePart = ResourceManager.Instance.ConsumeResources(ResourceType.MachinePart, machinePartCost);

        // 안전 확인: ConsumeResources가 false를 반환하면 (내부 로직 실패 시)
        if (!spentFuel || !spentFood || !spentMachinePart)
        {
            Debug.LogError($"[ShopManager] 구매 비용 지불 중 심각한 오류 발생! (롤백 로직 필요)");
            // 🚨 [주의] 이 시점에서 이미 소비된 자원(예: Fuel)을 되돌리는 롤백 로직이 필요할 수 있습니다.
            return;
        }

        // 4. 카드 덱에 추가 (성공)
        CardDeck.Instance.Add(cardData.cardID, 1);

        // 5. UI 갱신 (선택 사항: 덱 패널이 CardDeck.OnCardUsed 이벤트를 구독하고 있다면 필요 없음)
        // RefreshShopUI(); 
        // CardDeckUI.Instance.RefreshDeckUI();
    }
}