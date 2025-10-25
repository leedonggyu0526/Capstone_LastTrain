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
    
    // Entrance 이벤트 ID 목록
    private List<string> caveEventIDList;

    private string currentEventID = "";
    private bool isEventEnded = false;

    // 초기화
    void Start()
    {
        // UI Manager 검증
        if (trainEventUIManager == null)
        {
            Debug.LogError("[SettlementObjectClickHandler] trainEventUIManager가 할당되지 않았습니다! Inspector에서 할당해주세요.");
        }
        else
        {
            Debug.Log("[SettlementObjectClickHandler] trainEventUIManager 연결 확인됨");
        }
        
        if (storePanel != null)
            storePanel.SetActive(false);
        if (eventPanel != null)
            eventPanel.SetActive(false);
        
        // 동굴 이벤트 ID 목록 초기화
        caveEventIDList = new List<string>();
        
        // TrainEventManager에서 Entrance 이벤트를 가져와서 이벤트 ID 수집
        Debug.Log("[SettlementObjectClickHandler] Entrance 이벤트 수집 시작");
        var trainEventManager = TrainEventManager.Instance;
        if (trainEventManager != null)
        {
            Debug.Log("[SettlementObjectClickHandler] TrainEventManager 찾음");
            var entranceEventIDList = trainEventManager.GetEntranceTrainEventID();
            if (entranceEventIDList != null)
            {
                Debug.Log($"[SettlementObjectClickHandler] {entranceEventIDList.Count}개의 Entrance 이벤트 발견");
                foreach (var entranceEventID in entranceEventIDList)
                {
                    caveEventIDList.Add(entranceEventID);
                    Debug.Log($"[SettlementObjectClickHandler] 이벤트 추가: {entranceEventID}");
                }
            }
            else
            {
                Debug.LogWarning("[SettlementObjectClickHandler] entranceEventIDList가 null입니다");
            }
        }
        else
        {
            Debug.LogError("[SettlementObjectClickHandler] TrainEventManager를 찾을 수 없습니다");
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

        // 탐험 이벤트 무작위 선택
        if (trainEventUIManager != null)
            {
                if (caveEventIDList != null && caveEventIDList.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, caveEventIDList.Count);
                    string selectedEventID = caveEventIDList[randomIndex];
                    currentEventID = selectedEventID;
                }
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
                
                // trainEventUIManager null 체크
                if (trainEventUIManager == null)
                {
                    Debug.LogError("[SettlementObjectClickHandler] trainEventUIManager가 null입니다! Inspector에서 할당해주세요.");
                    break;
                }
                
                if (currentEventID != "")
                {
                    Debug.Log($"[SettlementObjectClickHandler] ShowEvent 호출: {currentEventID}");
                    trainEventUIManager.ShowEvent(currentEventID);
                }
                else
                {
                    Debug.LogError("[SettlementObjectClickHandler] currentEventID가 비어있습니다!");
                }
                break;
            default:
                Debug.Log($"[SettlementObjectClickHandler] 알 수 없는 객체: '{objectName}' - 처리할 수 있는 객체가 아닙니다.");
                break;
        }
        
        Debug.Log($"[SettlementObjectClickHandler] 객체 '{objectName}' 처리 완료");
    }
}