using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

public class CardDeck : MonoBehaviour
{
    public static CardDeck Instance { get; private set; }
    private Dictionary<string, int> deck = new Dictionary<string, int>();

    [Header("Test Seed (임시) — 추후 상점 붙으면 끄세요")]
    public bool seedOnStart = true;
    public string[] seedIds = new[] { "1", "2", "3" };
    public int seedAmountEach = 3;

    public static event Action<string> OnCardUsed; // 외부 UI 갱신 이벤트

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (seedOnStart)
        {
            foreach (var id in seedIds)
                Add(id, seedAmountEach);
        }
    }

    public void Add(string cardID, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(cardID) || amount <= 0) return;
        if (deck.ContainsKey(cardID))
            deck[cardID] += amount;
        else
            deck.Add(cardID, amount);
    }

    public IEnumerable<KeyValuePair<string, int>> GetAllOwned()
    {
        foreach (var kv in deck)
            yield return kv;
    }

    public string GetRandomCardID()
    {
        List<string> expanded = new List<string>();
        foreach (var kv in deck)
        {
            for (int i = 0; i < kv.Value; i++)
                expanded.Add(kv.Key);
        }
        if (expanded.Count == 0) return null;
        return expanded[UnityEngine.Random.Range(0, expanded.Count)];
    }

    public int GetUniqueCardCount() => deck.Count;

    public int GetTotalCardCount()
    {
        int total = 0;
        foreach (int count in deck.Values)
            total += count;
        return total;
    }

    public void UseCard(string cardID)
    {
        if (string.IsNullOrEmpty(cardID)) return;
        if (deck.ContainsKey(cardID))
        {
            deck[cardID]--;
            if (deck[cardID] <= 0)
            {
                deck.Remove(cardID);
            }
            OnCardUsed?.Invoke(cardID); // 이벤트 호출
        }
    }

    /// <summary>
    /// 지정된 카드의 수량을 감소시킵니다. (카드 판매 시 사용)
    /// </summary>
    /// <param name="cardID">제거할 카드의 ID</param>
    /// <param name="count">제거할 수량 (기본값 1)</param>
    public bool Remove(string cardID, int count = 1)
    {
        if (!deck.ContainsKey(cardID) || deck[cardID] < count)
        {
            Debug.LogWarning($"[CardDeck] 카드가 부족하여 {cardID}를 제거할 수 없습니다.");
            return false;
        }

        deck[cardID] -= count;

        if (deck[cardID] <= 0)
        {
            deck.Remove(cardID);
        }

        // 🚨 이벤트 발생: 덱의 수량이 변경되었음을 알립니다.
        OnCardUsed?.Invoke(cardID);

        return true;
    }
}