using UnityEngine;

public class CameraHorizontalMove : MonoBehaviour
{
    [Header("이동 속도")]
    [Tooltip("A/B 키 입력 시 이동 속도")]
    public float moveSpeed = 5f;

    void Update()
    {
        Vector3 dir = Vector3.zero;

        // A 키 입력 시 왼쪽으로 이동, B 키 입력 시 오른쪽으로 이동
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir += Vector3.right;
        }

        // 이동 속도 적용
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }
}
