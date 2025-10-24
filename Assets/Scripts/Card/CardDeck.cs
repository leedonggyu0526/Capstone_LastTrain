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
}