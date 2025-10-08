using UnityEngine;

// PassengerSpawner.cs
public class PassengerSpawner : MonoBehaviour
{
    [Header("승객 스폰 설정")]
    public int defaultMinSpawn = 1;           // 최소 스폰 수
    public int defaultMaxSpawn = 5;           // 최대 스폰 수
    public int maxPassengers = 100;           // 총 승객 수 한도(초기값). Awake에서 ResourceManager로 갱신

    void Awake()
    {
        if (ResourceManager.Instance != null)
        {
            maxPassengers = ResourceManager.Instance.GetMaxCapacity(ResourceType.Passenger);
        }
        else
        {
            Debug.LogWarning("[PassengerSpawner] ResourceManager.Instance가 없어 기본값을 사용합니다.");
        }
    }

    void Update()
    {
        // 테스트용: P 키 입력으로 승객 스폰 시도
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnPassengers(defaultMinSpawn, defaultMaxSpawn);
            Debug.Log("테스트: P 키로 승객 스폰 시도");
        }
    }

    /// <summary>
    /// 조건을 확인한 뒤 호출되는 승객 스폰 로직
    /// </summary>
    public void TrySpawnPassengers(int minSpawn, int maxSpawn)
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("[PassengerSpawner] ResourceManager.Instance가 NULL이라 스폰을 건너뜁니다.");
            return;
        }
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
        Debug.Log($"승객 +{amount} → 총 {current + amount}/{maxPassengers}");
    }
}
