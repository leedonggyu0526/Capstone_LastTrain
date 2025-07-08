using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public EventSpawner eventSpawner;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;

        if (droppedCard == null)
        {
            Debug.LogError("����� ī�尡 �����ϴ�.");
            return;
        }

        CardDrag card = droppedCard.GetComponent<CardDrag>();

        if (card != null)
        {
            // �̺�Ʈ�� ���� ��� �� ī�� ����
            if (eventSpawner.currentEventID == -1)
            {
                Debug.Log("�̺�Ʈ�� �����ϴ�! ī�� ����.");
                card.ReturnToOriginalPositionSmooth(); // �ִϸ��̼� ���� ��õ
                return;
            }

            // ī�� ID�� ��ġ�ϴ� ��� �� ����
            if (card.cardID == eventSpawner.currentEventID)
            {
                Debug.Log("�������� ī�� ���! �̺�Ʈ ����");
                eventSpawner.DestroyCurrentEvent();
                Destroy(card.gameObject);
            }
            else
            {
                // ī�� ID�� �ٸ� ��� �� ī�� ����
                Debug.Log("ī�尡 ���� �ʽ��ϴ�! ī�� ����.");
                card.ReturnToOriginalPositionSmooth();
            }
        }
    }
}
