using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    #region Inspector

    public SpriteRenderer spriteRenderer;
    public float interval;
    public float speed = 1f;
    public SetBackground setBackground;

    #endregion

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private int firstIndex = 1;

    private void Awake()
    {
        // SetBackground 컴포넌트 찾기 (없으면 null로 유지)
        if (setBackground == null)
        {
            setBackground = GetComponent<SetBackground>();
        }
    }
    
    private void Start()
    {
        var newSpriteRenderer = Instantiate<SpriteRenderer>(spriteRenderer);
        
        // 원본 spriteRenderer의 sprite를 복사 (SetBackground가 Awake에서 이미 설정했음)
        if (spriteRenderer.sprite != null)
        {
            newSpriteRenderer.sprite = spriteRenderer.sprite;
        }
        // SetBackground가 있고 현재 배경 스프라이트가 있으면 적용
        else if (setBackground != null && setBackground.currentSceneBackgroundSprite != null)
        {
            newSpriteRenderer.sprite = setBackground.currentSceneBackgroundSprite;
        }
        
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
        if (setBackground != null)
        {
            //interval = setBackground.FitSpritesToScreen(spriteRenderers, transform, (count, _) => SortImage());
            float oldInterval = interval; // ⬅️ 기존 값 저장

            // FitSpritesToScreen이 interval 값을 갱신합니다.
            float newInterval = setBackground.FitSpritesToScreen(spriteRenderers, transform, (count, _) => SortImage());

            // ⬅️ 반환 값이 0이거나 유효하지 않으면 기존 값을 사용합니다.
            if (newInterval > 0.001f)
            {
                interval = newInterval;
            }
            else
            {
                Debug.LogWarning("[BackgroundScrolling] FitSpritesToScreen에서 유효하지 않은 간격(0)을 반환했습니다. 기존 값인 " + oldInterval + "을 사용합니다.");
                interval = oldInterval;
            }
        }
        else
        {
            // SetBackground가 없으면 기본 동작
            Debug.LogWarning("[BackgroundScrolling] SetBackground 컴포넌트가 없습니다. 스케일 조정을 건너뜁니다.");
        }

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
}