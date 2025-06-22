using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  순수 데이터 구조 클래스
/// </summary>
[System.Serializable]
public class TrainEvent
{
    public int eventId;
    public Sprite eventImage;
    public string description;
    public List<string> choices;
}
