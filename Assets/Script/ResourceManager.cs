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
            foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
                resources[t] = 0;
        }
        else Destroy(gameObject);
    }

    void Update()
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
        Debug.Log($"{type} {(amount >= 0 ? "+" : "")}{amount} → 총 {resources[type]} (Max {maxCapacities[type]})");
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

    // 🔽 여기에 추가한 메서드
    public bool ConsumeResources(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
        {
            Debug.LogWarning($"{type} 리소스가 존재하지 않습니다!");
            return false;
        }

        if (resources[type] < amount)
        {
            Debug.LogWarning($"{type} 부족! 남은량: {resources[type]}");
            return false;
        }

        resources[type] -= amount;
        Debug.Log($"{type} -{amount} → 총 {resources[type]} (Max {maxCapacities[type]})");
        return true;
    }
}
