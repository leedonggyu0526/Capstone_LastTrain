using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private Dictionary<ResourceType, int> maxCapacities = new Dictionary<ResourceType, int>()
    {
        { ResourceType.Fuel,        1000 },
        { ResourceType.Food,        2000 },
        { ResourceType.MachinePart, 500  },
        { ResourceType.Passenger,   100  }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
                resources[type] = maxCapacities[type] / 2;
        }
        else Destroy(gameObject);
    }

    void Start()
    {
#if UNITY_EDITOR
        // 에디터에서만 5초마다 리소스 로그 출력
        InvokeRepeating(nameof(LogResources), 1f, 5f);
#endif
    }

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

    public void AddResource(ResourceType type, int amount)
    {
        int newAmount = resources[type] + amount;
        if (maxCapacities.ContainsKey(type))
            newAmount = Mathf.Clamp(newAmount, 0, maxCapacities[type]);

        resources[type] = newAmount;
        Debug.Log($"{type} {(amount >= 0 ? "+" : "")}{amount} 추가 후 {resources[type]} (Max {maxCapacities[type]})");
    }

    public int GetResource(ResourceType type)
    {
        return resources[type];
    }

    public int GetMaxCapacity(ResourceType type)
    {
        return maxCapacities.ContainsKey(type)
            ? maxCapacities[type]
            : int.MaxValue;
    }

    // 자원 소비 함수
    public bool ConsumeResources(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
        {
            Debug.LogWarning($"{type} 리소스가 존재하지 않습니다!");
            return false;
        }

        if (resources[type] < amount)
        {
            Debug.LogWarning($"{type} 부족! 현재량: {resources[type]}");
            return false;
        }

        resources[type] -= amount;
        Debug.Log($"{type} -{amount} 소비 후 {resources[type]} (Max {maxCapacities[type]})");
        return true;
    }
}
