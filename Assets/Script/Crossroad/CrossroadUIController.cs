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

    [Header("CSV")]
    public TextAsset crossroadSelect;      // 비워두면 Resources.Load로 로딩
    private readonly List<CrossRoadOption> allOptions = new List<CrossRoadOption>();

    [Header("Spawn Count")]
    public int minCount = 2;
    public int maxCount = 3;

    private bool _initialized = false;

    private void Start()
    {
        // 첫 프레임에 CSV 미리 로드(실패해도 Show에서 재시도함)
        LoadCSV();
        _initialized = true;

        // ⭐ 안전하게 패널만 꺼두기(씬에서 켜놔도 여기서 한 번 꺼주니 상태 일관)
        if (panel != null && panel.activeSelf)
            panel.SetActive(false);
    }

    private void Awake()
    {
        // panel이 비어 있으면 현재 오브젝트를 패널로 사용 (일반적으로 컨트롤러가 패널 루트에 붙어 있음)
        if (panel == null) panel = this.gameObject;

        // optionPrefab이 비어 있으면 Resources에서 시도 로드
        if (optionPrefab == null)
        {
            optionPrefab = Resources.Load<GameObject>("CrossRoad/CrossRoadSelectPrefab");
        }

        LoadCSV();
        Hide();
    }

    /// <summary>
    /// 팝업 열기 + 옵션 생성
    /// </summary>
    public void Show()
    {
        ClearChildren();

        if (allOptions.Count == 0){
            Debug.LogWarning("[CrossroadUI] CSV에서 로드된 옵션이 0개입니다.");
        }

        // 2~3개 가중치 랜덤 픽
        int count = Mathf.Clamp(UnityEngine.Random.Range(minCount, maxCount + 1), 1, 10);
        var picks = WeightedPick(allOptions, count);

        foreach (var opt in picks)
        {
            var go = Instantiate(optionPrefab, optionsParent);

            var view = go.GetComponent<CrossroadOptionView>();

            // 리소스 로드 (경로는 CSV의 iconPath/bgPath, ex: CrossRoad/Icons/crate)
            Sprite bgSprite = LoadSpriteSafe(opt.bgPath);
            Sprite iconSprite = LoadSpriteSafe(opt.iconPath);

            view.Bind(
                bgSprite,
                iconSprite,
                opt.title,
                opt.desc,
                () =>
                {
                    Sprite sceneBgSprite = LoadSpriteSafe(opt.sceneBgPath);
                    var bgScroll = FindObjectOfType<BackgroundScrolling>();
                    bgScroll.SetBackground(sceneBgSprite);
                    // TODO: 선택 결과(보상/패널티) 처리 지점
                    Debug.Log($"[Crossroad] 선택됨: {opt.id} - {opt.title}");
                    Hide(); // 선택하면 이벤트 종료
                    SceneManager.LoadScene("Settlement");
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
    }

    private void ClearChildren()
    {
        if (!optionsParent) return;
        for (int i = optionsParent.childCount - 1; i >= 0; i--)
            Destroy(optionsParent.GetChild(i).gameObject);
    }

    // -------- CSV 로딩/파싱 --------
    private void LoadCSV()
    {
        if (_initialized) return;

        allOptions.Clear();

        if (crossroadSelect == null)
        {
            Debug.LogWarning("[CrossroadUI] csvAsset이 없습니다. Assets/Resources/CrossRoad/CrossRoadSelect.csv 확인!");
            return;
        }

        var lines = crossroadSelect.text.Split('\n');
        if (lines.Length == 0)
        {
            Debug.LogWarning("[CrossroadUI] CSV가 비어 있습니다.");
            return;
        }

        // 첫 유효 라인이 헤더인지 자동 판별 (id가 정수면 데이터로 간주)
        int startIndex = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var probe = lines[i].Trim();
            if (string.IsNullOrEmpty(probe)) continue;
            var cols0 = SplitCsvFlexible(probe);
            bool firstIsHeader = true;
            if (cols0.Count > 0)
            {
                int tmpId;
                if (int.TryParse(cols0[0], out tmpId))
                    firstIsHeader = false;
            }
            startIndex = firstIsHeader ? i + 1 : i;
            break;
        }

        int added = 0;
        for (int i = startIndex; i < lines.Length; i++)
        {   
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var cols = SplitCsvFlexible(line); // 큰따옴표 지원
            
            // 4열 전용: id,title,desc,weight → 경로는 SetPath()로 자동 설정
            var opt = new CrossRoadOption();
            if (cols.Count < 4)
            {
                Debug.LogWarning($"[CrossroadUI] 라인 {i + 1}: 컬럼 수({cols.Count}) < 4 → 스킵. 원문: {line}");
                continue;
            }
            if (!int.TryParse(cols[0], out opt.id))
            {
                Debug.LogWarning($"[CrossroadUI] 라인 {i + 1}: id 파싱 실패 → 스킵. 원문: {line}");
                continue;
            }
            opt.title = cols[1];
            opt.desc = cols[2];
            if (!int.TryParse(cols[3], out opt.weight))
            {
                Debug.LogWarning($"[CrossroadUI] 라인 {i + 1}: weight 파싱 실패 → 기본값 1 사용. 원문: {line}");
                opt.weight = 1;
            }
            // 경로는 id 기반으로 자동 설정
            opt.SetPath();

            if (opt.weight < 1) opt.weight = 1;

            if (!opt.IsValid(out var reason))
            {
                Debug.LogWarning($"[CrossroadUI] 라인 {i + 1} 옵션 무시(id={opt.id}): {reason}");
                continue;
            }

            allOptions.Add(opt);
            added++;

        }

        Debug.Log($"[CrossroadUI] CSV 로드 완료: {allOptions.Count}개");
    }

    private List<string> SplitCsvFlexible(string line)
    {
        bool hasQuote = line.IndexOf('\"') >= 0;
        if (hasQuote)
            return SplitCsvQuoted(line);

        if (line.IndexOf('\t') >= 0) return new List<string>(line.Split('\t'));
        if (line.IndexOf(';') >= 0) return new List<string>(line.Split(';'));
        return new List<string>(line.Split(',')); // 기본 콤마
    }

    private List<string> SplitCsvQuoted(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '\"')
            {
                // "" → " 로 처리
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')
                {
                    sb.Append('\"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (!inQuotes && (c == ',' || c == ';' || c == '\t'))
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }
        result.Add(sb.ToString());
        return result;
    }

    // -------- 유틸 --------
    private Sprite LoadSpriteSafe(string resPath)
    {
        if (string.IsNullOrWhiteSpace(resPath)) return null;
        string cleaned = resPath;
        Sprite s = Resources.Load<Sprite>(cleaned);
        if (s == null)
        {
            Debug.LogWarning($"[CrossroadUI] Sprite 로드 실패: '{resPath}' → 변환 '{cleaned}'");
        }
        return s;
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
