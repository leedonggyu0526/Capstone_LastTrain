using UnityEngine;

// PassengerSpawner.cs
public class PassengerSpawner : MonoBehaviour
{
    [Header("승객 생성 설정")]
    public int minSpawn = 1;           // 최소 생성 수
    public int maxSpawn = 5;           // 최대 생성 수
    public int maxPassengers = 100;    // 최대 승객 수

    void Update()
    {
        // 테스트: P 키 입력 시 승객 생성
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnPassengers();
            Debug.Log("테스트: P 키 입력 시 승객 생성");
        }
    }

    /// <summary>
    /// 승객 생성 함수
    /// </summary>
    public void TrySpawnPassengers()
    {
        int current = ResourceManager.Instance.GetResource(ResourceType.Passenger);
        if (current >= maxPassengers)
        {
            Debug.Log("승객이 최대 수에 도달했습니다.");
            return;
        }

        int spawnAmount = Random.Range(minSpawn, maxSpawn + 1);
        int availableSpace = maxPassengers - current;
        int amount = Mathf.Min(spawnAmount, availableSpace);

        ResourceManager.Instance.AddResource(ResourceType.Passenger, amount);
        Debug.Log($"승객 +{amount} 추가 후 {current + amount}/{maxPassengers}");
    }
}
