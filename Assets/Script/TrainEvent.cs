using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  ���� ������ ���� Ŭ����
/// </summary>
[System.Serializable]
public class TrainEvent
{
    public int eventId;
    public Sprite eventImage;
    public string description;
    public List<string> choices;
}
