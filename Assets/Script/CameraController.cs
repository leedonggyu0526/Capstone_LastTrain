using UnityEngine;

public class CameraHorizontalMove : MonoBehaviour
{
    [Header("이동 속도")]
    [Tooltip("A/D 키 입력 시 좌우 이동 속도")]
    public float moveSpeed = 5f;

    void Update()
    {
        Vector3 dir = Vector3.zero;

        // A 키를 누르면 왼쪽으로, D 키를 누르면 오른쪽으로 이동
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir += Vector3.right;
        }

        // 월드 좌표계 기준으로 이동 적용
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }
}
