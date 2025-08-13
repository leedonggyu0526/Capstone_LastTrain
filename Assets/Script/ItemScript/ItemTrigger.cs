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
    /// 아이템 ID별 효과 적용여부 저장
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
        rp_factory = factory.GetComponents<ResourceProducer>(); //팩토리 같은 스크립트 2개여서 배열로 접근 
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

    void ApplyMyEffect(int id)
    {
        switch (id) {
            case 2:
                IItemEffect effect3 = new ResourceIncrease(rp_factory[1]);
                ItemEffectManager.ApplyEffect(effect3);
                break;
            default:
                Debug.Log("아이템 소지 3번 이외");
                break;
        }
    }

    void RemoveMyEffect(int id)
        {//TODO : 해제, 복구 처리

            Debug.Log($"{id}아이템 소실로 효과 해제!");
        }
 
}

