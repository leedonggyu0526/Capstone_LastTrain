using UnityEngine;

/// <summary>
/// 중심각이 30도인 부채꼴 Mesh를 만들어주는 컴포넌트
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class SectorButton : MonoBehaviour
{
    public float radius = 100f; // 반지름
    public int segments = 10; // 부채꼴을 나눌 조각 수
    public float angleDegree = 30f; // 중심각

    public int buttonIndex; // 버튼 고유 번호

    
    void Start()
    {
        CreateSectorMesh();
    }

    void CreateSectorMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        int vertCount = segments + 2; // 중심점 + 외곽 점들
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[segments * 3];

        float angleRad = Mathf.Deg2Rad * angleDegree;
        float anglePerSegment = angleRad / segments;


        vertices[0] = Vector3.zero; // 중심점




        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = i * anglePerSegment;
            float x = Mathf.Sin(currentAngle) * radius;
            float y = Mathf.Cos(currentAngle) * radius;
            vertices[i + 1] = new Vector3(x, y, 0);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        // PolygonCollider2D 충돌 영역 지정
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        Vector2[] colliderPoints = new Vector2[vertCount];
        for (int i = 0; i < vertCount; i++)
            colliderPoints[i] = vertices[i];
        collider.points = colliderPoints;
    }

    void OnMouseDown()
    {
        Debug.Log($"Clicked Button {buttonIndex}");

        switch (buttonIndex)
        {
            case 0: Debug.Log("기어 1단 설정"); break;
            case 1: Debug.Log("기어 2단 설정"); break;
            case 2: Debug.Log("기어 3단 설정"); break;
            case 3: Debug.Log("기어 4단 설정"); break;
            case 4: Debug.Log("기어 5단 설정"); break;
            case 5: Debug.Log("기어 6단 설정"); break;
        }
    }
}
