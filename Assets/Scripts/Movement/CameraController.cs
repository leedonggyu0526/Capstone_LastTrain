using UnityEngine;

public class CameraHorizontalMove : MonoBehaviour
{
    [Header("이동 속도")]
<<<<<<< Updated upstream:Assets/Script/CameraController.cs
    [Tooltip("A/D 키 입력 시 좌우 이동 속도")]
=======
    [Tooltip("A/B 키 입력 시 좌우 이동")]
>>>>>>> Stashed changes:Assets/Scripts/Movement/CameraController.cs
    public float moveSpeed = 5f;

    void Update()
    {
        Vector3 dir = Vector3.zero;

<<<<<<< Updated upstream:Assets/Script/CameraController.cs
        // A 키를 누르면 왼쪽으로, D 키를 누르면 오른쪽으로 이동
=======
        // A 키 입력 시 좌측 이동, B 키 입력 시 우측 이동
>>>>>>> Stashed changes:Assets/Scripts/Movement/CameraController.cs
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir += Vector3.right;
        }

<<<<<<< Updated upstream:Assets/Script/CameraController.cs
        // 월드 좌표계 기준으로 이동 적용
=======
        // 이동 속도 적용
>>>>>>> Stashed changes:Assets/Scripts/Movement/CameraController.cs
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }
}
