using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private void Awake()
    {
        // 인스펙터에 넣지 않았다면 Resources에서 찾음
        if (csvAsset == null)
            csvAsset = Resources.Load<TextAsset>("CrossRoad/CrossRoadSelect"); // 경로: Assets/Resources/CrossRoad/CrossRoadSelect.csv

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
        int count = Mathf.Clamp(Random.Range(minCount, maxCount + 1), 1, 10);
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
        if (lines.Length <= 1)
        {
            Debug.LogWarning("[CrossroadUI] CSV가 비어있거나 헤더만 있습니다.");
            return;
        }

        // 첫 줄 헤더 스킵
        for (int i = 1; i < lines.Length; i++)
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
        return Resources.Load<Sprite>(resPath);
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

            int r = Random.Range(1, totalWeight + 1);
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
