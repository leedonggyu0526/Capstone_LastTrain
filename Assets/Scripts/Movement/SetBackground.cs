using UnityEngine;
using System.Collections.Generic;

public class SetBackground : MonoBehaviour
{
    #region Inspector
    
    [Header("배경 설정")]
    public SpriteRenderer targetSpriteRenderer;
    public Sprite[] sceneBackgroundSprites;
    public bool fitToScreenOnStart = true;
    
    #endregion
    
    public Sprite currentSceneBackgroundSprite { get; private set; }

    private void Awake()
    {
        if (targetSpriteRenderer == null)
        {
            targetSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (fitToScreenOnStart && sceneBackgroundSprites != null)
        {
            RandomBackground();
        }
    }

    public void RandomBackground()
    {
        if (sceneBackgroundSprites == null || sceneBackgroundSprites.Length == 0) return;
        currentSceneBackgroundSprite = sceneBackgroundSprites[Random.Range(0, sceneBackgroundSprites.Length)];
        ApplyBackground(currentSceneBackgroundSprite);
        Debug.Log($"[SetBackground] 랜덤 배경 적용: {currentSceneBackgroundSprite.name}");
    }

    /// <summary>
    /// 배경 스프라이트를 적용하고 화면에 맞게 조정
    /// </summary>
    public void ApplyBackground(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("[SetBackground] ApplyBackground: sprite=NULL");
            return;
        }

        if (targetSpriteRenderer == null)
        {
            Debug.LogError("[SetBackground] ApplyBackground: targetSpriteRenderer가 설정되지 않았습니다.");
            return;
        }

        targetSpriteRenderer.sprite = sprite;
        FitToScreen(targetSpriteRenderer);
        
        Debug.Log($"[SetBackground] 배경 적용 완료: {sprite.name}");
    }

    /// <summary>
    /// 단일 SpriteRenderer를 화면에 맞게 조정
    /// </summary>
    public void FitToScreen(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogWarning("[SetBackground] FitToScreen: spriteRenderer 또는 sprite가 없습니다.");
            return;
        }

        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[SetBackground] FitToScreen: 메인 카메라를 찾을 수 없습니다.");
            return;
        }

        (float width, float height) = GetScreenSize(cam, spriteRenderer.transform);
        
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        if (spriteSize.x <= 0f || spriteSize.y <= 0f) return;

        // 자식 객체들의 월드 위치와 스케일 저장
        List<(Transform child, Vector3 worldPos, Vector3 worldScale)> childrenData = new List<(Transform, Vector3, Vector3)>();
        foreach (Transform child in spriteRenderer.transform)
        {
            childrenData.Add((child, child.position, child.lossyScale));
        }

        float scaleX = width / spriteSize.x;
        float scaleY = height / spriteSize.y;
        float scale = Mathf.Max(scaleX, scaleY); // cover 모드
        
        spriteRenderer.transform.localScale = new Vector3(scale, scale, 1f);

        // 자식 객체들의 위치와 스케일 복원
        foreach (var data in childrenData)
        {
            data.child.position = data.worldPos;
            // 스케일 복원 (부모 스케일 변경으로 인한 영향 보정)
            Vector3 parentScale = data.child.parent.lossyScale;
            data.child.localScale = new Vector3(
                data.worldScale.x / parentScale.x,
                data.worldScale.y / parentScale.y,
                data.worldScale.z / parentScale.z
            );
        }
    }

    /// <summary>
    /// 여러 SpriteRenderer를 화면에 맞게 조정 (BackgroundScrolling 용)
    /// </summary>
    public float FitSpritesToScreen(List<SpriteRenderer> spriteRenderers, Transform parentTransform, System.Action<int, int> sortImageCallback)
    {
        var cam = Camera.main;
        if (cam == null) return 0f;

        (float width, float height) = GetScreenSize(cam, parentTransform);

        // 각 스프라이트를 화면을 덮도록 동일 스케일 적용
        foreach (var sr in spriteRenderers)
        {
            if (sr == null || sr.sprite == null) continue;
            Vector2 spriteSize = sr.sprite.bounds.size;
            if (spriteSize.x <= 0f || spriteSize.y <= 0f) continue;

            // 자식 객체들의 월드 위치와 스케일 저장
            List<(Transform child, Vector3 worldPos, Vector3 worldScale)> childrenData = new List<(Transform, Vector3, Vector3)>();
            foreach (Transform child in sr.transform)
            {
                childrenData.Add((child, child.position, child.lossyScale));
            }

            float scaleX = width / spriteSize.x;
            float scaleY = height / spriteSize.y;
            float scale = Mathf.Max(scaleX, scaleY);
            sr.transform.localScale = new Vector3(scale, scale, 1f);

            // 자식 객체들의 위치와 스케일 복원
            foreach (var data in childrenData)
            {
                data.child.position = data.worldPos;
                // 스케일 복원 (부모 스케일 변경으로 인한 영향 보정)
                Vector3 parentScale = data.child.parent.lossyScale;
                data.child.localScale = new Vector3(
                    data.worldScale.x / parentScale.x,
                    data.worldScale.y / parentScale.y,
                    data.worldScale.z / parentScale.z
                );
            }
        }

        // 간격을 실제 스프라이트 폭으로 갱신하고 정렬 재수행
        if (spriteRenderers.Count > 0 && spriteRenderers[0] != null)
        {
            float interval = spriteRenderers[0].bounds.size.x;
            sortImageCallback?.Invoke(spriteRenderers.Count, (int)interval);
            return interval;
        }

        return 0f;
    }

    /// <summary>
    /// 카메라 화면 크기 계산
    /// </summary>
    private (float width, float height) GetScreenSize(Camera cam, Transform targetTransform)
    {
        float worldScreenHeight;
        float worldScreenWidth;

        if (cam.orthographic)
        {
            worldScreenHeight = cam.orthographicSize * 2f;
            worldScreenWidth = worldScreenHeight * cam.aspect;
        }
        else
        {
            // Perspective 카메라
            float distance = Mathf.Abs(targetTransform.position.z - cam.transform.position.z);
            worldScreenHeight = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            worldScreenWidth = worldScreenHeight * cam.aspect;
        }

        return (worldScreenWidth, worldScreenHeight);
    }
}
