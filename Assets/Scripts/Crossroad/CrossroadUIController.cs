// CrossroadUIController.cs (최종 수정 버전 - FindObjectOfType 오류 해결)

using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CrossroadUIController : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject panel;        // 전체 팝업 (처음 비활성)
    public Transform optionsParent; // 그리드/리스트 부모(OptionsParent)
    public GameObject optionPrefab; // CrossRoadSelectPrefab (루트에 Button + CrossroadOptionView)

    [Header("Spawn Count")]
    public int minCount = 2;
    public int maxCount = 3;

    private void Start()
    {
        // ⭐ 안전하게 패널만 꺼두기(씬에서 켜놔도 여기서 한 번 꺼주니 상태 일관)
        if (panel != null && panel.activeSelf)
            panel.SetActive(false);
            
        Show();
    }

    private void Awake()
    {
        // panel이 비어 있으면 현재 오브젝트를 패널로 사용 (일반적으로 컨트롤러가 패널 루트에 붙어 있음)
        if (panel == null) panel = this.gameObject;

        // optionPrefab이 비어 있으면 Resources에서 시도 로드
        if (optionPrefab == null)
        {
            optionPrefab = Resources.Load<GameObject>("Prefabs/UI/CrossRoadSelectPrefab");
            //  수정 사항 : CrossRoadSelectPrefab 파일의 위치를 Prefabs/UI 폴더로 변경
        }

        Hide();
    }

    /// <summary>
    /// 팝업 열기 + 옵션 생성
    /// </summary>
    public void Show()
    {
        ClearChildren();

        if (CrossroadEventManager.Instance == null || CrossroadEventManager.Instance.AllOptions.Count == 0)
        {
            Debug.LogWarning("[CrossroadUI] 옵션이 0개입니다.");
        }

        // 게임 일시 정지
        Time.timeScale = 0f;

        // 2~3개 가중치 랜덤 픽
        int count = Mathf.Clamp(UnityEngine.Random.Range(minCount, maxCount + 1), 1, 10);
        var source = CrossroadEventManager.Instance != null ? CrossroadEventManager.Instance.AllOptions : null;
        var picks = WeightedPick(source, count);

        foreach (var opt in picks)
        {
            var go = Instantiate(optionPrefab, optionsParent);

            var view = go.GetComponent<CrossroadOptionView>();

            view.Bind(
                opt.bg,
                opt.icon,
                opt.title,
                opt.desc,
                () =>
                {
                    // 선택된 옵션 ID 저장 (씬 전환 전에 상태 보존)
                    if (CrossroadEventManager.Instance != null)
                        CrossroadEventManager.Instance.SaveSelectedOptionId(opt.id);

                    // [수정] API Deprecation 해결: FindAnyObjectByType 사용
                    // CrossroadEventTrigger를 찾아 비활성화하여 루프 방지
                    var trigger = FindAnyObjectByType<CrossroadEventTrigger>();
                    if (trigger != null)
                    {
                        trigger.enabled = false;
                        Debug.Log("[Crossroad] 이벤트 트리거 컴포넌트 비활성화 (루프 방지)");
                    }
                    else // CrossroadEventTrigger가 없으면 CrossroadUI를 확인
                    {
                        var uiTrigger = FindAnyObjectByType<CrossroadUI>();
                        if (uiTrigger != null)
                        {
                            uiTrigger.enabled = false;
                            Debug.Log("[Crossroad] CrossroadUI 컴포넌트 비활성화 (루프 방지)");
                        }
                    }

                    // TODO: 선택 결과(보상/패널티) 처리 지점
                    Debug.Log($"[Crossroad] 선택됨: {opt.id} - {opt.title}");

                    Hide(); // (Time.timeScale = 1f 복구 포함)

                    //SceneManager.LoadScene("Settlement");
                }
            );
        }

        panel.SetActive(true);
    }

    /// <summary>팝업 닫기 + 자식 정리</summary>
    public void Hide()
    {
        if (panel) panel.SetActive(false);
        ClearChildren();

        // 게임 재개
        Time.timeScale = 1f;
    }

    private void ClearChildren()
    {
        if (!optionsParent) return;
        for (int i = optionsParent.childCount - 1; i >= 0; i--)
            Destroy(optionsParent.GetChild(i).gameObject);
    }

    // 가중치 랜덤, 중복 없이 N개 뽑기
    private List<CrossRoadOption> WeightedPick(List<CrossRoadOption> source, int count)
    {
        List<CrossRoadOption> picks = new List<CrossRoadOption>();
        if (source == null || source.Count == 0) return picks;

        List<CrossRoadOption> pool = new List<CrossRoadOption>(source);

        for (int n = 0; n < count && pool.Count > 0; n++)
        {
            int totalWeight = 0;
            foreach (var o in pool) totalWeight += Mathf.Max(1, o.weight);

            int r = UnityEngine.Random.Range(1, totalWeight + 1);
            int acc = 0;
            CrossRoadOption chosen = pool[0];

            foreach (var o in pool)
            {
                acc += Mathf.Max(1, o.weight);
                if (r <= acc) { chosen = o; break; }
            }

            picks.Add(chosen);
            pool.Remove(chosen); // 중복 방지
        }

        return picks;
    }
}