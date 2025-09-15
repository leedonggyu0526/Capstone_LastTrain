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
    public TextAsset csvAsset;      // 비워두면 Resources.Load로 로딩
    private readonly List<CrossRoadOption> allOptions = new List<CrossRoadOption>();

    [Header("Spawn Count")]
    public int minCount = 2;
    public int maxCount = 3;

    [Header("UnDestroyOnLoad")]
    public GameObject trainObject;

    private void Awake()
    {
        // 인스펙터에 넣지 않았다면 Resources에서 찾음
        if (csvAsset == null)
        {
            csvAsset = Resources.Load<TextAsset>("CrossRoad/CrossRoadSelect"); // 기본 경로: Assets/Resources/CrossRoad/CrossRoadSelect.csv
            if (csvAsset == null)
            {
                // 폴백: Assets/Resources/CrossRoadSelect.csv 도 지원
                csvAsset = Resources.Load<TextAsset>("CrossRoadSelect");
                if (csvAsset != null)
                    Debug.LogWarning("[CrossroadUI] CSV를 폴백 경로(CrossRoadSelect)에서 로드했습니다. 폴더 구조를 CrossRoad/로 정리하는 것을 권장합니다.");
            }
        }

        // panel이 비어 있으면 현재 오브젝트를 패널로 사용 (일반적으로 컨트롤러가 패널 루트에 붙어 있음)
        if (panel == null) panel = this.gameObject;

        // optionPrefab이 비어 있으면 Resources에서 시도 로드
        if (optionPrefab == null)
        {
            optionPrefab = Resources.Load<GameObject>("CrossRoad/CrossRoadSelectPrefab");
            if (optionPrefab == null)
                Debug.LogError("[CrossroadUI] optionPrefab이 비었고 Resources/CrossRoad/CrossRoadSelectPrefab 로드에도 실패했습니다. 인스펙터에 프리팹을 연결하세요.");
        }

        LoadCSV();
        HideImmediate();
    }

    /// <summary>
    /// 팝업 열기 + 옵션 생성
    /// </summary>
    public void Show()
    {
        if (panel == null) { Debug.LogError("[CrossroadUIController] panel=NULL"); return; }
        if (optionsParent == null) { Debug.LogError("[CrossroadUIController] optionsParent=NULL"); return; }
        if (optionPrefab == null) { Debug.LogError("[CrossroadUIController] optionPrefab=NULL(프로젝트 프리팹 에셋 연결 필요)"); return; }
        if (allOptions.Count == 0) { Debug.LogWarning("[CrossroadUIController] CSV에서 로드된 옵션이 0개입니다."); }

        Debug.Log("[CrossroadUIController] Show() 호출됨 → 카드 생성 시작");
        if (!ValidateRefs()) return;

        ClearChildren();

        // 2~3개 가중치 랜덤 픽
        int count = Mathf.Clamp(UnityEngine.Random.Range(minCount, maxCount + 1), 1, 10);
        var picks = WeightedPick(allOptions, count);

        foreach (var opt in picks)
        {
            var go = Instantiate(optionPrefab, optionsParent);

            var view = go.GetComponent<CrossroadOptionView>();
            if (view == null)
            {
                Debug.LogError("[CrossroadUI] optionPrefab에 CrossroadOptionView가 없습니다.");
                continue;
            }

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

    private void HideImmediate()
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

    private bool ValidateRefs()
    {
        bool ok = true;
        if (!panel) { Debug.LogError("[CrossroadUI] panel이 비었습니다."); ok = false; }
        if (!optionsParent) { Debug.LogError("[CrossroadUI] optionsParent가 비었습니다."); ok = false; }
        if (!optionPrefab) { Debug.LogError("[CrossroadUI] optionPrefab이 비었습니다."); ok = false; }
        if (allOptions.Count == 0) { Debug.LogWarning("[CrossroadUI] CSV에서 로드된 옵션이 0개입니다."); }
        return ok;
    }

    // -------- CSV 로딩/파싱 --------
    private void LoadCSV()
    {
        allOptions.Clear();

        if (csvAsset == null)
        {
            Debug.LogWarning("[CrossroadUI] csvAsset이 없습니다. Assets/Resources/CrossRoad/CrossRoadSelect.csv 확인!");
            return;
        }

        var lines = csvAsset.text.Split('\n');
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
            var cols0 = SplitCsv(probe);
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

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var cols = SplitCsv(line); // 큰따옴표 지원
            // 기대: id,title,desc,iconPath,bgPath,weight = 6개
            if (cols.Count < 6) continue;

            var opt = new CrossRoadOption();
            int.TryParse(cols[0], out opt.id);
            opt.title = cols[1];
            opt.desc = cols[2];
            opt.iconPath = cols[3];
            opt.bgPath = cols[4];
            int.TryParse(cols[5], out opt.weight);
            if (opt.weight < 1) opt.weight = 1;

            // (선택) 유효성 검사
            if (!opt.IsValid(out var reason))
            {
                Debug.LogWarning($"[CrossroadUI] 옵션 무시(id={opt.id}): {reason}");
                continue;
            }

            allOptions.Add(opt);
        }

        Debug.Log($"[CrossroadUI] CSV 로드 완료: {allOptions.Count}개");
    }

    // 큰따옴표("...") 내의 쉼표를 보존하는 간이 파서
    private List<string> SplitCsv(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '\"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }
        result.Add(sb.ToString()); // 마지막 필드
        return result;
    }

    // -------- 유틸 --------
    private Sprite LoadSpriteSafe(string resPath)
    {
        if (string.IsNullOrWhiteSpace(resPath)) return null;
        string cleaned = SanitizeResourcePath(resPath);
        Sprite s = Resources.Load<Sprite>(cleaned);
        if (s == null)
        {
            Debug.LogWarning($"[CrossroadUI] Sprite 로드 실패: '{resPath}' → 변환 '{cleaned}'");
        }
        return s;
    }

    private string SanitizeResourcePath(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        string p = input.Trim();

        // 경로 구분자 통일
        p = p.Replace("\\", "/");

        // 선행 Assets/ 제거
        if (p.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            p = p.Substring("Assets/".Length);

        // 선행 Resources/ 제거 (Resources 상대 경로로 맞춤)
        if (p.StartsWith("Resources/", StringComparison.OrdinalIgnoreCase))
            p = p.Substring("Resources/".Length);

        // 파일 확장자 제거
        int dot = p.LastIndexOf('.');
        if (dot >= 0) p = p.Substring(0, dot);

        return p;
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
