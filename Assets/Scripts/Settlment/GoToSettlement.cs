using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoToSettlement : MonoBehaviour
{
    [Header("UI 설정")]
    public Button settlementButton;
    
    [Header("Train 오브젝트들")]
    public GameObject trainObject; // Inspector에서 Train 오브젝트들을 할당
    
    void Start()
    {
        settlementButton.onClick.AddListener(LoadSettlementScene);
    }

    public void LoadSettlementScene()
    {
        // 씬 전환 전에 Train 객체들 숨기기
        HideTrainObject();
        
        // Settlement 씬으로 이동
        SceneManager.LoadScene("Settlement");
    }
    
    // Train 객체들 숨기기
    private void HideTrainObject()
    {
        if (trainObject != null)
        {
            trainObject.SetActive(false);
            Debug.Log($"Train 객체 '{trainObject.name}' 숨김");
        }
    }
    
    // Train 객체들 보이기 (필요시 사용)
    public void ShowTrainObject()
    {
        if (trainObject != null)
        {
            trainObject.SetActive(true);
            Debug.Log($"Train 객체 '{trainObject.name}' 표시");
        }
    }
}
