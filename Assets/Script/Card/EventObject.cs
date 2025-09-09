using UnityEngine;

public class EventObject : MonoBehaviour
{
    public int eventID; // �̺�Ʈ ID

    // ó������ ��Ȱ��ȭ
    void Start()
    {
        gameObject.SetActive(false);
    }

    // �̺�Ʈ ���� �� ȣ��
    public void ActivateEvent()
    {
        gameObject.SetActive(true);
    }

    // �̺�Ʈ ���� �� ȣ��
    public void DeactivateEvent()
    {
        gameObject.SetActive(false);
    }
}
