using UnityEngine;
using System.Collections.Generic;

// 아이템 적용판별
public class ItemTrigger : MonoBehaviour
{
    [Header("최대 ID값 유동적 조정")]
    public int MaxID = 5;
    public ResourceProducer rp;
    // 아이템 변수 영역
    public float cor = 0.2f;



    /// <summary>
    /// 아이템 ID별 효과가 적용되었는지 기억하는 딕셔너리 
    /// </summary>
    private Dictionary<int, bool> effectAppliedDict = new Dictionary<int, bool>();
    private ItemEffect resourceIncreaseEffect;
    private ItemEffect maxResourceIncreaseEffect;


    private void Start()
    {
        for (int id = 0; id <= MaxID; id++)
        {
            effectAppliedDict[id] = false;
        }

        // ResourceIncrease 효과 인스턴스 생성
        resourceIncreaseEffect = new ResourceIncrease(rp);
        maxResourceIncreaseEffect = new MaxResourceIncrease(ResourceManager.Instance);
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
        ResourceType type = GetResourceTypeByID(id);
        // TODO: 각 ID에 맞는 아이템 효과 값(cor)과 효과 종류(resourceIncreaseEffect)를 적용
        switch(id)
        {
            case 0:
                ItemEffectManager.Instance.ApplyEffect(resourceIncreaseEffect, type, cor);
                break;
            case 1:
                ItemEffectManager.Instance.ApplyEffect(resourceIncreaseEffect, type, cor);
                break;
            case 2:
                ItemEffectManager.Instance.ApplyEffect(resourceIncreaseEffect, type, cor);
                break;
            case 3:
                ItemEffectManager.Instance.ApplyEffect(resourceIncreaseEffect, type, cor);
                break;
            case 4:
                ItemEffectManager.Instance.ApplyEffect(resourceIncreaseEffect, type, cor);
                break;
            case 5:
                ItemEffectManager.Instance.ApplyEffect(maxResourceIncreaseEffect, type, cor);
                break;
            // case 4, 5, 6... 등 다른 아이템 효과 추가
            default:
                Debug.LogWarning($"[ItemTrigger] ID {id}에 대한 아이템 효과가 정의되지 않았습니다.");
                return; // 효과를 적용하지 않고 함수 종료
        }
        Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} correction +{cor}");
    }

    /// <summary>
    /// 아이템 효과 제거
    /// </summary>
    private void RemoveItemEffect(int id)
    {
        ResourceType type = GetResourceTypeByID(id);
        ItemEffectManager.Instance.RemoveEffect(resourceIncreaseEffect, type, cor);
        Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 제거: {type} correction -{cor}");
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
            case 3: return ResourceType.Passenger; // 승객
            default: return ResourceType.Durability; // 내구도

        }
    }
}