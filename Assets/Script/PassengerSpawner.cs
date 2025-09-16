using UnityEngine;

// PassengerSpawner.cs
public class PassengerSpawner : MonoBehaviour
{
    [Header("승객 생성 설정")]
    public int minSpawn = 1;           // 최소 생성 수
    public int maxSpawn = 5;           // 최대 생성 수
    public int maxPassengers = 100;    // 전체 승객 수용 한계

    void Update()
    {
        // 테스트용: P 키 누르면 승객 스폰 시도
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnPassengers();
            Debug.Log("테스트: P 키로 승객 스폰 시도");
        }
    }

    /// <summary>
    /// 조건 충족 시 호출되는 승객 생성 로직
    /// </summary>
    public void TrySpawnPassengers()
    {
        int current = ResourceManager.Instance.GetResource(ResourceType.Passenger);
        if (current >= maxPassengers)
        {
            Debug.Log("승객이 이미 최대치에 도달했습니다.");
            return;
        }

        int spawnAmount = Random.Range(minSpawn, maxSpawn + 1);
        int availableSpace = maxPassengers - current;
        int amount = Mathf.Min(spawnAmount, availableSpace);

        ResourceManager.Instance.AddResource(ResourceType.Passenger, amount);
        Debug.Log($"승객 +{amount} → 현재 {current + amount}/{maxPassengers}");
    }
}
