using System.Collections.Generic;
using System.Text; // GetSummary용
using UnityEngine;

/// <summary>
/// 플레이어 보유 카드 관리: cardID(string) ↔ 수량(int)
/// </summary>
public class CardDeck : MonoBehaviour
{
    // 보유 카드 저장소 (key = cardID, value = 수량)
    private Dictionary<string, int> deck = new Dictionary<string, int>();

    // 카드 ID가 덱에 "처음" 추가된 순서를 저장하는 리스트 (순서 보장)
    private List<string> insertionOrder = new List<string>();

    // ── 테스트용 시드(임시). 추후 상점/보상 연결 시 끄면 됨 ──
    [Header("Test Seed (임시) — 추후 상점 붙으면 끄세요")]
    public bool seedOnStart = true;
    public string[] seedIds = new[] { "1", "2", "3" }; // CSV의 cardID와 동일해야 함
    public int seedAmountEach = 3;

    void Start()
    {
        // 시작 시 테스트용으로 카드 자동 지급
        if (seedOnStart)
        {
            foreach (var id in seedIds)
                Add(id, seedAmountEach);
        }
    }

    /// <summary>보유 요약 문자열(디버그·UI 표시용)</summary>
    public string GetSummary()
    {
        if (deck.Count == 0) return "(empty)";
        StringBuilder sb = new StringBuilder();
        foreach (var kv in deck) sb.Append($"{kv.Key}:{kv.Value} ");
        return sb.ToString();
    }

    /// <summary>카드 추가</summary>
    public void Add(string cardID, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(cardID)) return;
        cardID = cardID.Trim();

        // Dictionary에 카드가 있는지 확인
        if (deck.ContainsKey(cardID))
        {
            deck[cardID] += amount;
        }
        else
        {
            // 신규 카드: 수량 설정
            deck[cardID] = amount;
            // 신규 카드의 경우에만 순서 리스트에 추가 (삽입 순서 보장)
            insertionOrder.Add(cardID);
        }
    }

    /// <summary>카드 제거(성공 시 true)</summary>
    public bool Remove(string cardID, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(cardID)) return false;
        cardID = cardID.Trim();

        if (!deck.ContainsKey(cardID)) return false;

        deck[cardID] -= amount;
        if (deck[cardID] <= 0) 
        {
            deck.Remove(cardID);
            //수량이 0 이하가 되면 순서 기록 리스트에서도 제거
            insertionOrder.Remove(cardID);
        }

        return true;
    }

    // CardDeckUI에서 사용할, 삽입 순서대로 (cardID, count)를 반환하는 메서드
    public IEnumerable<KeyValuePair<string, int>> GetOwnedInInsertionOrder()
    {
        foreach (string cardID in insertionOrder)
        {
            // 수량이 0보다 큰 카드만 반환
            if (deck.TryGetValue(cardID, out int count) && count > 0)
            {
                yield return new KeyValuePair<string, int>(cardID, count);
            }
        }
    }

    /// <summary>해당 카드ID의 보유 수량</summary>
    public int GetCount(string cardID)
    {
        if (string.IsNullOrWhiteSpace(cardID)) return 0;
        cardID = cardID.Trim();
        return deck.ContainsKey(cardID) ? deck[cardID] : 0;
    }

    /// <summary>전체 보유 카드 (cardID, count) 열거</summary>
    public IEnumerable<KeyValuePair<string, int>> GetAllOwned()
    {
        foreach (var kv in deck)
            yield return kv;
    }

    /// <summary>
    /// 보유 수량 비율에 따른 무작위 cardID 반환(없으면 null)
    /// </summary>
    public string GetRandomCardID()
    {
        List<string> expanded = new List<string>();
        foreach (var kv in deck)
        {
            for (int i = 0; i < kv.Value; i++)
                expanded.Add(kv.Key);
        }
        if (expanded.Count == 0) return null;
        return expanded[Random.Range(0, expanded.Count)];
    }

    // ───────────── 편의 메서드 ─────────────

    /// <summary>
    /// 무작위 cardID를 뽑고, CardDatabase에서 CardData까지 조회한다.
    /// 성공 시 true, 실패 시 false.
    /// </summary>
    public bool TryDrawRandomData(out CardData data, out string cardID)
    {
        data = null;
        cardID = GetRandomCardID();
        if (string.IsNullOrEmpty(cardID)) return false;

        if (CardDatabase.Instance == null) return false;

        data = CardDatabase.Instance.Get(cardID);
        return data != null;
    }

    /// <summary>
    /// 보유 목록을 (CardData, count)로 변환해 열거(UI 표시용).
    /// DB나 데이터가 없으면 해당 항목은 건너뜀.
    /// </summary>
    public IEnumerable<(CardData data, int count)> EnumerateOwnedAsData()
    {
        if (CardDatabase.Instance == null) yield break;

        foreach (var kv in deck)
        {
            var data = CardDatabase.Instance.Get(kv.Key);
            if (data != null)
                yield return (data, kv.Value);
        }
    }
}
