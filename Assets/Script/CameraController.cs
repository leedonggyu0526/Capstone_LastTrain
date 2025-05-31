using UnityEngine;

public class CameraHorizontalMove : MonoBehaviour
{
    [Header("�̵� �ӵ�")]
    [Tooltip("A/B Ű �Է� �� �¿� �̵� �ӵ�")]
    public float moveSpeed = 5f;

    void Update()
    {
        Vector3 dir = Vector3.zero;

        // A Ű�� ������ ��������, B Ű�� ������ ���������� �̵�
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir += Vector3.right;
        }

        // ������ �������� �̵� ����
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }
}
