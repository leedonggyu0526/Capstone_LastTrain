using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTrainEvent", menuName = "TrainEvent/Create New Event")]
public class TrainEventData : ScriptableObject
{
    public int eventId;
    public Sprite eventImage;
    [TextArea(2, 5)] public string description;
    public List<string> choices;
}

