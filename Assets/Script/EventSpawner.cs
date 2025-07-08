using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    public EventObject[] eventObjects; // Inspector�� ����� �̺�Ʈ ������Ʈ �迭
    public int currentEventID = -1;    // ���� �̺�Ʈ ID (-1�̸� �̺�Ʈ ����)

    private float timer = 0f;          // Ÿ�̸� ����
    private float spawnInterval = 5f;  // 5�� �ֱ�

    void Update()
    {
        // Ÿ�̸� ����
        timer += Time.deltaTime;

        // �ֱ⸶�� �̺�Ʈ �߻� �õ�
        if (timer >= spawnInterval)
        {
            TrySpawnEvent();
            timer = 0f; // Ÿ�̸� �ʱ�ȭ
        }

        // P Ű�� ������ ��� �̺�Ʈ �߻� �õ�
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnEvent();
        }
    }

    // �̺�Ʈ �߻� �Լ�
    public void TrySpawnEvent()
    {
        // ���� �̺�Ʈ�� �����ϸ� ���� �߻����� ����
        if (currentEventID != -1)
        {
            Debug.Log("�̹� �̺�Ʈ�� �߻� ���Դϴ�.");
            return;
        }

        // ���� �̺�Ʈ ����
        int randomIndex = Random.Range(0, eventObjects.Length);

        // �̺�Ʈ Ȱ��ȭ
        eventObjects[randomIndex].ActivateEvent();

        // ���� �̺�Ʈ ID ����
        currentEventID = eventObjects[randomIndex].eventID;

        Debug.Log($"�̺�Ʈ �߻�: {currentEventID}");
    }

    // �̺�Ʈ ���� �Լ�
    public void DestroyCurrentEvent()
    {
        if (currentEventID == -1) return;

        // ���� �̺�Ʈ ��Ȱ��ȭ
        foreach (var evt in eventObjects)
        {
            if (evt.eventID == currentEventID)
            {
                evt.DeactivateEvent();
                break;
            }
        }

        currentEventID = -1;

        Debug.Log("�̺�Ʈ�� ����Ǿ����ϴ�.");
    }
}
