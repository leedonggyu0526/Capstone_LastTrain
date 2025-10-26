using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GearSpeedController : MonoBehaviour, IPointerDownHandler
{
    [Header("Refs")]
    public BackgroundScrolling backgroundScrolling;

    [Header("Gears (0 = 기본속도 1f)")]
    [SerializeField] private float[] speeds = { 1f, 2f, 3f, 4f, 5f};
    [SerializeField] private int defaultGearIndex = 0;
    
    public Sprite gearSprite;
    public GameObject gearCursor;
    
    [Header("커서 회전 설정")]
    [SerializeField] private float rotationSpeed = 5f; // 회전 속도

    public int currentGear = 0;
    public float currentSpeed = 0f;
    
    private Coroutine rotationCoroutine;

    void Start()
    {
        SetGearSpeed(defaultGearIndex);
        StartCoroutine(ApplySpeedNextFrame());
        Debug.Log($"[Gear] 시작: speeds[0]={speeds[0]}");
    }

    IEnumerator ApplySpeedNextFrame()
    {
        yield return null;
        ApplySpeed();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
    }

    /// <summary>
    /// 기어 이미지를 클릭했을 때 호출됨
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // 클릭 위치를 월드 좌표로 변환
        Vector2 clickPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out clickPosition
        );

        // 기어 중심으로부터 클릭 위치까지의 각도 계산
        float angle = GetAngleFromCenter(clickPosition);
        
        Debug.Log($"[Gear] 클릭 각도: {angle}도 (로컬 위치: {clickPosition})");
        
        // 각도를 기반으로 기어 변경 (옵션)
        ChangeGearByAngle(angle);
    }

    /// <summary>
    /// 중심으로부터의 각도 계산 (0~360도)
    /// </summary>
    private float GetAngleFromCenter(Vector2 localPosition)
    {
        // Atan2를 사용하여 각도 계산 (-180 ~ 180도)
        float angle = Mathf.Atan2(localPosition.y, localPosition.x) * Mathf.Rad2Deg;
        
        // 0~360도로 변환
        if (angle < 0)
            angle += 360f;
        
        return angle;
    }

    /// <summary>
    /// 각도에 따라 기어 변경 (반원형 게이지용)
    /// </summary>
    private void ChangeGearByAngle(float angle)
    {
        // 반원형 게이지: 0도(오른쪽) ~ 180도(왼쪽)
        // I(왼쪽) ~ V(오른쪽) 순서로 배치
        
        int newGear = -1;
        
        // 각도 범위에 따른 기어 결정
        // V: 0~36도 (오른쪽 아래)
        // IV: 36~72도
        // III: 72~108도 (가장 위)
        // II: 108~144도
        // I: 144~180도 (왼쪽 아래)
        
        if (angle >= 0f && angle < 36f)
        {
            newGear = 4; // V = 5단 (인덱스 4)
        }
        else if (angle >= 36f && angle < 72f)
        {
            newGear = 3; // IV = 4단 (인덱스 3)
        }
        else if (angle >= 72f && angle < 108f)
        {
            newGear = 2; // III = 3단 (인덱스 2)
        }
        else if (angle >= 108f && angle < 144f)
        {
            newGear = 1; // II = 2단 (인덱스 1)
        }
        else if (angle >= 144f && angle <= 180f)
        {
            newGear = 0; // I = 1단 (인덱스 0)
        }
        
        // 유효한 기어이고 현재와 다르면 변경
        if (newGear >= 0 && newGear < speeds.Length && newGear != currentGear)
        {
            SetGearSpeed(newGear);
            Debug.Log($"[Gear] {GetRomanNumeral(newGear)} 영역 클릭 → {newGear + 1}단");
        }
        else if (newGear == -1)
        {
            Debug.LogWarning($"[Gear] 클릭한 각도({angle})가 유효 범위 밖입니다.");
        }
    }

    private void SetGearCursor(int gearIndex)
    {
        if (gearCursor == null) return;
        
        // 목표 각도 결정
        float targetAngle = 0f;
        switch (gearIndex)
        {
            case 0: targetAngle = 80f; break;   // I
            case 1: targetAngle = 45f; break;   // II
            case 2: targetAngle = 0f; break;    // III
            case 3: targetAngle = -45f; break;  // IV
            case 4: targetAngle = -80f; break;  // V
            default: targetAngle = 0f; break;
        }
        
        // 이전 코루틴이 실행 중이면 중지
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }
        
        // 부드럽게 회전하는 코루틴 시작
        rotationCoroutine = StartCoroutine(RotateCursorSmoothly(targetAngle));
    }
    
    /// <summary>
    /// 커서를 부드럽게 회전시키는 코루틴
    /// </summary>
    private IEnumerator RotateCursorSmoothly(float targetZAngle)
    {
        if (gearCursor == null) yield break;
        
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZAngle);
        
        while (Quaternion.Angle(gearCursor.transform.rotation, targetRotation) > 0.1f)
        {
            gearCursor.transform.rotation = Quaternion.Lerp(
                gearCursor.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }
        
        // 최종 위치로 정확히 맞춤
        gearCursor.transform.rotation = targetRotation;
    }
    
    /// <summary>
    /// 기어 인덱스를 로마 숫자로 변환
    /// </summary>
    private string GetRomanNumeral(int gearIndex)
    {
        string[] romanNumerals = { "I", "II", "III", "IV", "V" };
        if (gearIndex >= 0 && gearIndex < romanNumerals.Length)
            return romanNumerals[gearIndex];
        return "?";
    }

    public void SetGearSpeed(int gearIndex)
    {
        if (gearIndex < 0 || gearIndex >= speeds.Length)
        {
            Debug.LogWarning($"[기어 변경] 잘못된 인덱스: {gearIndex}");
            return;
        }
        currentGear = gearIndex;
        ApplySpeed();
    }

    private void ApplySpeed()
    {
        if (speeds == null || speeds.Length == 0) speeds = new float[] { 1f, 2f, 3f, 4f };
        if (speeds[0] <= 0f) { Debug.LogWarning("[Diag] speeds[0]를 1f로 보정"); speeds[0] = 1f; }

        currentSpeed = speeds[currentGear];

        if (backgroundScrolling != null)
        {
            backgroundScrolling.speed = currentSpeed;
            Debug.Log($"[기어 변경] {currentGear + 1}단 → 속도 {currentSpeed}");
        }
        else
        {
            Debug.LogError("[Gear] BackgroundScrolling 미연결");
        }

        // 기어 커서 회전
        if (gearCursor != null)
        {
            SetGearCursor(currentGear);
        }
    }

    //이벤트 발생 시 속도저하
    public void ApplySpeedDebuff(float debuffRate)
    {
        // 현재 속도를 기준으로 debuffRate만큼 낮춥니다.
        currentSpeed = speeds[currentGear] * (1f - debuffRate);

        // 배경 스크롤링에 적용
        backgroundScrolling.speed = currentSpeed;
    }

    // 이벤트 해결 시 (DeactivateEvent):
    public void ClearSpeedDebuff()
    {
        // 원래 기어 속도로 복구합니다.
        currentSpeed = speeds[currentGear];
        backgroundScrolling.speed = currentSpeed;
    }

}
