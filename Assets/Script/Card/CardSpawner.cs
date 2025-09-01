using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드 프리팹을 관리하고, 덱/DB에서 랜덤 카드를 뽑아 손패 슬롯(Card1~4)에 세팅하는 스크립트.
/// 기존에는 CSV에서 직접 생성했지만, 지금은 CardDatabase + CardDeck 구조를 이용한다.
/// </summary>
public class CardSpawner : MonoBehaviour
{
    [Header("필수 참조")]
    public CardDatabase database;    // 카드 데이터베이스 (CSV 로드 담당)
    public CardDeck playerDeck;      // 플레이어 카드 덱 (보유 상태 관리)

    [Header("손패 슬롯(Card1~4의 CardDisplay)")]
    public List<CardDisplay> handSlots; // 카드가 표시될 UI 슬롯들 (Card1~4 오브젝트의 CardDisplay)

    void Start()
    {
        // Start에서 바로 FillHandAtStart()를 호출하면
        // CardDeck.Start()에서 시드 추가가 끝나기 전에 실행될 수 있다.
        // 따라서 코루틴으로 한 프레임 대기 후 실행 → 안전하게 덱 초기화 완료 후 동작.
        StartCoroutine(FillHandNextFrame());
    }

    /// <summary>
    /// 한 프레임 대기 후 손패 채우기 (덱 초기화가 끝난 뒤 실행 보장)
    /// </summary>
    private IEnumerator FillHandNextFrame()
    {
        yield return null; // 한 프레임 대기

        // 디버그: 내가 물고 있는 참조가 맞는지와 현재 덱 요약 출력
        if (playerDeck == null) Debug.LogError("[CardSpawner] playerDeck == null");
        else Debug.Log($"[CardSpawner] Deck summary before fill: {playerDeck.GetSummary()} (this={playerDeck.name})");

        FillHandAtStart();
    }

    /// <summary>
    /// 게임 시작 시 손패 슬롯(Card1~4)을 랜덤 카드로 채운다.
    /// </summary>
    public void FillHandAtStart()
    {
        if (database == null) database = CardDatabase.Instance;

        for (int i = 0; i < handSlots.Count; i++)
        {
            var slot = handSlots[i];
            if (slot == null) continue;

            // 덱에서 무작위 카드 ID 뽑기
            string id = playerDeck?.GetRandomCardID();
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[CardSpawner] 덱이 비어 있어 {i}번 슬롯을 비움");
                slot.cardData = null;
                slot.RefreshUI();
                continue;
            }

            // DB에서 카드 데이터 가져오기
            var data = database?.Get(id);
            if (data == null)
            {
                Debug.LogWarning($"[CardSpawner] DB에서 {id} 못 찾음 → 슬롯 {i} 비움");
                slot.cardData = null;
                slot.RefreshUI();
                continue;
            }

            // 카드 데이터 세팅
            slot.cardData = data;
            slot.RefreshUI();

            // 드래그용 cardID 세팅
            var drag = slot.GetComponent<CardDrag>();
            if (drag != null) drag.cardID = data.cardID;

            Debug.Log($"[CardSpawner] Slot{i + 1} ← {data.cardName} 세팅");
        }
    }
}
