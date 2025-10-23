using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 플레이어 보유 카드 관리: cardID(string) ↔ 수량(int)
/// 씬 간 이동 시 파괴되지 않는 싱글턴으로 구현.
/// </summary>
public class CardDeck : MonoBehaviour
{
    // ⬇️ 🚨 Lazy Instantiation을 위한 private 필드 🚨 
    private static CardDeck instance;

    public static CardDeck Instance // ⬅️ 싱글턴 인스턴스 (접근 시 초기화 시도)
    {
        get
        {
            if (instance == null)
            {
                // 인스턴스가 없으면 씬에서 찾아 등록 (복구 로직)
                instance = FindFirstObjectByType<CardDeck>();

                if (instance == null)
                {
                    Debug.LogError("CardDeck 인스턴스를 씬에서 찾을 수 없습니다. (배치 오류)");
                    return null;
                }

                // 찾은 인스턴스에 DontDestroyOnLoad를 적용 (Awake 대신 여기서 초기화 시도)
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
    // ⬆️ Instance 속성 변경 완료 ⬆️

    // 보유 카드 저장소 (key = cardID, value = 수량)
    private Dictionary<string, int> deck = new Dictionary<string, int>();

    // ── 테스트용 시드(임시). 추후 상점/보상 연결 시 끄면 됨 ──
    [Header("Test Seed (임시) — 추후 상점 붙으면 끄세요")]
    public bool seedOnStart = true;
    public string[] seedIds = new[] { "1", "2", "3" }; // CSV의 cardID와 동일해야 함
    public int seedAmountEach = 3;

    void Awake()
    {
        // 씬 이동 시 파괴되지 않는 싱글턴 구현 (Awake가 실행되면 바로 private 필드에 등록)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
            // 보유 수량만큼 리스트에 카드 ID를 반복해서 추가 (확장)
            for (int i = 0; i < kv.Value; i++)
                expanded.Add(kv.Key);
        }
        if (expanded.Count == 0) return null; // 보유 카드가 없으면 null 반환

        // 확장된 리스트에서 무작위 인덱스를 뽑아 카드 ID 반환
        return expanded[Random.Range(0, expanded.Count)];
    }

    /// <summary>외부에서 CardDeck 인스턴스를 싱글턴으로 설정합니다. (주로 복구용)</summary>
    public static void RegisterInstance(CardDeck targetInstance)
    {
        if (instance == null)
        {
            instance = targetInstance;
        }
    }

    // ... (GetSummary 등 다른 함수는 필요에 따라 추가/유지)
}