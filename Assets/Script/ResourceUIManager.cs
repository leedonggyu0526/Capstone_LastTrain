using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public struct ResourceUI
{
    public ResourceType type;   // Fuel, Food, MachinePart, Passenger
    public Image icon;          // Inspector에 드롭다운으로 할당
    public TMP_Text countText;      // “500/1000” 을 표시할 Text
}

public class ResourceUIManager : MonoBehaviour
{
    [Header("자원별 UI 아이콘과 텍스트")]
    public List<ResourceUI> resourceUIs = new List<ResourceUI>();

    void Start()
    {
        // 초기 한번 렌더
        UpdateAllUI();
    }

    void Update()
    {
        // 실시간 변동이 잦으면 매 프레임마다 갱신해도 되고,
        // 변동이 적다면 이벤트 기반(UpdateAllUI() 호출)으로도 충분합니다.
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        foreach (var rui in resourceUIs)
        {
            int current = ResourceManager.Instance.GetResource(rui.type);
            int max = ResourceManager.Instance.GetMaxCapacity(rui.type);
            rui.countText.text = $"{current}/{max}";
            // 아이콘는 미리 Inspector에서 할당해 두었다면 별도 업데이트 불필요
        }
    }
}
