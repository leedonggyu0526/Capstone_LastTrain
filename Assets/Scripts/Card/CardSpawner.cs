using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardSpawner : MonoBehaviour
{
    [Header("필수 참조")]
    public CardDatabase database;
    public CardDeck playerDeck;

    [Header("손패 슬롯(Card1~4)")]
    public List<CardDisplay> handSlots;

    void Start()
    {
        StartCoroutine(FillHandNextFrame());
    }

    private IEnumerator FillHandNextFrame()
    {
        yield return null;
        ClearHandSlots();
        FillHandAtStart();
    }

    private void OnEnable()
    {
        StartCoroutine(EnableHandNextFrame());
        if (playerDeck != null)
        {
            CardDeck.OnCardUsed += HandleCardUsed;
        }
    }

    private void OnDisable()
    {
        if (playerDeck != null)
        {
            CardDeck.OnCardUsed -= HandleCardUsed;
        }
    }

    private IEnumerator EnableHandNextFrame()
    {
        yield return null;
        ClearHandSlots();
        FillHandAtStart();
    }

    private void ClearHandSlots()
    {
        for (int j = 0; j < handSlots.Count; j++)
        {
            CardDisplay cardDisplay = handSlots[j];
            if (cardDisplay != null)
            {
                cardDisplay.cardData = null;
                cardDisplay.RefreshUI();
                var drag = cardDisplay.GetComponent<CardDrag>();
                if (drag != null) drag.cardID = null;
            }
        }
    }

    // 초기 손패 생성 (수량 비례 + 중복 방지)
    public void FillHandAtStart()
    {
        if (database == null) database = CardDatabase.Instance;
        if (playerDeck == null) playerDeck = CardDeck.Instance;

        List<string> availableCardIds = new List<string>();
        foreach (var kv in playerDeck.GetAllOwned())
        {
            for (int i = 0; i < kv.Value; i++)
                availableCardIds.Add(kv.Key);
        }

        int cardsToDraw = Mathf.Min(availableCardIds.Count, handSlots.Count);

        for (int i = 0; i < handSlots.Count; i++)
        {
            var slot = handSlots[i];
            if (slot == null) continue;

            if (i < cardsToDraw)
            {
                if (availableCardIds.Count == 0) break;

                int randIndex = UnityEngine.Random.Range(0, availableCardIds.Count);
                string id = availableCardIds[randIndex];
                availableCardIds.RemoveAt(randIndex); // 🚨 중복 방지

                var data = database?.Get(id);
                if (data == null) continue;

                slot.cardData = data;
                slot.RefreshUI();

                var drag = slot.GetComponent<CardDrag>();
                if (drag != null)
                {
                    drag.cardID = data.cardID;
                    // 초기 손패 배치 후 원위치를 현재 위치로 설정합니다.
                    drag.UpdateOriginalPosition();
                }
            }
            else
            {
                slot.cardData = null;
                slot.RefreshUI();
            }
        }
    }

    /// <summary>
    /// [새로 추가된 메서드]
    /// 리롤 버튼 클릭 시 호출되어 현재 손패를 완전히 지우고, 
    /// 덱에 있는 카드 중 무작위로 새로운 손패를 채웁니다.
    /// </summary>
    public void RerollHand()
    {
        // 1. 현재 손패 슬롯의 데이터와 UI를 모두 초기화합니다.
        ClearHandSlots();

        // 2. 새로운 카드로 손패를 다시 채웁니다.
        FillHandAtStart();

        Debug.Log("[CardSpawner] 손패가 리롤되었습니다. 새로운 카드들이 배치됩니다.");
    }


    // 카드 사용 시 호출 (손패 압축 로직)
    private void HandleCardUsed(string usedCardID)
    {
        int usedSlotIndex = -1;
        for (int j = 0; j < handSlots.Count; j++)
        {
            CardDisplay slot = handSlots[j];
            if (slot != null && slot.cardData != null && slot.cardData.cardID == usedCardID)
            {
                usedSlotIndex = j;
                break;
            }
        }

        if (usedSlotIndex != -1)
        {
            // 2. 사용된 슬롯부터 오른쪽 끝까지 카드 데이터 이동 및 스냅
            for (int i = usedSlotIndex; i < handSlots.Count - 1; i++)
            {
                CardDisplay currentSlot = handSlots[i];
                CardDisplay nextSlot = handSlots[i + 1];

                var currentDrag = currentSlot.GetComponent<CardDrag>();
                var nextDrag = nextSlot.GetComponent<CardDrag>();

                if (currentDrag != null && nextDrag != null)
                {
                    // 데이터 복사 (오른쪽 카드를 왼쪽으로 이동)
                    currentSlot.cardData = nextSlot.cardData;
                    currentSlot.RefreshUI();

                    // CardDrag ID 복사
                    currentDrag.cardID = nextDrag.cardID;

                    // 스냅을 통해 카드 위치를 새 슬롯으로 강제 이동
                    currentDrag.ResetToOriginalPositionInstant();

                    // 🚨 스냅된 위치를 새로운 '원위치'로 저장합니다. (가장 중요)
                    currentDrag.UpdateOriginalPosition();
                }
            }

            // 3. 맨 끝 슬롯(빈 공간)의 데이터를 지우고 스냅/원위치 저장
            CardDisplay lastSlot = handSlots[handSlots.Count - 1];
            lastSlot.cardData = null;
            lastSlot.RefreshUI();

            var lastDrag = lastSlot.GetComponent<CardDrag>();
            if (lastDrag != null)
            {
                lastDrag.cardID = null;
                lastDrag.ResetToOriginalPositionInstant();
                lastDrag.UpdateOriginalPosition();
            }
        }
    }
}