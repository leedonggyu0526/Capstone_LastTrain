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
    public float correction;      // 생산 조정 값
    public float multiplier;     // 생산 조정 값 계산을 위한 변수
}

public class ResourceProducer : MonoBehaviour
{
    [Header("자원 생산")]
    public float productionInterval = 5f;   // 생산 간격(초)
    public List<ResourceProduction> productions = new List<ResourceProduction>();

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
                // 최종생산량 : 기본 * 멀티플러 * 콜렉션
                int finalAmount = (int)(Mathf.Max(0, production.amountPerCycle * production.multiplier * production.correction));
                if (finalAmount == 0) continue;
                ResourceManager.Instance.AddResource(production.type, finalAmount);
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

    public void SetMultiplier(ResourceType type, float value)
    {
        for (int i = 0; i < productions.Count; i++)
        {
            if (productions[i].type == type)
            {
                var prod = productions[i];
                prod.multiplier = value;
                productions[i] = prod;
                return;
            }
        }
    }

    /// <summary>
    ///  생산 코루틴 보정값(Correction) 증가
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void AddCorrection(ResourceType type, float value)
    {
        for (int i = 0; i < productions.Count; i++)
        {
            if (productions[i].type == type)
            {
                var prod = productions[i];
                prod.correction += value;
                productions[i] = prod;
                return;
            }
        }
    }

    public void SetCorrection(ResourceType type, float value)
    {
        for (int i = 0; i < productions.Count; i++)
        {
            if (productions[i].type == type)
            {
                var prod = productions[i];
                prod.correction = value;
                productions[i] = prod;
                return;
            }
        }
    }
}
