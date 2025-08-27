using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CSV에서 카드 정의를 읽어 메모리에 캐싱하는 전역 데이터베이스.
/// 카드 정의(이름, 설명, 이미지, 레어도 등)는 여기서 관리되고,
/// 다른 시스템은 cardID(string)만 넘겨주면 CardData를 가져올 수 있다.
/// </summary>
public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance { get; private set; }

    // cardID → CardData 캐싱
    private Dictionary<string, CardData> cards = new Dictionary<string, CardData>();

    void Awake()
    {
        // 싱글턴 처리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Initialize();
    }

    /// <summary>
    /// CSV 파일을 읽어 카드 데이터를 Dictionary에 로드한다.
    /// </summary>
    private void Initialize()
    {
        TextAsset csv = Resources.Load<TextAsset>("CardData"); // Resources/CardData.csv
        if (csv == null)
        {
            Debug.LogError("[CardDatabase] CardData.csv 파일을 찾을 수 없습니다.");
            return;
        }

        string[] lines = csv.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더이므로 스킵
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] cols = line.Split(',');
            CardData data = CardCsvLoader.CreateFromCsvRow(cols);

            if (!cards.ContainsKey(data.cardID))
                cards.Add(data.cardID, data);
        }

        Debug.Log($"[CardDatabase] 카드 {cards.Count}개 로드 완료");
    }

    // 특정 카드 가져오기
    public CardData Get(string cardID)
    {
        if (cards.TryGetValue(cardID, out var data))
            return data;
        Debug.LogWarning($"[CardDatabase] cardID {cardID} 를 찾을 수 없습니다.");
        return null;
    }

    // 전체 카드 반환
    public IEnumerable<CardData> GetAll() => cards.Values;

    // 무작위 카드 하나 반환
    public CardData GetRandom()
    {
        if (cards.Count == 0) return null;
        int idx = Random.Range(0, cards.Count);
        foreach (var kv in cards)
        {
            if (idx-- == 0) return kv.Value;
        }
        return null;
    }
}
