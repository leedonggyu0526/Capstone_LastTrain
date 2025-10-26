using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameEnd : MonoBehaviour
{
    [Header("Refs")]
    public ResourceManager resourceManager;
    public GameObject gameEndUI;
    public TMP_Text playTimeText;
    private float playTime = 0f;
    private bool isGameEnded = false;

    void OnEnable()
    {
        Debug.Log($"[GameEnd] OnEnable 호출 - GameObject: {gameObject.name}, enabled: {enabled}");
    }
    
    void OnDisable()
    {
        Debug.Log($"[GameEnd] OnDisable 호출 - GameObject: {gameObject.name}");
    }
    
    void Start()
    {
        playTime = 0f;
        isGameEnded = false;
        
        Debug.Log($"[GameEnd] Start 실행 - GameObject: {gameObject.name}, activeInHierarchy: {gameObject.activeInHierarchy}, enabled: {enabled}");
        
        if (gameEndUI != null)
        {
            gameEndUI.SetActive(false);
            Debug.Log("[GameEnd] gameEndUI 비활성화 완료");
        }
        else
        {
            Debug.LogError("[GameEnd] gameEndUI가 연결되지 않았습니다!");
        }
            
        if (resourceManager == null)
        {
            resourceManager = ResourceManager.Instance;
            if (resourceManager == null)
                Debug.LogError("[GameEnd] ResourceManager를 찾을 수 없습니다!");
            else
                Debug.Log($"[GameEnd] ResourceManager.Instance 찾음, 현재 내구도: {resourceManager.GetResource(ResourceType.Durability)}");
        }
        else
        {
            Debug.Log($"[GameEnd] resourceManager 연결됨, 현재 내구도: {resourceManager.GetResource(ResourceType.Durability)}");
        }
        
        Debug.Log($"[GameEnd] Start 완료 - Time.timeScale: {Time.timeScale}");
    }

    private float updateLogTimer = 0f;
    
    // Update is called once per frame
    void Update()
    {
        // Update 호출 확인 (5초마다 로그)
        updateLogTimer += Time.deltaTime;
        if (updateLogTimer >= 5f)
        {
            Debug.Log($"[GameEnd] Update 실행 중... Time.timeScale: {Time.timeScale}, isGameEnded: {isGameEnded}");
            updateLogTimer = 0f;
        }
        
        // 게임이 이미 종료되었으면 더 이상 체크하지 않음
        if (isGameEnded) return;
        
        playTime += Time.deltaTime;
        CheckGameEnd();
    }

    private float debugLogTimer = 0f;
    
    public void CheckGameEnd()
    {
        // 게임이 이미 종료되었으면 더 이상 체크하지 않음
        if (isGameEnded) return;
        
        // resourceManager null 체크
        if (resourceManager == null)
        {
            Debug.LogError("[GameEnd] resourceManager가 null입니다!");
            return;
        }
        
        int durability = resourceManager.GetResource(ResourceType.Durability);
        
        // 1초마다 내구도 로그 (디버그용)
        debugLogTimer += Time.deltaTime;
        if (debugLogTimer >= 1f)
        {
            Debug.Log($"[GameEnd] 현재 내구도: {durability}");
            debugLogTimer = 0f;
        }
        
        if (durability <= 0)
        {
            Debug.Log($"[GameEnd] ★★★ 게임 종료 조건 충족! 내구도: {durability} ★★★");
            isGameEnded = true;
            
            // 시간 정지
            Debug.Log("[GameEnd] Time.timeScale을 0으로 설정");
            Time.timeScale = 0f;
            
            // 게임 종료 UI 표시
            if (gameEndUI != null)
            {
                Debug.Log("[GameEnd] gameEndUI 활성화");
                gameEndUI.SetActive(true);
                Debug.Log($"[GameEnd] gameEndUI.activeSelf: {gameEndUI.activeSelf}");
            }
            else
            {
                Debug.LogError("[GameEnd] gameEndUI가 null입니다!");
            }

            // 플레이 시간 표시
            if (playTimeText != null)
            {
                int minutes = Mathf.FloorToInt(playTime / 60f);
                int seconds = Mathf.FloorToInt(playTime % 60f);
                string timeText = $"생존 시간: {minutes}분 {seconds}초";
                playTimeText.text = timeText;
                Debug.Log($"[GameEnd] 플레이 시간 설정: {timeText}");
            }
            else
            {
                Debug.LogError("[GameEnd] playTimeText가 null입니다!");
            }
            
            Debug.Log("[GameEnd] 게임 종료 처리 완료");
        }
    }

    public void ResetGame()
    {
        // 시간 재개
        Time.timeScale = 1f;
        
        Debug.Log("[GameEnd] 게임 재시작");
        SceneManager.LoadScene("StartScene");
    }
}
