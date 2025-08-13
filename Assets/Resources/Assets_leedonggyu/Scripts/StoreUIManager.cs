using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StoreUIManager : UIManager
{
    [Header("상점 UI 요소")]
    public GameObject storePanel;
    
    [Header("상점 아이템 UI")]
    public Button[] itemButtons;
    public TextMeshProUGUI[] itemNames;
    public TextMeshProUGUI[] itemPrices;
    public TextMeshProUGUI[] itemDescriptions;
    
    [Header("상점 정보")]
    public TextMeshProUGUI storeTitle;
    public TextMeshProUGUI playerMoneyText;
    
    [Header("상점 설정")]
    public bool showStoreOnStart = false;
    
    // 자동으로 찾은 CanvasGroup
    private CanvasGroup storeCanvasGroup;
    
    protected override void Start()
    {
        base.Start(); // UIManager의 Start 호출
        
        // CanvasGroup 자동 찾기
        FindCanvasGroup();
        
        // 상점 초기화
        InitializeStore();
        
        // 시작 시 상점 표시 여부
        if (showStoreOnStart)
        {
            ShowStore();
        }
        else
        {
            HideStore();
        }
    }
    
    /// <summary>
    /// CanvasGroup 자동 찾기
    /// </summary>
    private void FindCanvasGroup()
    {
        if (storePanel != null)
        {
            // storePanel에서 CanvasGroup 찾기
            storeCanvasGroup = storePanel.GetComponent<CanvasGroup>();
            
            // 없으면 자동으로 추가
            if (storeCanvasGroup == null)
            {
                storeCanvasGroup = storePanel.AddComponent<CanvasGroup>();
            }
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy(); // UIManager의 OnDestroy 호출
        
        // 상점 관련 정리
        StopAllAnimations();
    }
    
    protected override void OnCloseButtonClick()
    {
        // 상점 닫기
        HideStore();
    }
    
    /// <summary>
    /// 상점 초기화
    /// </summary>
    private void InitializeStore()
    {
        // 상점 제목 설정
        if (storeTitle != null)
        {
            storeTitle.text = "상점";
        }
        
        // 아이템 버튼 이벤트 설정
        for (int i = 0; i < itemButtons.Length; i++)
        {
            int itemIndex = i; // 클로저를 위한 변수
            if (itemButtons[i] != null)
            {
                itemButtons[i].onClick.AddListener(() => OnItemButtonClick(itemIndex));
            }
        }
        
        // 초기 아이템 정보 설정
        UpdateItemInfo();
        
        // 플레이어 돈 업데이트
        UpdatePlayerMoney();
    }
    
    /// <summary>
    /// 상점 표시
    /// </summary>
    public void ShowStore()
    {
        if (storePanel != null)
        {
            storePanel.SetActive(true);
            
            // 페이드인 애니메이션
            if (storeCanvasGroup != null)
            {
                fadeCoroutine = StartCoroutine(FadeIn(storeCanvasGroup));
            }
            
            // 아이템 정보 업데이트
            UpdateItemInfo();
            UpdatePlayerMoney();
        }
    }
    
    /// <summary>
    /// 상점 숨기기
    /// </summary>
    public void HideStore()
    {
        if (storePanel != null)
        {
            // 페이드아웃 애니메이션
            if (storeCanvasGroup != null)
            {
                fadeCoroutine = StartCoroutine(FadeOut(storeCanvasGroup));
                StartCoroutine(DeactivateAfterFade());
            }
            else
            {
                storePanel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 페이드아웃 후 패널 비활성화
    /// </summary>
    private IEnumerator DeactivateAfterFade()
    {
        yield return new WaitForSeconds(1f / fadeSpeed);
        
        if (storePanel != null)
        {
            storePanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 아이템 버튼 클릭 처리
    /// </summary>
    private void OnItemButtonClick(int itemIndex)
    {
        // 여기에 아이템 구매 로직 추가
        // 예: ResourceManager에서 돈 차감, 아이템 추가 등
        
        // 구매 후 UI 업데이트
        UpdatePlayerMoney();
    }
    
    /// <summary>
    /// 아이템 정보 업데이트
    /// </summary>
    private void UpdateItemInfo()
    {
        // 예시 아이템 데이터 (실제로는 데이터베이스나 ScriptableObject에서 가져옴)
        string[] itemNames = { "식량", "연료", "기계부품", "의료품" };
        int[] itemPrices = { 100, 200, 300, 400 };
        string[] itemDescriptions = { "기차 승객들의 식량", "기차 연료", "기차 수리 부품", "의료용품" };
        
        for (int i = 0; i < this.itemNames.Length && i < itemNames.Length; i++)
        {
            if (this.itemNames[i] != null)
            {
                this.itemNames[i].text = itemNames[i];
            }
            
            if (this.itemPrices[i] != null)
            {
                this.itemPrices[i].text = $"{itemPrices[i]}원";
            }
            
            if (this.itemDescriptions[i] != null)
            {
                this.itemDescriptions[i].text = itemDescriptions[i];
            }
        }
    }
    
    /// <summary>
    /// 플레이어 돈 업데이트
    /// </summary>
    private void UpdatePlayerMoney()
    {
        if (playerMoneyText != null)
        {
            // ResourceManager에서 돈 정보 가져오기
            if (ResourceManager.Instance != null)
            {
                int money = ResourceManager.Instance.GetResource(ResourceType.Fuel); // 예시로 연료를 돈으로 사용
                playerMoneyText.text = $"보유 금액: {money}원";
            }
            else
            {
                playerMoneyText.text = "보유 금액: 0원";
            }
        }
    }
    
    /// <summary>
    /// 상점 토글 (표시/숨김)
    /// </summary>
    public void ToggleStore()
    {
        if (storePanel != null && storePanel.activeSelf)
        {
            HideStore();
        }
        else
        {
            ShowStore();
        }
    }
}
