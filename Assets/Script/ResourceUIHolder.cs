using UnityEngine;

/// <summary>
/// ResourceUIManager가 씬에서 UI 요소를 찾을 수 있도록 정보를 담는 컴포넌트입니다.
/// 각 자원 UI(예: Fuel_UI_Group)의 최상위 오브젝트에 이 스크립트를 추가하고,
/// 자원 타입과 UI 요소들을 인스펙터에서 할당해주세요. : 모든 씬에서 UI 적용 위함
/// </summary>
public class ResourceUIHolder : MonoBehaviour
{
    public ResourceUI ui;
}
