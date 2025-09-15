using UnityEngine;
using System.Collections;

// ResourceProducer.cs
public class ResourceProducer : MonoBehaviour
{
    [Header("생산 설정")]
    public ResourceType resourceType;      // 어떤 자원 생산할지
    public int amountPerCycle = 100;       // 한 사이클당 생산량
    public float productionInterval = 10f; // 생산 간격(초)

    void Start()
    {
        StartCoroutine(ProduceRoutine());
    }

    private IEnumerator ProduceRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionInterval);
            ResourceManager.Instance.AddResource(resourceType, amountPerCycle);
        }
    }
}
