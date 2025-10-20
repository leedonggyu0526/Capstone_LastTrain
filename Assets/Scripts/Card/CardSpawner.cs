using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드 프리팹을 관리하고, 덱/DB에서 랜덤 카드를 뽑아 손패 슬롯(Card1~4)에 세팅하는 스크립트.
/// </summary>
public class CardSpawner : MonoBehaviour
{
    [Header("필수 참조")]
    public CardDatabase database;       // 카드 데이터베이스 (CSV 로드 담당)
    public CardDeck playerDeck;         // 플레이어 카드 덱 (보유 상태 관리)

    [Header("손패 슬롯(Card1~4)")]
    public List<CardDisplay> handSlots; // 손패 카드 슬롯 (CardDisplay 참조)

    void Start()
    {
        // 덱 초기화가 완료된 뒤 손패 세팅
        StartCoroutine(FillHandNextFrame());
    }

    /// <summary>
    /// 한 프레임 대기 후 손패 채우기 (덱 초기화 보장)
    /// </summary>
    private IEnumerator FillHandNextFrame()
    {
        yield return null;
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
                slot.cardData = null;
                slot.RefreshUI();
                continue;
            }

            // DB에서 카드 데이터 가져오기
            var data = database?.Get(id);
            if (data == null)
            {
                slot.cardData = null;
                slot.RefreshUI();
                continue;
            }

            // 카드 UI에 데이터 세팅
            slot.cardData = data;
            slot.RefreshUI();

            // 드래그용 cardID 세팅
            var drag = slot.GetComponent<CardDrag>();
            if (drag != null) drag.cardID = data.cardID;
        }
    }
}
