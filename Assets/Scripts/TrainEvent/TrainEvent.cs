using UnityEngine;
using System.Collections.Generic;
using System;

// 이벤트 구조체
// 이벤트 이름, 이벤트 설명, 이벤트 이미지, 이벤트 선택지 1~3, 각 선택지 설명, 각 선택지 요구사항, 이벤트 결과
// 이벤트 이미지는 이벤트 ID로 저장된 이미지 파일을 불러옴
public class TrainEvent
{
    private string eventName;
    private string eventDescription;
    private Sprite eventImage;
    private string eventChoice1 = null;
    private string eventChoice1Description = null;
    private Dictionary<ResourceType, int> eventChoice1Requirement = null;
    private string eventChoice2 = null;
    private string eventChoice2Description = null;
    private Dictionary<ResourceType, int> eventChoice2Requirement = null;
    private string eventChoice3 = null;
    private string eventChoice3Description = null;
    private Dictionary<ResourceType, int> eventChoice3Requirement = null;
    private Dictionary<ResourceType, int> eventResult = null;

    public TrainEvent(Sprite eventImage, string eventName, string eventDescription, 
                      string eventChoice1, string eventChoice1Description, string eventChoice1Requirement,
                      string eventChoice2, string eventChoice2Description, string eventChoice2Requirement,
                      string eventChoice3, string eventChoice3Description, string eventChoice3Requirement,
                      string eventResult)
    {
        this.eventName = eventName;
        this.eventDescription = eventDescription;
        this.eventImage = eventImage;
        this.eventChoice1 = eventChoice1;
        this.eventChoice1Description = eventChoice1Description;
        this.eventChoice1Requirement = seperateResourceAndQuantity(eventChoice1Requirement);
        this.eventChoice2 = eventChoice2;
        this.eventChoice2Description = eventChoice2Description;
        this.eventChoice2Requirement = seperateResourceAndQuantity(eventChoice2Requirement);
        this.eventChoice3 = eventChoice3;
        this.eventChoice3Description = eventChoice3Description;
        this.eventChoice3Requirement = seperateResourceAndQuantity(eventChoice3Requirement);
        this.eventResult = seperateResourceAndQuantity(eventResult);
    }

    // Getter 메서드들
    public string GetEventName() => eventName;
    public string GetEventDescription() => eventDescription;
    public Sprite GetEventImage() => eventImage;
    public string GetEventChoice1() => eventChoice1;
    public string GetEventChoice1Description() => eventChoice1Description;
    public Dictionary<ResourceType, int> GetEventChoice1Requirement() => eventChoice1Requirement;
    public string GetEventChoice2() => eventChoice2;
    public string GetEventChoice2Description() => eventChoice2Description;
    public Dictionary<ResourceType, int> GetEventChoice2Requirement() => eventChoice2Requirement;
    public string GetEventChoice3() => eventChoice3;
    public string GetEventChoice3Description() => eventChoice3Description;
    public Dictionary<ResourceType, int> GetEventChoice3Requirement() => eventChoice3Requirement;
    public Dictionary<ResourceType, int> GetEventResult() => eventResult;

    // Setter 메서드
    public void SetEventImage(Sprite newImage)
    {
        eventImage = newImage;
        Debug.Log($"이벤트 이미지 업데이트: {eventName}");
    }

    // 리소스 부분과 수치 부분을 분리 후 반환
    public Dictionary<ResourceType, int> seperateResourceAndQuantity(string resourceAndQuantity)
    {
        Dictionary<ResourceType, int> result = new Dictionary<ResourceType, int>();
        string[] parts = resourceAndQuantity.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string part in parts)
        {
            string[] resourceAndQuantityParts = part.Split(new[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);
            string resource = resourceAndQuantityParts[0];
            string quantity = resourceAndQuantityParts[1];
            if (Enum.IsDefined(typeof(ResourceType), resource))
            {
                if (int.TryParse(quantity, out int quantityInt))
                {
                    result.Add((ResourceType)Enum.Parse(typeof(ResourceType), resource), quantityInt);
                }
            }
            else
            {
                return null;
            }
        }
        return result;
    }
    // 선택지 유효성 검사 메서드
    public bool HasChoice1() => IsValidChoice(eventChoice1);
    public bool HasChoice2() => IsValidChoice(eventChoice2);
    public bool HasChoice3() => IsValidChoice(eventChoice3);

