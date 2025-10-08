// Assets/Scripts/Building.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))] // 클릭 판정용
public class TrainKhan : MonoBehaviour
{
    [Header("CSV 1열의 typeKey와 동일해야 함")]
    public string typeKey = "Engine";

    [Min(0)]
    public int level = 0;

    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        ApplySprite(); // 시작 시 현재 레벨 스프라이트 적용
    }

    public void Upgrade()
    {
        int maxLevel = TrainImageDB.GetMaxLevel(typeKey);
        if (maxLevel < 0)
        {
            Debug.LogWarning($"[TrainKhan] 타입 '{typeKey}'에 등록된 레벨이 없습니다.");
            return;
        }

        if (level >= maxLevel)
        {
            Debug.LogError("[TrainKhan] 최대 레벨에 도달했습니다.");
            return;
        }
        level++;
        ApplySprite();
    }

    public void ApplySprite()
    {
        if (TrainImageDB.GetSprite(typeKey, level, out var sp))
        {
            _sr.sprite = sp;
            return;
        }
        // 현재 레벨 스프라이트가 없으면, 이하여서 가장 가까운 레벨로 폴백
        int fallback = TrainImageDB.GetNearestLeqLevel(typeKey, level);
        if (fallback >= 0 && TrainImageDB.GetSprite(typeKey, fallback, out var sp2))
            _sr.sprite = sp2;
    }
}
