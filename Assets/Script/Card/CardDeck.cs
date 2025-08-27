using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 보유한 카드들을 관리하는 클래스.
/// cardID(string)과 수량(int)을 저장한다.
/// </summary>
public class CardDeck : MonoBehaviour
{
    // 보유 카드 저장소 (key = cardID, value = 수량)
    private Dictionary<string, int> deck = new Dictionary<string, int>();

    void Start()
    {
        // [임시 코드] ============================
        // 게임 시작 시 gold 카드 3장을 자동으로 추가한다.
        // 추후 상점/보상 시스템이 붙으면 반드시 제거할 것!
        // Start도 지워도 됨
        Add("1", 3); // cardID = "1" (CSV에서 goldcard 라인)
        Debug.Log("[CardDeck] 테스트용으로 gold 카드 3장을 추가했습니다.");
        // =======================================
    }

    /// <summary>
    /// 카드 추가
    /// </summary>
    public void Add(string cardID, int amount = 1)
    {
        if (deck.ContainsKey(cardID))
            deck[cardID] += amount;
        else
            deck[cardID] = amount;
    }

    /// <summary>
    /// 카드 제거 (성공 시 true 반환)
    /// </summary>
    public bool Remove(string cardID, int amount = 1)
    {
        if (!deck.ContainsKey(cardID)) return false;

        deck[cardID] -= amount;
        if (deck[cardID] <= 0)
            deck.Remove(cardID);

        return true;
    }

    /// <summary>
    /// 해당 카드ID의 보유 수량 반환
    /// </summary>
    public int GetCount(string cardID)
    {
        return deck.ContainsKey(cardID) ? deck[cardID] : 0;
    }

    /// <summary>
    /// 전체 보유 카드 (cardID, count) 쌍 반환
    /// </summary>
    public IEnumerable<KeyValuePair<string, int>> GetAllOwned()
    {
        foreach (var kv in deck)
            yield return kv;
    }

    /// <summary>
    /// 보유 카드 중 수량 비율에 따라 무작위 cardID 반환
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
}
