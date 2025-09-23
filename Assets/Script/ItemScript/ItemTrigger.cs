using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemTrigger : MonoBehaviour
{
    [Header("최대 ID값 유동적 조정")]
    public int MaxID = 3;
    public ItemEffectManager ItemEffectManager;
    public ResourceProducer[] rp_factory;
    public ResourceProducer rp_plant;
    public GameObject factory;
    public GameObject plant;
    /// <summary>
    /// 아이템 ID별 효과가 적용되었는지 기억하는 딕셔너리 
    /// </summary>
    private Dictionary<int, bool> effectAppliedDict = new Dictionary<int, bool>();
    /// <summary>
    /// 아이템 ID별 효과 객체 캐싱
    /// </summary>
    private Dictionary<int, IItemEffect> effectCache = new Dictionary<int, IItemEffect>();
    private void Start()
    {
        for (int id = 0; id <= MaxID; id++)
        {
            effectAppliedDict[id] = false;
            effectCache[id] = CreateEffectByID(id);
        }
    }

    void Awake()
    {
        rp_factory = factory.GetComponents<ResourceProducer>(); //팩토리는 동일 스크립트 2개여서 배열로 접근
        rp_plant =  plant.GetComponent<ResourceProducer>();
    }

    void Update()
    {// 반복문으로 만들 것..
        for (int id = 0; id <= MaxID; id++)
        {
            bool hasItem = InventoryUIManager.Instance.HasItemByID(id);

            if (hasItem && !effectAppliedDict[id]) // 획득 판정시
            {
                if (effectCache[id] != null)
                    ItemEffectManager.ApplyEffect(effectCache[id]);
                effectAppliedDict[id] = true;
            }
            else if (!hasItem && effectAppliedDict[id])
            {
                // if (effectCache[id] != null)
                //     ItemEffectManager.RemoveEffect(effectCache[id]); // RemoveEffect 구현 필요
                // effectAppliedDict[id] = false;
            }
        }

    }

    /// <summary>
    /// ID별 효과 객체 생성
    /// </summary>
    private IItemEffect CreateEffectByID(int id)
    {
        switch (id)
        {
            case 0: return new ResourceIncrease(rp_plant); //식량 생산량 증가
            case 1: return new ResourceIncrease(rp_factory[0]); //부품 생산량 증가
            case 2: return new ResourceIncrease(rp_factory[1]); //연료 생산량 증가
            default: return null; //예외 : 미지정 아이템 획득 판정
        }
    }

    void RemoveMyEffect(int id)
        {//TODO : 해제, 복구 처리 (필요할 경우 제작)

            Debug.Log($"{id}아이템 소실로 효과 해제!");
        }
 
}

