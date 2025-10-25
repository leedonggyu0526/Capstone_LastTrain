using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// 상점 내 '카드 판매' 패널 UI를 관리하고, 카드 판매(자원 환불) 기능을 처리합니다.
/// </summary>
public class SellPanelUI : MonoBehaviour
{
    public static SellPanelUI Instance { get; private set; }

    // CardDeckUI와 동일한 구조 사용을 가정합니다.
    public GameObject cardSlotPrefab;     // 카드 슬롯 프리팹
    public Transform cardGrid;            // 카드를 배치할 Grid Layout
    public GameObject sellPanel;          // 판매 전체 패널

    // 판매 가격 배율 (예: 구매 가격의 50% 환불)
    [Range(0.1f, 1.0f)]
    public float sellRatio = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // 🚨 덱 변경 이벤트 구독: 카드가 사용되거나 판매되어 덱 수량이 변할 때 UI 갱신
        if (CardDeck.Instance != null)
        {
            CardDeck.OnCardUsed += HandleDeckChange;
        }
        // ResourceManager 이벤트가 없으므로 자원 UI 갱신은 별도의 HUD 스크립트에서 처리해야 합니다.
    }

    void OnEnable()
    {
        // 패널이 활성화될 때 UI 갱신
        RefreshSellUI();
    }

    private void OnDestroy()
    {
        // 구독 해제 (메모리 누수 방지)
        if (CardDeck.Instance != null)
        {
            CardDeck.OnCardUsed -= HandleDeckChange;
        }
    }

    /// <summary>덱 변경 이벤트 핸들러</summary>
    private void HandleDeckChange(string cardID)
    {
        // 카드가 판매/사용되어 덱이 변경되면 전체 판매 목록을 다시 그립니다.
        RefreshSellUI();
        // 🚨 자원 표시 UI가 별도로 있다면 해당 UI도 갱신되어야 합니다.
    }

    /// <summary>판매 창 열기</summary>
    public void OpenSellPanel()
    {
        sellPanel.SetActive(true);
        RefreshSellUI();
    }
    /// <summary>판매 창 닫기</summary>
    public void CloseSellPanel()
    {
        sellPanel.SetActive(false);
    }

    /// <summary>현재 보유 카드를 UI로 갱신하여 판매 목록을 진열</summary>
    public void RefreshSellUI()
    {
        if (CardDeck.Instance == null || CardDatabase.Instance == null) return;

        // 1. 기존 슬롯 제거
        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        // 2. 보유 카드 순회 후 슬롯 생성
        foreach (var kv in CardDeck.Instance.GetAllOwned())
        {
            string cardID = kv.Key;
            int count = kv.Value;

            if (count <= 0) continue;

            CardData data = CardDatabase.Instance.Get(cardID);
            if (data == null) continue;

            // 3. 슬롯 생성
            GameObject slot = Instantiate(cardSlotPrefab, cardGrid);

            // ... CardDisplay 설정 생략 (CardDeckUI와 동일한 로직) ...
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

            // QuantityText에 현재 보유 수량 및 판매 가격 표시
            // 🚨 주의: CardSlotPrefab의 UI 구조에 따라 텍스트 컴포넌트 이름을 확인하여 사용하세요.
            TextMeshProUGUI quantityText = slot.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
            if (quantityText != null)
            {
                quantityText.gameObject.SetActive(true);

                // 판매 가격 계산
                int fuelRefund = Mathf.FloorToInt(data.Fuel_price * sellRatio);
                int foodRefund = Mathf.FloorToInt(data.Food_price * sellRatio);
                int machinePartRefund = Mathf.FloorToInt(data.MachinePart_price * sellRatio);

                // 수량과 판매 가격을 함께 표시
                string priceDisplay = $"{fuelRefund}Food\n {foodRefund}Food\n {machinePartRefund}Parts";
                quantityText.text = $"x{count}\n{priceDisplay}";
            }

            // 4. 버튼 컴포넌트 연결 (판매 함수)
            Button sellButton = slot.GetComponent<Button>();
            if (sellButton != null)
            {
                sellButton.onClick.RemoveAllListeners();
                // 🚨 SellCard 메서드에 현재 카드 데이터(data)를 인자로 동적 연결
                sellButton.onClick.AddListener(() => SellCard(data));
            }
        }
    }

    /// <summary>
    /// 카드 판매 처리: 덱에서 수량 감소, 자원 환불
    /// </summary>
    public void SellCard(CardData cardData)
    {
        if (cardData == null || CardDeck.Instance == null || ResourceManager.Instance == null)
            return;

        // 1. 판매 가격 계산 및 환불 자원 확보
        int fuelRefund = Mathf.FloorToInt(cardData.Fuel_price * sellRatio);
        int foodRefund = Mathf.FloorToInt(cardData.Food_price * sellRatio);
        int machinePartRefund = Mathf.FloorToInt(cardData.MachinePart_price * sellRatio);

        // 2. 덱에서 카드 제거 (CardDeck.Remove 사용)
        bool removed = CardDeck.Instance.Remove(cardData.cardID, 1);

        if (removed)
        {
            // 3. 자원 환불 (ResourceManager.AddResource 사용)
            // ResourceManager.cs에 이미 존재하는 AddResource 함수를 사용합니다.
            ResourceManager.Instance.AddResource(ResourceType.Fuel, fuelRefund);
            ResourceManager.Instance.AddResource(ResourceType.Food, foodRefund);
            ResourceManager.Instance.AddResource(ResourceType.MachinePart, machinePartRefund);

            Debug.Log($"[SellPanelUI] {cardData.cardName} 1개 판매 완료. 환불: F{fuelRefund}, D{foodRefund}, P{machinePartRefund}");

            // UI 갱신은 CardDeck.OnCardUsed 이벤트 구독을 통해 자동으로 이루어집니다.
        }
    }
}