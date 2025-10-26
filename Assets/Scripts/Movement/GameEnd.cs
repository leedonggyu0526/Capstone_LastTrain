using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameEnd : MonoBehaviour
{
    [Header("Refs")]
    public GameObject gameEndUI;
    public TMP_Text playTimeText;
    private float playTime = 0f;
    private bool isGameEnded = false;
    public 

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
    }

    private float debugLogTimer = 0f;
    
    public void CheckGameEnd()
    {
        gameEndUI.SetActive(true);
        playTimeText.text = $"생존 시간: {playTime:F2}초";
        isGameEnded = true;
    }

    public void ResetGame()
    {
        // 시간 재개
        Time.timeScale = 1f;
        
        Debug.Log("[GameEnd] 게임 재시작");
        SceneManager.LoadScene("StartScene");
    }
}
