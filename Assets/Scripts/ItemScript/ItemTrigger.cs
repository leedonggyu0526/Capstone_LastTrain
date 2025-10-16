using UnityEngine;
using System.Collections.Generic;

public class ItemTrigger : MonoBehaviour
{
    [Header("최대 ID값 유동적 조정")]
    public int MaxID = 3;
    public ResourceProducer rp;
    
    [Header("아이템별 효과 설정")]
    public float effectValue = 0.2f; // 생산량 증가 비율
    
    /// <summary>
    /// 아이템 ID별 효과가 적용되었는지 기억하는 딕셔너리 
    /// </summary>
    private Dictionary<int, bool> effectAppliedDict = new Dictionary<int, bool>();
    
    private void Start()
    {
        for (int id = 0; id <= MaxID; id++)
        {
            effectAppliedDict[id] = false;
        }
    }

    void Update()
    {
        for (int id = 0; id <= MaxID; id++)
        {
            bool hasItem = InventoryUIManager.Instance.HasItemByID(id);

            if (hasItem && !effectAppliedDict[id]) // 획득 판정 시
            {
                ApplyItemEffect(id);
                effectAppliedDict[id] = true;
            }
            else if (!hasItem && effectAppliedDict[id]) // 아이템 소실 시
            {
                RemoveItemEffect(id);
                effectAppliedDict[id] = false;
            }
        }
    }

    /// <summary>
    /// 아이템 효과 적용
    /// </summary>
    private void ApplyItemEffect(int id)
    {
        rp.AddCorrection(GetResourceTypeByID(id), effectValue);
        Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {GetResourceTypeByID(id)} correction +{effectValue}");
    }

    /// <summary>
    /// 아이템 효과 제거
    /// </summary>
    private void RemoveItemEffect(int id)
    {
        rp.AddCorrection(GetResourceTypeByID(id), -effectValue);
        Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 제거: {GetResourceTypeByID(id)} correction -{effectValue}");
    }

    /// <summary>
    /// 아이템 ID에 따른 자원 타입 반환
    /// </summary>
    private ResourceType GetResourceTypeByID(int id)
    {
        switch (id)
        {
            case 0: return ResourceType.Food;        // 식량
            case 1: return ResourceType.MachinePart; // 부품
            case 2: return ResourceType.Fuel;        // 연료
            default: return ResourceType.Food;
        }
    }
}