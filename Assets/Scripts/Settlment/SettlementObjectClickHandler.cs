using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class SettlementObjectClickHandler : MonoBehaviour
{
    // Inspector 창에서 연결할 패널 및 오브젝트
    public GameObject storePanel;
    public GameObject eventPanel;
    
    // TrainEventUIManager 참조
    public TrainEventUIManager trainEventUIManager;
    public StoreUIManager storeUIManager;
    
    // RepairUIManager 참조
    public RepairUIManager repairUIManager;
    
    // DepartureUIManager 참조
    public DepartureUIManager departureUIManager;
    
    // 동굴 이벤트 ID 목록
    private List<string> caveEventIDList;

    // 초기화
    void Start()
    {
        if (storePanel != null)
            storePanel.SetActive(false);
        if (eventPanel != null)
            eventPanel.SetActive(false);
        
        // 동굴 이벤트 ID 목록 초기화
        caveEventIDList = new List<string>();
        
        // TrainEventManager에서 모든 이벤트를 가져와서 CAVE로 시작하는 이벤트 ID 수집
        var trainEventManager = TrainEventManager.Instance;
        if (trainEventManager != null)
        {
            var allEvents = trainEventManager.GetAllTrainEvents();
            if (allEvents != null)
            {
                foreach (var eventPair in allEvents)
                {
                    string eventId = eventPair.Key;
                    if (!string.IsNullOrEmpty(eventId) && eventId.StartsWith("CAVE"))
                    {
                        caveEventIDList.Add(eventId);
                    }
                }
            }
        }
        
        // trainEventUIManager가 할당되었는지 확인
        if (trainEventUIManager == null)
        {
            Debug.LogWarning("trainEventUIManager가 할당되지 않았습니다. Unity Inspector에서 TrainEventUIManager를 연결해주세요.");
        }
        
        // repairUIManager가 할당되었는지 확인
        if (repairUIManager == null)
        {
            Debug.LogWarning("repairUIManager가 할당되지 않았습니다. Unity Inspector에서 RepairUIManager를 연결해주세요.");
        }
        
        // departureUIManager가 할당되었는지 확인
        if (departureUIManager == null)
        {
            Debug.LogWarning("departureUIManager가 할당되지 않았습니다. Unity Inspector에서 DepartureUIManager를 연결해주세요.");
        }
    }

    // 매 프레임마다 마우스 클릭 확인
    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            string clickedObjectName = null;
            GameObject clickedObject = null;
            
            // 1. UI 요소 클릭 감지 (GraphicRaycast)
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            
            if (results.Count > 0)
            {
                clickedObject = results[0].gameObject;
                clickedObjectName = clickedObject.name;
                Debug.Log($"[SettlementObjectClickHandler] UI 요소 클릭됨: {clickedObjectName} (위치: {clickedObject.transform.position})");
            }
            else
            {
                // 2. 3D 오브젝트 클릭 감지 (Physics Raycast)
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    clickedObject = hit.collider.gameObject;
                    clickedObjectName = clickedObject.name;
                    Debug.Log($"[SettlementObjectClickHandler] 3D 오브젝트 클릭됨: {clickedObjectName} (위치: {clickedObject.transform.position})");
                }
                else
                {
                    Debug.Log("[SettlementObjectClickHandler] 클릭된 오브젝트가 없습니다.");
                    return;
                }
            }
            
            // 클릭된 오브젝트 처리
            if (!string.IsNullOrEmpty(clickedObjectName))
            {
                Debug.Log($"[SettlementObjectClickHandler] 선택된 객체: '{clickedObjectName}' 처리 시작");
                HandleObjectClick(clickedObjectName);
            }
        }
    }
    
    // 오브젝트 클릭 처리 메서드
    void HandleObjectClick(string objectName)
    {
        Debug.Log($"[SettlementObjectClickHandler] HandleObjectClick() 호출됨 - 객체명: '{objectName}'");

        
        switch (objectName)
        {
            case "Store":
                Debug.Log("[SettlementObjectClickHandler] Store 객체 처리 중...");
                if (storePanel != null)
                {
                    storeUIManager.ShowStore();
                }
                break;
            case "Departure":
                Debug.Log("[SettlementObjectClickHandler] Departure 객체 처리 중...");
                
                if (departureUIManager != null)
                {
                    departureUIManager.ShowDeparturePanel();
                }
                else
                {
                    Debug.LogError("[SettlementObjectClickHandler] departureUIManager가 null입니다!");
                }
                break;
            case "Repair":
                Debug.Log("[SettlementObjectClickHandler] Repair 객체 처리 중...");
                
                if (repairUIManager != null)
                {
                    repairUIManager.ToggleRepairPanel();
                }
                else
                {
                    Debug.LogError("[SettlementObjectClickHandler] repairUIManager가 null입니다!");
                }
                break;
            case "Cave":
                Debug.Log("[SettlementObjectClickHandler] Cave 객체 처리 중...");
                if (trainEventUIManager != null)
                {
                    if (caveEventIDList != null && caveEventIDList.Count > 0)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, caveEventIDList.Count);
                        string selectedCaveEvent = caveEventIDList[randomIndex];

                        trainEventUIManager.ShowEvent(selectedCaveEvent);
                    }
                    else
                    {
                        Debug.LogWarning("CAVE로 시작하는 이벤트가 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning("trainEventUIManager가 할당되지 않았습니다. Unity Inspector에서 TrainEventUIManager를 연결해주세요.");
                }
                break;
            default:
                Debug.Log($"[SettlementObjectClickHandler] 알 수 없는 객체: '{objectName}' - 처리할 수 있는 객체가 아닙니다.");
                break;
        }
        
        Debug.Log($"[SettlementObjectClickHandler] 객체 '{objectName}' 처리 완료");
    }
}