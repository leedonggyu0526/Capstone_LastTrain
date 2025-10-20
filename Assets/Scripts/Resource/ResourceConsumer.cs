using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// ResourceConsumer.cs
[System.Serializable]
public struct ResourceConsumption
{
    public ResourceType type;      // 자원 종류
    public int amountPerCycle;     // 사이클당 소비량(음수 불가)
}

public class ResourceConsumer : MonoBehaviour
{
    [Header("자원 소비")]
    public List<ResourceConsumption> consumptions = new List<ResourceConsumption>();
    public float consumptionInterval = 5f; // 소비 간격(초)

    private Coroutine _consumeRoutine;

    // 씬 전환 시 소비 시작/중지
    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        var current = SceneManager.GetActiveScene().name;
        if (current == "Settlement")
            StopConsume();
        else
            StartConsume();
    }

    // 씬 전환 시 소비 중지
    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        StopConsume();
    }

    // 소비 루틴
    private IEnumerator ConsumeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(consumptionInterval);

            if (ResourceManager.Instance == null)
            {
                Debug.LogWarning("[ResourceConsumer] ResourceManager.Instance가 없습니다.");
                continue;
            }

            foreach (var consumption in consumptions)
            {
                int amount = Mathf.Max(0, consumption.amountPerCycle);
                if (amount == 0) continue;

                // 충분한 자원이 있으면 소비, 아니면 경고
                bool isSuccess = ResourceManager.Instance.ConsumeResources(consumption.type, amount);
                if (!isSuccess)
                {
                    int current = ResourceManager.Instance.GetResource(consumption.type);
                    Debug.LogWarning($"{consumption.type} 부족! 현재량: {current}");
                }
            }
        }
    }

    // 소비 시작
    private void StartConsume()
    {
        if (_consumeRoutine == null)
            _consumeRoutine = StartCoroutine(ConsumeRoutine());
    }

    // 소비 중지
    private void StopConsume()
    {
        if (_consumeRoutine != null)
        {
            StopCoroutine(_consumeRoutine);
            _consumeRoutine = null;
        }
    }

    // 씬 전환 시 소비 시작/중지
    private void OnActiveSceneChanged(Scene prev, Scene next)
    {
        if (next.name == "Settlement")
            StopConsume();
        else
            StartConsume();
    }
}
