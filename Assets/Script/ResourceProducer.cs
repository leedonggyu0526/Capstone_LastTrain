using UnityEngine;
using System.Collections;

public class ResourceProducer : MonoBehaviour
{
    [Header("���� ����")]
    public ResourceType resourceType;
    public int amountPerCycle = 100;
    public float productionInterval = 5f;

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
            int finalAmount = Mathf.RoundToInt(amountPerCycle * multiplier);
            ResourceManager.Instance.AddResource(resourceType, finalAmount);
        }
    }

    // ���׷��̵� �Ŵ������� ȣ���� �Լ�
    public void SetMultiplier(float value)
    {
        multiplier = value;
    }
}
