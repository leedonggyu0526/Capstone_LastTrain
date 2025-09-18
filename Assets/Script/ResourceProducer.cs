using UnityEngine;
using System.Collections;

// ResourceProducer.cs
public class ResourceProducer : MonoBehaviour
{
    [Header("���� ����")]
    public ResourceType resourceType;      // � �ڿ� ��������
    public int amountPerCycle = 100;       // �� ����Ŭ�� ���귮
    public float productionInterval = 10f; // ���� ����(��)

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
