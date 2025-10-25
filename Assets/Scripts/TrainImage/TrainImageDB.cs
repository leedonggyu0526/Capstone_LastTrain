// SpriteDB.cs (또는 TrainImageDB.cs)
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TrainImageDB
{
    private static readonly Dictionary<(string type, int level), Sprite> _trainImageMap = new();
    private static readonly Dictionary<string, HashSet<int>> _trainImageLevelsByType = new(); // 등록된 레벨 추적

    private const string BASE_PATH = "Sprites/TrainImage/"; // 실제 폴더명과 100% 동일!

    public static void Set(string typeKey, int level, string imageName)
    {
        Debug.Log($"[TrainImageDB] Set() 호출됨 - typeKey: '{typeKey}', level: {level}, imageName: '{imageName}'");
        
        if (string.IsNullOrWhiteSpace(typeKey) || string.IsNullOrWhiteSpace(imageName)) {
            Debug.LogError("[TrainImageDB] typeKey 또는 imageName이 비어있습니다.");
            Debug.LogError("[TrainImageDB] typeKey: " + typeKey);
            Debug.LogError("[TrainImageDB] imageName: " + imageName);
            return;
        }

        string fullPath = BASE_PATH + imageName.Trim();
        Debug.Log($"[TrainImageDB] Resources 경로: {fullPath}");

        var sprite = Resources.Load<Sprite>(fullPath);
        if (sprite == null)
        {
            Debug.LogError($"[TrainImageDB] Sprite not found: {fullPath} (확장자 제외).");
            return;
        }

        _trainImageMap[(typeKey, level)] = sprite;

        if (!_trainImageLevelsByType.TryGetValue(typeKey, out var set))
        {
            set = new HashSet<int>();
            _trainImageLevelsByType[typeKey] = set;
            Debug.Log($"[TrainImageDB] 새로운 타입 '{typeKey}' 등록됨");
        }
        set.Add(level);

        Debug.Log($"[TrainImageDB] 등록 완료: ({typeKey}, L{level}) -> {fullPath}");
        Debug.Log($"[TrainImageDB] 현재 '{typeKey}' 레벨들: {string.Join(", ", set.OrderBy(x => x))}");
    }

    public static bool GetSprite(string typeKey, int level, out Sprite sprite)
    {
        string lowerTypeKey = typeKey.ToLower();
        return _trainImageMap.TryGetValue((lowerTypeKey, level), out sprite);
    }

    // 해당 타입에 이 레벨이 존재하는가?
    public static bool HasSprite(string typeKey, int level)
    {
        string lowerTypeKey = typeKey.ToLower();
        return _trainImageMap.ContainsKey((lowerTypeKey, level));
    }

    // 이 타입의 가능한 최댓 레벨 (없으면 -1)
    public static int GetMaxLevel(string typeKey)
    {
        if (string.IsNullOrEmpty(typeKey))
        {
            Debug.LogWarning("[TrainImageDB] GetMaxLevel: typeKey가 null이거나 비어있습니다.");
            return -1;
        }

        // 대소문자 구분 없이 검색
        string lowerTypeKey = typeKey.ToLower();
        
        if (!_trainImageLevelsByType.TryGetValue(lowerTypeKey, out var set))
        {
            Debug.LogWarning($"[TrainImageDB] GetMaxLevel: '{typeKey}' 타입이 등록되지 않았습니다. (검색 키: '{lowerTypeKey}')");
            Debug.LogWarning($"[TrainImageDB] 등록된 타입들: {string.Join(", ", _trainImageLevelsByType.Keys)}");
            return -1;
        }

        if (set == null || set.Count == 0)
        {
            Debug.LogWarning($"[TrainImageDB] GetMaxLevel: '{typeKey}' 타입에 등록된 레벨이 없습니다.");
            return -1;
        }

        int maxLevel = set.Max();
        Debug.Log($"[TrainImageDB] GetMaxLevel: '{typeKey}' 타입의 최대 레벨은 {maxLevel}입니다. (등록된 레벨들: {string.Join(", ", set.OrderBy(x => x))})");
        return maxLevel;
    }

    // 이 타입에서 level 이하 중 가장 가까운(최대) 존재 레벨 찾기 (없으면 -1)
    public static int GetNearestLeqLevel(string typeKey, int level)
    {
        string lowerTypeKey = typeKey.ToLower();
        if (!_trainImageLevelsByType.TryGetValue(lowerTypeKey, out var set) || set.Count == 0) return -1;
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
        if (string.IsNullOrEmpty(typeKey))
        {
            return "(typeKey is null or empty)";
        }

        string lowerTypeKey = typeKey.ToLower();
        if (!_trainImageLevelsByType.TryGetValue(lowerTypeKey, out var set))
        {
            return $"(type '{typeKey}' not found, searched as '{lowerTypeKey}')";
        }

        if (set == null || set.Count == 0)
        {
            return $"(type '{typeKey}' has no levels)";
        }

        var ordered = set.OrderBy(x => x).ToArray();
        return string.Join(", ", ordered.Select(l => $"L{l}"));
    }

    // 디버그: 모든 등록된 타입과 레벨들을 출력
    public static void debugAllLevels()
    {
        Debug.Log("[TrainImageDB] === 등록된 모든 타입과 레벨들 ===");
        if (_trainImageLevelsByType.Count == 0)
        {
            Debug.Log("[TrainImageDB] 등록된 타입이 없습니다.");
            return;
        }

        foreach (var kvp in _trainImageLevelsByType)
        {
            string typeKey = kvp.Key;
            var set = kvp.Value;
            string levels = set != null && set.Count > 0 
                ? string.Join(", ", set.OrderBy(x => x).Select(l => $"L{l}"))
                : "(no levels)";
            Debug.Log($"[TrainImageDB] '{typeKey}': {levels}");
        }
    }
}
