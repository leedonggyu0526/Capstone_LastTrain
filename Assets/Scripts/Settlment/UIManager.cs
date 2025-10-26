using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("공통 UI 요소")]
    public Button closeButton;
    
    [Header("애니메이션 설정")]
    [Range(0.1f, 2f)]
    public float typingSpeed = 0.01f; // 타이핑 애니메이션 속도
    [Range(0.1f, 1f)]
    public float fadeSpeed = 0.5f; // 페이드 애니메이션 속도
    public bool enableTypingAnimation = true; // 타이핑 애니메이션 활성화
    
    // 코루틴 관리
    protected Coroutine typingCoroutine;
    protected Coroutine fadeCoroutine;
    protected bool isTypingAnimation = false;

    protected virtual void Start()
    {
        // closeButton 리스너 설정
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseButtonClick);
    }
    
    protected virtual void OnDestroy()
    {
        // closeButton 리스너 정리
        if (closeButton != null) closeButton.onClick.RemoveAllListeners();
    }
    
    // 공통 close 버튼 클릭 처리
    protected virtual void OnCloseButtonClick()
    {
        // 자식 클래스에서 오버라이드 가능
    }
    
    // 공용 애니메이션 메소드들
    
    /// <summary>
    /// CanvasGroup을 페이드인 효과로 표시
    /// </summary>
    protected IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        float duration = 1f / fadeSpeed;
        
        // 시작 시 알파 보장
        canvasGroup.alpha = 0f;
        
        while (elapsedTime < duration)
        {
            // 타임스케일 영향 없이 자연스러운 경과 시간
            elapsedTime += Time.unscaledDeltaTime;
            
            // 진행도 0~1 클램프 및 부드러운 이징
            float t = Mathf.Clamp01(elapsedTime / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            
            canvasGroup.alpha = eased;
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    /// <summary>
    /// CanvasGroup을 페이드아웃 효과로 숨김
    /// </summary>
    protected IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        float duration = 1f / fadeSpeed;
        
        // 시작 시 알파 보장
        canvasGroup.alpha = 1f;
        
        while (elapsedTime < duration)
        {
            // 타임스케일 영향 없이 자연스러운 경과 시간
            elapsedTime += Time.unscaledDeltaTime;
            
            // 진행도 0~1 클램프 및 부드러운 이징
            float t = Mathf.Clamp01(elapsedTime / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            
            // 1 -> 0으로 감소
            canvasGroup.alpha = 1f - eased;
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
    }
    
    /// <summary>
    /// 텍스트를 타이핑 애니메이션으로 표시
    /// </summary>
    protected IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText)
    {
        Debug.Log($"[UIManager] TypeText 시작 - 텍스트 길이: {fullText.Length}, 속도: {typingSpeed}, 예상 시간: {fullText.Length * typingSpeed}초");
        Debug.Log($"[UIManager] Time.timeScale: {Time.timeScale}, textComponent null: {textComponent == null}");
        
        isTypingAnimation = true;
        textComponent.text = "";
        
        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i);
            // Time.timeScale의 영향을 받지 않도록 WaitForSecondsRealtime 사용
            yield return new WaitForSecondsRealtime(typingSpeed);
            
            // 10글자마다 진행 상황 로그
            if (i % 10 == 0)
            {
                Debug.Log($"[UIManager] TypeText 진행 중: {i}/{fullText.Length}");
            }
        }
        
        isTypingAnimation = false;
        Debug.Log("[UIManager] TypeText 완료");
    }
    
    /// <summary>
    /// 이미지 전용 페이드인 애니메이션
    /// </summary>
    protected IEnumerator FadeInImage(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        float duration = 1f / fadeSpeed;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    /// <summary>
    /// 모든 애니메이션 코루틴 중지
    /// </summary>
    protected virtual void StopAllAnimations()
    {
        // 개별 코루틴 중지
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        
        // 타이핑 애니메이션 상태 리셋
        isTypingAnimation = false;
    }
    
    /// <summary>
    /// 패널을 안전하게 활성화/비활성화
    /// </summary>
    protected void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
    
    /// <summary>
    /// 버튼을 안전하게 활성화/비활성화
    /// </summary>
    protected void SetButtonActive(Button button, bool active)
    {
        if (button != null)
        {
            button.gameObject.SetActive(active);
        }
    }
    
    /// <summary>
    /// 타이핑 애니메이션 활성화/비활성화 토글
    /// </summary>
    public void ToggleTypingAnimation()
    {
        enableTypingAnimation = !enableTypingAnimation;
        Debug.Log($"타이핑 애니메이션: {(enableTypingAnimation ? "활성화" : "비활성화")}");
    }
}