    // 선택지가 유효한지 확인하는 헬퍼 메서드
    private bool IsValidChoice(string choice)
    {
        return !string.IsNullOrEmpty(choice) && choice != "null" && choice.Trim() != "";
    }
    private bool IsValidResourceAndQuantity(Dictionary<ResourceType, int> resourceAndQuantity)
    {
        foreach (var resource in resourceAndQuantity.Keys)
        {
            if (!Enum.IsDefined(typeof(ResourceType), resource))
            {

                return false;
            }
            else if (resourceAndQuantity[resource] == 0)
            {
                return false;
            }
        }
        return true;
    }

    // 유효한 선택지 개수 반환
    public int GetValidChoiceCount()
    {
        int count = 0;
        if (HasChoice1()) count++;
        if (HasChoice2()) count++;
        if (HasChoice3()) count++;
        return count;
    }

    // 선택지 설명이 유효한지 확인
    public bool HasChoice1Description() => IsValidChoice(eventChoice1Description);
    public bool HasChoice2Description() => IsValidChoice(eventChoice2Description);
    public bool HasChoice3Description() => IsValidChoice(eventChoice3Description);
    
    // 선택지 요구사항이 유효한지 확인
    public bool HasChoice1Requirement() => IsValidResourceAndQuantity(eventChoice1Requirement);
    public bool HasChoice2Requirement() => IsValidResourceAndQuantity(eventChoice2Requirement);
    public bool HasChoice3Requirement() => IsValidResourceAndQuantity(eventChoice3Requirement);
    
    // 이벤트 결과가 유효한지 확인
    public bool HasEventResult() => IsValidResourceAndQuantity(eventResult);

    // 이벤트 데이터 유효성 검사
    public bool IsValidEvent()
    {
        // 기본 필수 데이터 검증
        if (string.IsNullOrEmpty(eventName) || string.IsNullOrEmpty(eventDescription))
        {
            Debug.LogWarning("이벤트 이름 또는 설명이 비어있습니다.");
            return false;
        }

        // 선택지와 선택지 설명의 일치성 검증
        if (HasChoice1() && !HasChoice1Description())
        {
            Debug.LogWarning("선택지 1이 있지만 설명이 없습니다.");
            return false;
        }
        if (!HasChoice1() && HasChoice1Description()) 
        {
            Debug.LogWarning("선택지 1의 설명이 있지만 선택지가 없습니다.");
            return false;
        }
        if (HasChoice2() && !HasChoice2Description())
        {
            Debug.LogWarning("선택지 2가 있지만 설명이 없습니다.");
            return false;
        }
        if (!HasChoice2() && HasChoice2Description())
        {
            Debug.LogWarning("선택지 2의 설명이 있지만 선택지가 없습니다.");
            return false;
        }
        if (HasChoice3() && !HasChoice3Description())
        {
            Debug.LogWarning("선택지 3이 있지만 설명이 없습니다.");
            return false;
        }
        if (!HasChoice3() && HasChoice3Description())
        {
            Debug.LogWarning("선택지 3의 설명이 있지만 선택지가 없습니다.");
            return false;
        }

        return true;
    }

    public string GetAllEventInfo()
    {
        return "이벤트 이름: " + eventName + "\n" +
            "이벤트 설명: " + eventDescription + "\n" +
            "이벤트 이미지: " + eventImage + "\n" +
            "이벤트 선택지 1: " + eventChoice1 + "\n" +
            "이벤트 선택지 1 설명: " + eventChoice1Description + "\n" +
            "이벤트 선택지 1 요구사항: " + eventChoice1Requirement + "\n" +
            "이벤트 선택지 2: " + eventChoice2 + "\n" +
            "이벤트 선택지 2 설명: " + eventChoice2Description + "\n" +
            "이벤트 선택지 2 요구사항: " + eventChoice2Requirement + "\n" +
            "이벤트 선택지 3: " + eventChoice3 + "\n" +
            "이벤트 선택지 3 설명: " + eventChoice3Description + "\n" +
            "이벤트 선택지 3 요구사항: " + eventChoice3Requirement + "\n" +
            "이벤트 결과: " + eventResult; 
    }
}
