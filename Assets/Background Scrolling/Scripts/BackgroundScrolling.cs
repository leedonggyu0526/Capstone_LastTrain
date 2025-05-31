using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    #region Inspector

    public SpriteRenderer spriteRenderer;
    public float interval;
    public float speed = 1f;

    #endregion

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private int firstIndex = 1;

    private void Awake()
    {
        var newSpriteRenderer = Instantiate<SpriteRenderer>(spriteRenderer);
        newSpriteRenderer.transform.SetParent(this.transform);
        spriteRenderers.Add(spriteRenderer);
        spriteRenderers.Add(newSpriteRenderer);
        SortImage();
    }

    /// <summary>
    /// �̹��� ���� (���� ��ũ�ѿ�: Ŭ���� �����ʿ� ��ġ)
    /// </summary>
    private void SortImage()
    {
        // 0: ����, 1: Ŭ��
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].transform.localPosition = Vector3.right * interval * i;
        }
        // ù ��°�� ������������ �� �ε����� �׻� Ŭ��(1)����
        firstIndex = 1;
    }

    private void Update()
    {
        UpdateMoveImages();
    }

    /// <summary>
    /// �̹��� �̵� ������Ʈ
    /// </summary>
    private void UpdateMoveImages()
    {
        float move = Time.deltaTime * speed;
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            var sr = spriteRenderers[i];
            // �� �������� �̵�
            sr.transform.localPosition += Vector3.left * move;

            // �������� ���� �Ÿ�(-interval) �Ѿ�� �� ���������� ���̱�
            if (sr.transform.localPosition.x <= -interval)
            {
                // firstIndex�� x ��ġ���� +interval ��ġ�� �Űܼ� �̾� ����
                float newX = spriteRenderers[firstIndex].transform.localPosition.x + interval;
                sr.transform.localPosition = new Vector3(newX, 0f, 0f);
                firstIndex = spriteRenderers.IndexOf(sr);
            }
        }
    }

}
