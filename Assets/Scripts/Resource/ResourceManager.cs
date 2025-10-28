using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ResourceManager Instance { get; private set; }

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    // 자원 최대 용량
    private Dictionary<ResourceType, int> maxCapacities = new Dictionary<ResourceType, int>()
    {
        { ResourceType.Fuel,        1000 },
        { ResourceType.Food,        2000 },
        { ResourceType.MachinePart, 500  },
        { ResourceType.Passenger,   100  },
        { ResourceType.Durability,  1000 }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 자원 초기화
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType))){
                if(type == ResourceType.Durability){
                    resources[type] = maxCapacities[type];
                }else{
                    resources[type] = maxCapacities[type] / 2;
                }
            }
        }
        else Destroy(gameObject);
    }

    // 시작 시 자원 로그 출력
    void Start()
    {
#if UNITY_EDITOR
        // 에디터에서만 5초마다 리소스 로그 출력
        InvokeRepeating(nameof(LogResources), 1f, 5f);
#endif
    }

    // 자원 로그 출력
    void LogResources()
    {
        Debug.Log("=== Current Resources ===");
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            int current = GetResource(type);
            int max = maxCapacities.ContainsKey(type) ? maxCapacities[type] : -1;
            Debug.Log($"{type}: {current} / {max}");
        }
    }

    // 자원 추가
    public void AddResource(ResourceType type, int amount)
    {
        int newAmount = resources[type] + amount;
        if (maxCapacities.ContainsKey(type))
            newAmount = Mathf.Clamp(newAmount, 0, maxCapacities[type]);

        resources[type] = newAmount;
        Debug.Log($"{type} {(amount >= 0 ? "+" : "")}{amount} 추가 후 {resources[type]} (Max {maxCapacities[type]})");
    }

    // 자원 조회
    public int GetResource(ResourceType type)
    {
        return resources[type];
    }

    // 자원 최대 용량 조회
    public int GetMaxCapacity(ResourceType type)
    {
        return maxCapacities.ContainsKey(type)
            ? maxCapacities[type]
            : int.MaxValue;
    }

    // 자원 소비 함수
    // 자원 소비 성공 여부 반환
    public bool ConsumeResources(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
        {
            Debug.LogWarning($"{type} 리소스가 존재하지 않습니다!");
            return false;
        }

        if (resources[type] < amount)
        {
            if(type == ResourceType.Durability)
            {
                this.SetResource(ResourceType.Durability, 0);
            }
            Debug.LogWarning($"{type} 부족! 현재량: {resources[type]}");
            return false;
        }

        resources[type] -= amount;
        Debug.Log($"{type} -{amount} 소비 후 {resources[type]} (Max {maxCapacities[type]})");
        return true;
    }
       // 최대 용량 증가 (amount <= 0 이면 아무 동작 안 함)
    public bool IncreaseMaxCapacity(ResourceType type, int amount, bool adjustCurrent = true)
    {
        if (amount <= 0) return false;

        if (maxCapacities.ContainsKey(type))
            maxCapacities[type] += amount;
        else
            maxCapacities[type] = amount;

        // 현재 보유량이 새 최대값을 넘지 않도록 조정(옵션)
        if (adjustCurrent && resources.ContainsKey(type))
            resources[type] = Mathf.Clamp(resources[type], 0, maxCapacities[type]);

        Debug.Log($"[ResourceManager] {type} MaxCapacity 증가: +{amount} => {maxCapacities[type]}");
        return true;
    }

    public void SetResource(ResourceType type, int amount)
    {
        resources[type] = amount;
    }
}
