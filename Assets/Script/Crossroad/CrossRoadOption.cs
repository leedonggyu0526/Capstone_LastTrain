// Assets/Scripts/CrossRoadOption.cs
using UnityEngine;

[System.Serializable]
public class CrossRoadOption
{
    public int id;            // 고유 번호
    public string title;      // 카드 제목
    public string desc;       // 설명(쉼표 있으면 CSV에서 "따옴표"로 감싸기)
    public string iconPath;   // Resources 경로 예: "CrossRoad/Icons/crate"
    public string bgPath;     // Resources 경로 예: "CrossRoad/Backgrounds/blue"
    public int weight = 1;    // 가중치(1 이상)

    // (선택) 간단 유효성 검사
    public bool IsValid(out string reason)
    {
        if (id <= 0) { reason = "id가 1 이상이어야 함"; return false; }
        if (string.IsNullOrWhiteSpace(title)) { reason = "title이 비어 있음"; return false; }
        if (string.IsNullOrWhiteSpace(iconPath)) { reason = "iconPath가 비어 있음"; return false; }
        if (string.IsNullOrWhiteSpace(bgPath)) { reason = "bgPath가 비어 있음"; return false; }
        if (weight < 1) { reason = "weight는 1 이상"; return false; }
        reason = null; return true;
    }
}

