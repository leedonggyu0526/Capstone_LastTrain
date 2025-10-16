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

    public void SetBackground(Sprite newSprite)
    {
        if (newSprite == null)
        {
            Debug.LogError("[BackgroundScrolling] SetBackground: newSprite=NULL");
            return;
        }

        // 원본 + 복제본 모두 스프라이트 교체
        if (spriteRenderer != null) spriteRenderer.sprite = newSprite;
        foreach (var sr in spriteRenderers)
            if (sr != null) sr.sprite = newSprite;

        // 화면을 가득 채우도록 스케일 조정 및 간격 재설정
        FitSpritesToScreen();

        Debug.Log($"[BackgroundScrolling] 배경 교체: {newSprite.name}");
    }

    /// <summary>
    /// 이미지 정렬 (배경 스크롤링: 클론을 원본 오른쪽에 배치)
    /// </summary>
    private void SortImage()
    {
        // 0: 원본, 1: 클론
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].transform.localPosition = Vector3.right * interval * i;
        }
        // 첫 번째로 보이는 스프라이트는 항상 클론(1)부터 시작
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
            // 왼쪽 방향으로 이동
            sr.transform.localPosition += Vector3.left * move;

            // 화면의 왼쪽 바깥(-interval)으로 나가면 오른쪽으로 재배치
            if (sr.transform.localPosition.x <= -interval)
            {
                // firstIndex의 x 위치에서 +interval 한 위치로 재배치
                float newX = spriteRenderers[firstIndex].transform.localPosition.x + interval;
                sr.transform.localPosition = new Vector3(newX, 0f, 0f);
                firstIndex = spriteRenderers.IndexOf(sr);
            }
        }
    }

    // 현재 카메라 화면을 가득 채우도록 스프라이트의 스케일을 조정하고 간격(interval)을 갱신
    private void FitSpritesToScreen()
    {
        var cam = Camera.main;
        if (cam == null) return;

        // Orthographic 카메라 기준. Perspective면 간단히 폭 기준으로만 맞춤
        float worldScreenHeight;
        float worldScreenWidth;

        if (cam.orthographic)
        {
            worldScreenHeight = cam.orthographicSize * 2f;
            worldScreenWidth = worldScreenHeight * cam.aspect;
        }
        else
        {
            // 대략적인 근사치: 카메라와 배경의 Z거리 기준 화면 폭/높이 추정
            float distance = Mathf.Abs(transform.position.z - cam.transform.position.z);
            worldScreenHeight = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            worldScreenWidth = worldScreenHeight * cam.aspect;
        }

        // 각 스프라이트를 화면을 덮도록 동일 스케일 적용
        foreach (var sr in spriteRenderers)
        {
            if (sr == null || sr.sprite == null) continue;
            Vector2 spriteSize = sr.sprite.bounds.size; // world units (PPU 반영됨)
            if (spriteSize.x <= 0f || spriteSize.y <= 0f) continue;

            float scaleX = worldScreenWidth / spriteSize.x;
            float scaleY = worldScreenHeight / spriteSize.y;
            float scale = Mathf.Max(scaleX, scaleY); // cover 모드: 화면을 꽉 채우도록 더 큰 값 사용
            sr.transform.localScale = new Vector3(scale, scale, 1f);
        }

        // 간격을 실제 스프라이트 폭으로 갱신하고 정렬 재수행
        if (spriteRenderers.Count > 0 && spriteRenderers[0] != null)
        {
            interval = spriteRenderers[0].bounds.size.x;
            SortImage();
        }
    }

}
