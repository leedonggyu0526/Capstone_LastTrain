// SpriteDB.cs (또는 TrainImageDB.cs)
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TrainImageDB
{
    private static readonly Dictionary<(string type, int level), Sprite> _trainImageMap = new();
    private static readonly Dictionary<string, HashSet<int>> _trainImageLevelsByType = new(); // 등록된 레벨 추적

    private const string BASE_PATH = "trainImage/"; // 실제 폴더명과 100% 동일!

    public static void Set(string typeKey, int level, string imageName)
    {
        if (string.IsNullOrWhiteSpace(typeKey) || string.IsNullOrWhiteSpace(imageName)) {
            Debug.LogError("[TrainImageDB] typeKey 또는 imageName이 비어있습니다.");
            Debug.LogError("[TrainImageDB] typeKey: " + typeKey);
            Debug.LogError("[TrainImageDB] imageName: " + imageName);
            return;
        }

        string normKey = (typeKey ?? string.Empty).Trim().ToLowerInvariant();
        string fullPath = BASE_PATH + normKey + "/" + imageName.Trim();

        var sprite = Resources.Load<Sprite>(fullPath);
        if (sprite == null)
        {
            Debug.LogError($"[TrainImageDB] Sprite not found: Resources/{fullPath} (확장자 제외).");
            return;
        }

        _trainImageMap[(normKey, level)] = sprite;

        if (!_trainImageLevelsByType.TryGetValue(normKey, out var set))
        {
            set = new HashSet<int>();
            _trainImageLevelsByType[normKey] = set;
        }
        set.Add(level);

        // 등록 확인 로그 — 잠깐 켜두고 확인 후 주석 처리해도 됨
        // Debug.Log($"[TrainImageDB] OK: ({typeKey}, L{level}) -> {fullPath}");
    }

    public static bool GetSprite(string typeKey, int level, out Sprite sprite)
    {
        string normKey = (typeKey ?? string.Empty).Trim().ToLowerInvariant();
        return _trainImageMap.TryGetValue((normKey, level), out sprite);
    }

    // 해당 타입에 이 레벨이 존재하는가?
    public static bool HasSprite(string typeKey, int level)
    {
        string normKey = (typeKey ?? string.Empty).Trim().ToLowerInvariant();
        return _trainImageMap.ContainsKey((normKey, level));
    }

    // 이 타입의 가능한 최댓 레벨 (없으면 -1)
    public static int GetMaxLevel(string typeKey)
    {
        string normKey = (typeKey ?? string.Empty).Trim().ToLowerInvariant();
        return _trainImageLevelsByType.TryGetValue(normKey, out var set) && set.Count > 0 ? set.Max() : -1;
    }

    // 이 타입에서 level 이하 중 가장 가까운(최대) 존재 레벨 찾기 (없으면 -1)
    public static int GetNearestLeqLevel(string typeKey, int level)
    {
        string normKey = (typeKey ?? string.Empty).Trim().ToLowerInvariant();
        if (!_trainImageLevelsByType.TryGetValue(normKey, out var set) || set.Count == 0) return -1;
        int best = -1;
        foreach (var lv in set)
            if (lv <= level && lv > best) best = lv;
        return best;
    }

    public static int ExtractLevelFromHeader(string header)
    {
        if (string.IsNullOrEmpty(header)) return -1;
        var m = Regex.Match(header, @"(\d+)");
        return m.Success ? int.Parse(m.Value) : -1;
    }

    // 디버그: 이 타입에 어떤 레벨들이 등록됐는지 문자열로 덤프
    public static string debugLevels(string typeKey)
    {
        string normKey = (typeKey ?? string.Empty).Trim().ToLowerInvariant();
        if (!_trainImageLevelsByType.TryGetValue(normKey, out var set) || set.Count == 0) return "(none)";
        var ordered = set.OrderBy(x => x).ToArray();
        return string.Join(", ", ordered.Select(l => $"L{l}"));
    }
}
