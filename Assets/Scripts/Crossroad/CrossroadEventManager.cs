using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CrossroadEventManager : MonoBehaviour
{
    private static CrossroadEventManager _instance;
    private bool _initialized = false;
    private readonly List<CrossRoadOption> allOptions = new List<CrossRoadOption>();
    public TextAsset crossroadSelect;      // 비워두면 Resources.Load로 로딩

    // 외부에서 접근하기 위한 싱글톤/읽기 전용 컬렉션 노출
    public static CrossroadEventManager Instance => _instance;
    public List<CrossRoadOption> AllOptions => allOptions;

    // 마지막으로 선택된 옵션 ID(씬 전환 후 재사용 가능)
    public string LastSelectedOptionId { get; private set; }

    public void SaveSelectedOptionId(string optionId)
    {
        LastSelectedOptionId = optionId;
        // 필요 시 PlayerPrefs 등 영구 저장 추가 가능
        Debug.Log($"[CrossroadEvent] 선택 ID 저장: {LastSelectedOptionId}");
    }

    // 씬 로드 시 main으로 돌아오면 저장된 ID의 배경을 적용
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(LastSelectedOptionId)) return;
        if (!scene.name.Equals("main")) return;

        var selected = allOptions.Find(o => o.id == LastSelectedOptionId);
        if (selected == null)
        {
            Debug.LogWarning($"[CrossroadEvent] 저장된 ID를 찾을 수 없습니다: {LastSelectedOptionId}");
            return;
        }

        var bgScroll = Object.FindObjectOfType<BackgroundScrolling>();
        if (bgScroll == null)
        {
            Debug.LogWarning("[CrossroadEvent] BackgroundScrolling 오브젝트를 main 씬에서 찾지 못했습니다.");
            return;
        }

        var sprite = selected.sceneBg != null ? selected.sceneBg : selected.bg;
        if (sprite == null)
        {
            Debug.LogWarning($"[CrossroadEvent] 선택된 옵션의 스프라이트가 없습니다: {LastSelectedOptionId}");
            return;
        }

        bgScroll.SetBackground(sprite);
        Debug.Log($"[CrossroadEvent] main 씬에 배경 적용: {sprite.name}");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // 싱글톤 보장: 씬 전환 시 중복 방지
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCSV();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

	private void OnDestroy()
	{
		if (_instance == this)
			SceneManager.sceneLoaded -= OnSceneLoaded;
	}

    // -------- CSV 로딩/파싱 --------
    private void LoadCSV()
    {
        if (_initialized) return;

        allOptions.Clear();

        if (crossroadSelect == null)
        {
            // 인스펙터 미할당 시 Resources에서 자동 로드 (Assets/Resources/CrossRoadSelect.csv)
            crossroadSelect = Resources.Load<TextAsset>("CrossRoadSelect");
            if (crossroadSelect == null)
            {
                Debug.LogWarning("[CrossroadEvent] csvAsset이 없습니다. Assets/Resources/CrossRoadSelect.csv 확인!");
                return;
            }
        }

        var lines = crossroadSelect.text.Split('\n');
        if (lines.Length == 0)
        {
            Debug.LogWarning("[CrossroadUI] CSV가 비어 있습니다.");
            return;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string rawLine = lines[i];
            if (string.IsNullOrWhiteSpace(rawLine)) continue; // 빈 줄 스킵(마지막 개행 등)

            // 윈도우 개행의 CR 제거 및 트림
            string line = rawLine.Trim();
            if (line.EndsWith("\r")) line = line.Substring(0, line.Length - 1);

            string[] values = line.Split(',');
            if (values.Length < 4)
            {
                Debug.LogWarning($"[CrossroadEvent] CSV 열 개수가 부족합니다(line {i}): '{line}'");
                continue;
            }

            string id = values[0].Trim();
            string title = values[1].Trim();
            string desc = values[2].Trim();
            string weightStr = values[3].Trim();

            int weight = 1;
            if (!int.TryParse(weightStr, out weight))
            {
                Debug.LogWarning($"[CrossroadEvent] weight 파싱 실패(line {i}): '{weightStr}', 기본값 1 사용");
                weight = 1;
            }

            // 디버그(필요 시 주석 해제)
            // Debug.Log($"[CrossroadUI] values: {id}, {title}, {desc}, {weight}");

            CrossRoadOption option = new CrossRoadOption();
            option.id = id;
            option.title = title;
            option.desc = desc;
            option.weight = Mathf.Max(1, weight);
            option.SetSprites();
            allOptions.Add(option);
        }

        Debug.Log($"[CrossroadEvent] CSV 로드 완료: {allOptions.Count}개");

        if(allOptions.Count > 0)
        {
            _initialized = true;
        }
    }
}
