using UnityEngine;
using System.Collections;

// ResourceConsumer.cs
public class ResourceConsumer : MonoBehaviour
{
    [Header("�Һ� ����")]
    public ResourceType resourceType;      // � �ڿ� �Һ�����
    public int amountPerCycle = 50;        // �� ����Ŭ�� �Һ�
    public float consumptionInterval = 5f; // �Һ� ����(��)

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
                //���ᰡ 0�̸� ���� ���� ���߿� �߰�
                Debug.LogWarning($"{resourceType} ����! ������: {ResourceManager.Instance.GetResource(resourceType)}");
            }
        }
    }
}
