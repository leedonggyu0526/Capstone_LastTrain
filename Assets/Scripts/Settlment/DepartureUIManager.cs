using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class DepartureUIManager : UIManager
{
    [Header("출발 UI 요소")]
    public GameObject departurePanel;
    public Button departureButton;
    public TextMeshProUGUI departureText;

    // 자동으로 찾은 CanvasGroup
    private CanvasGroup departureCanvasGroup;
    
    protected override void Start()
    {
        base.Start(); // UIManager의 Start 호출
        
        // CanvasGroup 자동 찾기
        FindCanvasGroup();
        
        // 출발 버튼 이벤트 리스너 설정
        if (departureButton != null)
        {
            departureButton.onClick.RemoveAllListeners();
            departureButton.onClick.AddListener(OnDepartureButtonClick);
        }
    }
    
    /// <summary>
    /// CanvasGroup 자동 찾기
    /// </summary>
    private void FindCanvasGroup()
    {
        if (departurePanel != null)
        {
            // departurePanel에서 CanvasGroup 찾기
            departureCanvasGroup = departurePanel.GetComponent<CanvasGroup>();
            
            // 없으면 자동으로 추가
            if (departureCanvasGroup == null)
            {
                departureCanvasGroup = departurePanel.AddComponent<CanvasGroup>();
            }
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy(); // UIManager의 OnDestroy 호출
        
        // 출발 관련 정리
        StopAllAnimations();
    }
    
    protected override void OnCloseButtonClick()
    {
        // 출발 패널 닫기
        HideDeparturePanel();
    }
    
    /// <summary>
    /// 출발 패널 표시
    /// </summary>
    public void ShowDeparturePanel()
    {
        if (departurePanel != null)
        {
            departurePanel.SetActive(true);
            
            // 출발 텍스트 설정
            if (departureText != null)
            {
                SetDepartureText();
            }
            
            // 페이드인 애니메이션
            if (departureCanvasGroup != null)
            {
                departureCanvasGroup.alpha = 0f; // 알파값 초기화
                StopAllAnimations();
                fadeCoroutine = StartCoroutine(FadeIn(departureCanvasGroup));
            }
        }
    }
    
    /// <summary>
    /// 출발 패널 숨기기
    /// </summary>
    public void HideDeparturePanel()
    {
        if (departurePanel != null)
        {
            // 페이드아웃 애니메이션
            if (departureCanvasGroup != null)
            {
                StopAllAnimations();
                StartCoroutine(DeactivateAfterFade());
                Debug.Log("[DepartureUIManager] 출발 패널 숨기기");
            }
            else
            {
                departurePanel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 페이드아웃 후 패널 비활성화
    /// </summary>
    private IEnumerator DeactivateAfterFade()
    {
        yield return FadeOut(departureCanvasGroup);
        if (departurePanel != null)
        {
            departurePanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 출발 버튼 클릭 처리
    /// </summary>
    private void OnDepartureButtonClick()
    {
        // main 씬으로 이동
        Debug.Log("[DepartureUIManager] 출발 버튼 클릭됨: 'main' 씬 로드 시도");
        SceneManager.LoadScene("main");
    }
    
    /// <summary>
    /// 출발 텍스트 설정
    /// </summary>
    public void SetDepartureText()
    {
        if (departureText != null)
        {
            departureText.text = "정말로 출발하시겠습니까?\n\n" +
                    "현재 정착지에서 나가면\n" +
                    "다시 돌아올 수 없습니다.";
                departureText.color = Color.black;
        }
    }
}
