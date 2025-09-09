using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemTrigger : MonoBehaviour
{
    [Header("ID값 유동적 조정")]
    public int MaxID = 3;
    public ItemEffectManager ItemEffectManager;
    public ResourceProducer[] rp_factory;
    public GameObject factory;
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

    void Awake()
    {
        rp_factory = factory.GetComponents<ResourceProducer>(); //팩토리는 동일 스크립트 2개여서 배열로 접근 
    }

    void Update()
    {// 반복문으로 만들 것..
        for (int id = 0; id <= MaxID; id++)
        {
            bool hasItem = InventoryUIManager.Instance.HasItemByID(id);

            if (hasItem)
            {
                // 아이템 있는데 효과가 아직 안 적용된 경우에만 적용
                if (!effectAppliedDict[id])
                {
                    ApplyMyEffect(id);
                    effectAppliedDict[id] = true;
                }
            }
            else
            {
                // 아이템 없는데 효과가 적용되어 있으면 해제
                if (effectAppliedDict[id])
                {
                    RemoveMyEffect(id);
                    effectAppliedDict[id] = false;
                }
            }
        }

    }

    /// <summary>
    ///     switch문으로 ID별 효과 적용
    /// </summary>
    /// <param name="id">아이템 ID</param>
    void ApplyMyEffect(int id)
    {
        switch (id) {
            case 0: // 식량 생산량 증가
                IItemEffect effect0 = new ResourceIncrease(rp_factory[0]);
                ItemEffectManager.ApplyEffect(effect0);
                break;
            case 1: // 부품 생산량 증가
                IItemEffect effect1 = new ResourceIncrease(rp_factory[0]);
                ItemEffectManager.ApplyEffect(effect1);
                break;
            case 2: // 연료 생산량 증가
                IItemEffect effect2 = new ResourceIncrease(rp_factory[1]);
                ItemEffectManager.ApplyEffect(effect2);
                break;
            default:
                Debug.Log("예외 : 미지정 아이템 획득 판정");
                break;
        }
    }

    void RemoveMyEffect(int id)
        {//TODO : 해제, 복구 처리 (필요할 경우 제작)

            Debug.Log($"{id}아이템 소실로 효과 해제!");
        }
 
}

