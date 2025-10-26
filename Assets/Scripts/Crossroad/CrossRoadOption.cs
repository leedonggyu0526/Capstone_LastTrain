// Assets/Scripts/CrossRoadOption.cs
using UnityEngine;

[System.Serializable]
public class CrossRoadOption
{
    public string id;            // 고유 번호
    public string title;      // 카드 제목
    public string desc;       // 설명
    public int weight = 1;    // 가중치(1 이상)
    public Sprite sceneBg; // 씬 배경
    public Sprite icon; // 아이콘
    public Sprite bg; // 배경


    // (선택) 간단 유효성 검사
    public bool IsValid(out string reason)
    {
        if (string.IsNullOrWhiteSpace(id)) { reason = "id가 비어 있음"; return false; }
        if (string.IsNullOrWhiteSpace(title)) { reason = "title이 비어 있음"; return false; }
        if (icon == null) { reason = "icon이 비어 있음"; return false; }
        if (bg == null) { reason = "bg가 비어 있음"; return false; }
        if (weight < 1) { reason = "weight는 1 이상"; return false; }
        if (sceneBg == null) { reason = "sceneBg가 비어 있음"; return false; }
        reason = null; return true;
    }

    public void SetSprites(){
        icon = Resources.Load<Sprite>("Sprites/CrossRoad/Icons/" + id);
        sceneBg = Resources.Load<Sprite>("Sprites/CrossRoad/SceneBackgrounds/" + id);
        bg = Resources.Load<Sprite>("Sprites/CrossRoad/Backgrounds/Crossroad_Bg");
        if (icon == null) Debug.LogWarning($"[CrossRoadOption] 아이콘 로드 실패: Sprites/CrossRoad/Icons/{id}");
        if (sceneBg == null) Debug.LogWarning($"[CrossRoadOption] 씬배경 로드 실패: Sprites/CrossRoad/SceneBackgrounds/{id}");
        if (bg == null) Debug.LogWarning($"[CrossRoadOption] 배경 로드 실패: Sprites/CrossRoad/Backgrounds/Crossroad_Bg");
    }
}

