using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEventController : MonoBehaviour
{
    public TrainEventUIManager uiManager;

    [Header("ScriptableObjects")]
    public List<TrainEventData> eventDataList;

    private List<TrainEvent> runtimeEvents = new List<TrainEvent>();

    private void Start()
    {
        // ScriptableObject → TrainEvent 변환
        foreach (var data in eventDataList)
        {
            runtimeEvents.Add(new TrainEvent
            {
                eventId = data.eventId,
                eventImage = data.eventImage,
                description = data.description,
                choices = new List<string>(data.choices)
            });
        }

        StartCoroutine(EventRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowTestEvent();
        }
    }

    private void ShowTestEvent()
    {
        if (runtimeEvents.Count == 0) return;

        int randomIndex = Random.Range(0, runtimeEvents.Count);
        TrainEvent randomEvent = runtimeEvents[randomIndex];

        uiManager.ShowEvent(randomEvent, (selectedChoice) =>
        {
            Debug.Log($"[테스트] 선택된 선택지: {selectedChoice}");
        });
    }


    IEnumerator EventRoutine()
    {
        while (true)
        {
            float waitTime = GetGaussianDelay(30f, 300f);
            yield return new WaitForSeconds(waitTime);

            int randomIndex = Random.Range(0, runtimeEvents.Count);
            TrainEvent randomEvent = runtimeEvents[randomIndex];

            bool waiting = true;

            uiManager.ShowEvent(randomEvent, (selectedChoice) => {
                Debug.Log($"선택된 선택지: {selectedChoice}");
                waiting = false;
            });

            while (waiting)
                yield return null;
        }
    }

    float GetGaussianDelay(float min, float max)
    {
        float u1 = Random.value;
        float u2 = Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        float mean = (min + max) / 2f;
        float stdDev = (max - min) / 6f;
        float value = mean + stdDev * randStdNormal;
        return Mathf.Clamp(value, min, max);
    }
}
