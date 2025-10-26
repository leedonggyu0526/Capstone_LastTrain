using UnityEngine;
using System.Collections.Generic;

// 아이템 적용판별
public class ItemTrigger : MonoBehaviour
{
    [Header("최대 ID값 유동적 조정")]
    public int MaxID = 15;
    public ResourceProducer rp;
    public ResourceConsumer rc;
    // 아이템 변수 영역
    public float cor = 0.2f; // correction 값
    public float maxcor = 200f; // correction 값
    public float decrcor = 10f; // 소비량 감소값

    /// <summary>
    /// 아이템 ID별 효과가 적용되었는지 기억하는 딕셔너리 
    /// </summary>
    private Dictionary<int, bool> effectAppliedDict = new Dictionary<int, bool>();
    private ItemEffect resourceIncreaseEffect;
    private ItemEffect maxResourceIncreaseEffect;
    private ItemEffect resDecreaseEffect;


    private void Start()
    {
        for (int id = 0; id <= MaxID; id++)
        {
            effectAppliedDict[id] = false;
        }

        // ResourceIncrease 효과 인스턴스 생성
        resourceIncreaseEffect = new ResourceIncrease(rp);
        maxResourceIncreaseEffect = new MaxResourceIncrease(ResourceManager.Instance);
        resDecreaseEffect = new ConsumptionDecrease(rc);
        decrcor = 15f;
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
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} correction +{cor}");
                break;
            case 1:
                ItemEffectManager.Instance.ApplyEffect(resDecreaseEffect, type, decrcor);
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} 소비량 -{decrcor}");
                break;
            case 2:
                ItemEffectManager.Instance.ApplyEffect(resourceIncreaseEffect, type, cor);
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} correction +{cor}");
                break;
            case 3:
                ItemEffectManager.Instance.ApplyEffect(resDecreaseEffect, type, decrcor);
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} 소비량 -{decrcor}");
                break;
            case 4:
                ItemEffectManager.Instance.ApplyEffect(maxResourceIncreaseEffect, type, maxcor);
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} max correction +{maxcor}");
                break;
            case 5:
                ItemEffectManager.Instance.ApplyEffect(maxResourceIncreaseEffect, type, maxcor);
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} max correction +{maxcor}");
                break;
            case 6:
                ItemEffectManager.Instance.ApplyEffect(resourceIncreaseEffect, type, cor);
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} cor +{cor}");
                break;
            case 7:
                ItemEffectManager.Instance.ApplyEffect(resDecreaseEffect, type, decrcor);
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} 소비량-{decrcor}");
                break;
            case 10: //내구도 감소보정
                ItemEffectManager.Instance.ApplyEffect(resDecreaseEffect, type, 2f);
                Debug.Log($"[ItemTrigger] 아이템 ID {id} 효과 적용: {type} 소비량 -{2f}");
                break;
            // case 4, 5, 6... 등 다른 아이템 효과 추가
            default:
                Debug.LogWarning($"TODO: [ItemTrigger] ID {id}에 대한 아이템 효과가 정의되지 않았습니다.");
                return; // 효과를 적용하지 않고 함수 종료
        }
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
            case 1: return ResourceType.Food;
            case 2: return ResourceType.Fuel;       
            case 3: return ResourceType.Fuel;
            case 4: return ResourceType.Fuel;
            case 5: return ResourceType.Food;
            case 6: return ResourceType.MachinePart;
            case 7: return ResourceType.MachinePart;
            case 8: return ResourceType.Passenger;
            case 10: return ResourceType.Durability; // 내구도
            default: return ResourceType.Durability; // 내구도

        }
    }
}