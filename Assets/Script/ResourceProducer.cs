using UnityEngine;
using System.Collections;

public class ResourceProducer : MonoBehaviour
{
    [Header("생산 설정")]
    public ResourceType resourceType;
    public int amountPerCycle = 100;
    public float productionInterval = 5f;
    /// <summary>
    /// 보정값 : 일단 아이템 소지시 증가량 ++
    /// </summary>
    public float correction = 1f; 
    private float multiplier = 1f;

    void Start()
    {
        StartCoroutine(ProduceRoutine());
    }

    private IEnumerator ProduceRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionInterval);
            int finalAmount = Mathf.RoundToInt(amountPerCycle * multiplier * correction);
            ResourceManager.Instance.AddResource(resourceType, finalAmount);
        }
    }

    // 업그레이드 매니저에서 호출할 함수
    public void SetMultiplier(float value)
    {
        multiplier = value;
    }
}
