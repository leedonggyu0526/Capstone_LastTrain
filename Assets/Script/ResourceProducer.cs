using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// ResourceProducer.cs
[System.Serializable]
public struct ResourceProduction
{
    public ResourceType type;          // 어떤 자원을 생산하는지
    public int amountPerCycle;         // 한 사이클 생산량
}

public class ResourceProducer : MonoBehaviour
{
    [Header("자원 생산")]
    public List<ResourceProduction> productions = new List<ResourceProduction>();
    public float productionInterval = 10f; // 생산 간격(초)

    private Coroutine _produceRoutine;

    // 씬 전환 시 생산 시작/중지
    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        var current = SceneManager.GetActiveScene().name;
        if (current == "Settlement")
            StopProduce();
        else
            StartProduce();
    }

    // 씬 전환 시 생산 중지
    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        StopProduce();
    }
        
    // 생산 루틴
    private IEnumerator ProduceRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionInterval);

            if (ResourceManager.Instance == null)
            {
                Debug.LogWarning("[ResourceProducer] ResourceManager.Instance가 없습니다.");
                continue;
            }

            foreach (var production in productions)
            {
                int amount = Mathf.Max(0, production.amountPerCycle);
                if (amount == 0) continue;
                ResourceManager.Instance.AddResource(production.type, amount);
            }
        }
    }

    // 생산 시작
    private void StartProduce()
    {
        if (_produceRoutine == null)
            _produceRoutine = StartCoroutine(ProduceRoutine());
    }

    // 생산 중지
    private void StopProduce()
    {
        if (_produceRoutine != null)
        {
            StopCoroutine(_produceRoutine);
            _produceRoutine = null;
        }
    }

    // 씬 전환 시 생산 시작/중지
    private void OnActiveSceneChanged(Scene prev, Scene next)
    {
        if (next.name == "Settlement")
            StopProduce();
        else
            StartProduce();
    }
}
