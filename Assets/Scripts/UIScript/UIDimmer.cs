using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 팝업 시 배경을 어둡게 처리하고 입력을 차단하는 범용 스크립트.
/// 싱글톤으로 구현되어 어디서든 접근 가능합니다.
/// show()로 표시, hide()로 파괴
/// </summary>
public class UIDimmer : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static UIDimmer Instance { get; private set; }

    [Tooltip("화면을 덮을 반투명 배경 이미지 프리팹")]
    public GameObject dimmerPrefab;

    private GameObject currentDimmerInstance;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 지정된 UI 요소 바로 뒤에 어두운 배경을 표시합니다.
    /// </summary>
    /// <param name="targetUI">포커싱할 UI의 Transform</param>
    public void Show(Transform targetUI)
    {
        if (dimmerPrefab == null)
        {
            Debug.LogError("UIDimmer에 Dimmer Prefab이 할당되지 않았습니다!");
            return;
        }

        // 이미 다른 배경이 있다면 일단 숨깁니다.
        Hide();

        // 프리팹을 targetUI와 같은 부모(Canvas) 아래에 생성합니다.
        currentDimmerInstance = Instantiate(dimmerPrefab, targetUI.parent);

        // 생성된 배경을 targetUI 바로 뒤(계층상 바로 위)로 보냅니다.
        currentDimmerInstance.transform.SetSiblingIndex(targetUI.GetSiblingIndex());
        currentDimmerInstance.SetActive(true);
    }

    /// <summary>
    /// 현재 표시된 어두운 배경을 숨기고 파괴합니다.
    /// </summary>
    public void Hide()
    {
        if (currentDimmerInstance != null)
        {
            Destroy(currentDimmerInstance);
            currentDimmerInstance = null;
        }
    }
}
