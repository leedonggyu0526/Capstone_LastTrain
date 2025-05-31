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
    /// 이미지 정렬 (왼쪽 스크롤용: 클론을 오른쪽에 배치)
    /// </summary>
    private void SortImage()
    {
        // 0: 원본, 1: 클론
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].transform.localPosition = Vector3.right * interval * i;
        }
        // 첫 번째로 “기준점”이 될 인덱스를 항상 클론(1)으로
        firstIndex = 1;
    }

    private void Update()
    {
        UpdateMoveImages();
    }

    /// <summary>
    /// 이미지 이동 업데이트
    /// </summary>
    private void UpdateMoveImages()
    {
        float move = Time.deltaTime * speed;
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            var sr = spriteRenderers[i];
            // ← 방향으로 이동
            sr.transform.localPosition += Vector3.left * move;

            // 왼쪽으로 일정 거리(-interval) 넘어가면 맨 오른쪽으로 붙이기
            if (sr.transform.localPosition.x <= -interval)
            {
                // firstIndex의 x 위치에서 +interval 위치로 옮겨서 이어 붙임
                float newX = spriteRenderers[firstIndex].transform.localPosition.x + interval;
                sr.transform.localPosition = new Vector3(newX, 0f, 0f);
                firstIndex = spriteRenderers.IndexOf(sr);
            }
        }
    }

}
