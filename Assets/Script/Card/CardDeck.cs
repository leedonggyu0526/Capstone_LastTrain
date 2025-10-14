using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 플레이어 보유 카드 관리: cardID(string) ↔ 수량(int)
/// 씬 간 이동 시 파괴되지 않는 싱글턴으로 구현.
/// </summary>
public class CardDeck : MonoBehaviour
{
    public static CardDeck Instance { get; private set; } // ⬅️ 싱글턴 인스턴스

    // 보유 카드 저장소 (key = cardID, value = 수량)
    private Dictionary<string, int> deck = new Dictionary<string, int>();

    // ── 테스트용 시드(임시). 추후 상점/보상 연결 시 끄면 됨 ──
    [Header("Test Seed (임시) — 추후 상점 붙으면 끄세요")]
    public bool seedOnStart = true;
    public string[] seedIds = new[] { "1", "2", "3" }; // CSV의 cardID와 동일해야 함
    public int seedAmountEach = 3;

    void Awake()
    {
        // 씬 이동 시 파괴되지 않는 싱글턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // ⬅️ 씬 이동 시 파괴 방지
    }

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
        if (amount <= 0) return;

        if (deck.ContainsKey(cardID))
        {
            deck[cardID] += amount;
        }
        else
        {
            deck.Add(cardID, amount);
        }
        Debug.Log($"[CardDeck] Added {amount}x card {cardID}. Total: {deck[cardID]}");
    }

    /// <summary>보유 카드 목록 전체 반환</summary>
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

    // ... (TryDrawRandomData 등 다른 편의 함수는 생략)
}