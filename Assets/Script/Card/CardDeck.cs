using System.Collections.Generic;
using System.Text;     // StringBuilder 사용
using UnityEngine;

/// <summary>
/// 플레이어가 보유한 카드들을 관리하는 클래스.
/// cardID(string)과 수량(int)을 저장한다.
/// </summary>
public class CardDeck : MonoBehaviour
{
    // 보유 카드 저장소 (key = cardID, value = 수량)
    private Dictionary<string, int> deck = new Dictionary<string, int>();

    // ======== 디버그/테스트 시드 ========
    [Header("Test Seed (임시) — 추후 상점 붙으면 끄세요")]
    public bool seedOnStart = true;
    public string[] seedIds = new[] { "1", "2", "3" }; // CSV의 cardID 값과 정확히 일치해야 함
    public int seedAmountEach = 3;

    void Start()
    {
        // 시작 테스트용: 필요시 카드 자동 추가
        if (seedOnStart)
        {
            foreach (var id in seedIds)
                Add(id, seedAmountEach);

            Debug.Log($"[CardDeck] Seed 완료. 보유 요약: {GetSummary()} (this={name}, id={GetInstanceID()})");
        }
    }

    /// <summary>디버그용 요약 문자열</summary>
    public string GetSummary()
    {
        if (deck.Count == 0) return "(empty)";
        StringBuilder sb = new StringBuilder();
        foreach (var kv in deck) sb.Append($"{kv.Key}:{kv.Value} ");
        return sb.ToString();
    }
    // ===================================

    /// <summary>카드 추가</summary>
    public void Add(string cardID, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(cardID)) return;
        cardID = cardID.Trim();

        if (deck.ContainsKey(cardID))
            deck[cardID] += amount;
        else
            deck[cardID] = amount;
    }

    /// <summary>카드 제거 (성공 시 true)</summary>
    public bool Remove(string cardID, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(cardID)) return false;
        cardID = cardID.Trim();

        if (!deck.ContainsKey(cardID)) return false;

        deck[cardID] -= amount;
        if (deck[cardID] <= 0)
            deck.Remove(cardID);

        return true;
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
    /// 보유 카드 중 수량 비율에 따라 무작위 cardID 반환.
    /// 없으면 null.
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

    // ---------- 편의 메서드(데이터 조회) ----------

    /// <summary>
    /// 무작위 cardID를 뽑고, CardDatabase를 통해 CardData까지 얻는다.
    /// </summary>
    public bool TryDrawRandomData(out CardData data, out string cardID)
    {
        data = null;
        cardID = GetRandomCardID();
        if (string.IsNullOrEmpty(cardID)) return false;

        if (CardDatabase.Instance == null)
        {
            Debug.LogError("[CardDeck] CardDatabase.Instance가 없습니다. 씬에 CardDatabase를 배치하세요.");
            return false;
        }

        data = CardDatabase.Instance.Get(cardID);
        if (data == null)
        {
            Debug.LogWarning($"[CardDeck] DB에서 cardID={cardID} 데이터를 찾지 못했습니다.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 현재 보유 목록을 (CardData, count)로 변환해 열거. UI에서 바로 artwork/name을 쓰기 좋음.
    /// </summary>
    public IEnumerable<(CardData data, int count)> EnumerateOwnedAsData()
    {
        if (CardDatabase.Instance == null)
        {
            Debug.LogError("[CardDeck] CardDatabase.Instance가 없습니다.");
            yield break;
        }

        foreach (var kv in deck)
        {
            var data = CardDatabase.Instance.Get(kv.Key);
            if (data != null)
                yield return (data, kv.Value);
            else
                Debug.LogWarning($"[CardDeck] DB에서 cardID={kv.Key} 데이터를 찾지 못했습니다.");
        }
    }
}
