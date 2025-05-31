using UnityEngine;

// PassengerSpawner.cs
public class PassengerSpawner : MonoBehaviour
{
    [Header("�°� ���� ����")]
    public int minSpawn = 1;           // �ּ� ���� ��
    public int maxSpawn = 5;           // �ִ� ���� ��
    public int maxPassengers = 100;    // ��ü �°� ���� �Ѱ�

    void Update()
    {
        // �׽�Ʈ��: P Ű ������ �°� ���� �õ�
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnPassengers();
            Debug.Log("�׽�Ʈ: P Ű�� �°� ���� �õ�");
        }
    }

    /// <summary>
    /// ���� ���� �� ȣ��Ǵ� �°� ���� ����
    /// </summary>
    public void TrySpawnPassengers()
    {
        int current = ResourceManager.Instance.GetResource(ResourceType.Passenger);
        if (current >= maxPassengers)
        {
            Debug.Log("�°��� �̹� �ִ�ġ�� �����߽��ϴ�.");
            return;
        }

        int spawnAmount = Random.Range(minSpawn, maxSpawn + 1);
        int availableSpace = maxPassengers - current;
        int amount = Mathf.Min(spawnAmount, availableSpace);

        ResourceManager.Instance.AddResource(ResourceType.Passenger, amount);
        Debug.Log($"�°� +{amount} �� ���� {current + amount}/{maxPassengers}");
    }
}
