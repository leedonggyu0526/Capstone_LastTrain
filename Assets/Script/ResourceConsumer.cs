using UnityEngine;
using System.Collections;

// ResourceConsumer.cs
public class ResourceConsumer : MonoBehaviour
{
    [Header("소비 설정")]
    public ResourceType resourceType;      // 어떤 자원 소비할지
    public int amountPerCycle = 50;        // 한 사이클당 소비량
    public float consumptionInterval = 5f; // 소비 간격(초)

    void Start()
    {
        StartCoroutine(ConsumeRoutine());
    }

    private IEnumerator ConsumeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(consumptionInterval);
            if (ResourceManager.Instance.GetResource(resourceType) >= amountPerCycle)
            {
                ResourceManager.Instance.AddResource(resourceType, -amountPerCycle);
            }
            else
            {
                //연료가 0이면 게임 오버 나중에 추가
                Debug.LogWarning($"{resourceType} 부족! 남은량: {ResourceManager.Instance.GetResource(resourceType)}");
            }
        }
    }
}
