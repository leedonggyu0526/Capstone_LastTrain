// Assets/Scripts/UpgradeButtonManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// - 오브젝트 클릭 시 업그레이드 버튼을 켜고,
/// - 버튼 클릭 시 TrainKhan.Upgrade() 호출,
/// - 다른 곳 클릭 시 버튼 숨김.
/// </summary>
public class UpgradeButtonManager : MonoBehaviour, IPointerClickHandler
{
    [Header("인스펙터에서 버튼 UI를 연결")]
    public GameObject upgradeButtonUI;   // 업그레이드 버튼 GameObject

    private TrainKhan _trainKhan;        // 같은 오브젝트의 TrainKhan 참조

    void Awake()
    {
        _trainKhan = GetComponent<TrainKhan>();
        if (upgradeButtonUI != null)
            upgradeButtonUI.SetActive(false); // 시작 시 버튼은 꺼둠
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 자기 자신을 클릭했을 때 → 버튼 켜기
        if (upgradeButtonUI != null && _trainKhan != null)
        {
            Debug.Log($"[UBM] CanUpgrade: {_trainKhan.CanUpgrade()} {_trainKhan.level} {TrainImageDB.GetMaxLevel(_trainKhan.typeKey)}");
            // [수정된 부분] 업그레이드가 가능한 경우에만 버튼 표시/토글 로직 실행
            if (_trainKhan.CanUpgrade())
            {
                bool isActive = upgradeButtonUI.activeSelf;
                HideAllButtons(); // 다른 오브젝트 버튼은 다 끔
                upgradeButtonUI.SetActive(!isActive); // 자기 버튼만 토글
            }
            else
            {
                // 최대 레벨에 도달했다면, 혹시 켜져있을 버튼을 숨깁니다.
                HideAllButtons();
            }
        }
    }

    // 버튼 OnClick()에 연결할 함수
    public void PerformUpgrade()
    {
        Debug.Log($"[UBM] PerformUpgrade clicked on {name}");
        if (_trainKhan != null)
        {
            int before = _trainKhan.level;
            _trainKhan.Upgrade();
            Debug.Log($"[UBM] {name} level {before} -> {_trainKhan.level}");
        }

        // 업그레이드 후에는 버튼을 숨깁니다.
        if (upgradeButtonUI != null) upgradeButtonUI.SetActive(false);
    }


    // 씬 내 모든 UpgradeButtonManager 버튼 숨김
    private void HideAllButtons()
    {
        // Unity 2023+ 권장 API
        var allManagers = FindObjectsByType<UpgradeButtonManager>(FindObjectsSortMode.None);

        foreach (var mgr in allManagers)
        {
            if (mgr.upgradeButtonUI != null)
                mgr.upgradeButtonUI.SetActive(false);
        }
    }
}