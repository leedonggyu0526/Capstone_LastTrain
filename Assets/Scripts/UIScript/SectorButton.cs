using UnityEngine;

/// <summary>
/// �߽ɰ��� 30���� ��ä�� Mesh�� ������ִ� ������Ʈ
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class SectorButton : MonoBehaviour
{
    public float radius = 100f; // ������
    public int segments = 10; // ��ä���� ���� ���� ��
    public float angleDegree = 30f; // �߽ɰ�

    public int buttonIndex; // ��ư ���� ��ȣ

    
    void Start()
    {
        CreateSectorMesh();
    }

    void CreateSectorMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        int vertCount = segments + 2; // �߽��� + �ܰ� ����
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[segments * 3];

        float angleRad = Mathf.Deg2Rad * angleDegree;
        float anglePerSegment = angleRad / segments;


        vertices[0] = Vector3.zero; // �߽���




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

        // PolygonCollider2D �浹 ���� ����
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
            case 0: Debug.Log("��� 1�� ����"); break;
            case 1: Debug.Log("��� 2�� ����"); break;
            case 2: Debug.Log("��� 3�� ����"); break;
            case 3: Debug.Log("��� 4�� ����"); break;
            case 4: Debug.Log("��� 5�� ����"); break;
            case 5: Debug.Log("��� 6�� ����"); break;
        }
    }
}
