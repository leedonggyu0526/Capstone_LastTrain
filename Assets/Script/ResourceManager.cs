using System.Collections.Generic;
using UnityEngine;

// ResourceManager.cs
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

    //�ڿ��� Ȯ��
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
        Debug.Log($"{type} {(amount >= 0 ? "+" : "")}{amount} �� �� {resources[type]} (Max {maxCapacities[type]})");
    }

    public int GetResource(ResourceType type)
    {
        return resources[type];
    }

    // �� �߰��� �޼���: �� �ڿ��� �ִ�ġ�� ��ȯ
    public int GetMaxCapacity(ResourceType type)
    {
        return maxCapacities.ContainsKey(type)
            ? maxCapacities[type]
            : int.MaxValue;
    }
}
