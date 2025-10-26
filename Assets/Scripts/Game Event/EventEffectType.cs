using UnityEngine;

// 이벤트 효과 유형 및 대상의 상수를 정의하는 클래스
public static class EventEffectType
{
    // ==========================================================
    // 1. 이벤트 효과 유형 (Effect Types)
    // CSV의 effectType_1, effectType_2 컬럼에 사용된 값
    // ==========================================================

    // 지속 피해: 초당 일정량의 대미지를 입힘
    public const string DAMAGE_DURATION = "DAMAGE_DURATION";

    // 즉시 피해: 한번에 큰 고정 대미지를 입힘
    public const string DAMAGE_INSTANT = "DAMAGE_INSTANT";

    // 생산량/능력 비율 저하: 생산 시설의 효율을 비율만큼 감소시킴 (0.0 ~ 1.0)
    public const string DEBUFF_RATE = "DEBUFF_RATE";

    // 열차 속도 비율 저하: 열차의 이동 속도를 비율만큼 감소시킴
    public const string DEBUFF_SPEED = "DEBUFF_SPEED";

    // 승객 수 비율 저하: 일정 주기마다 승객 수를 비율만큼 감소시킴
    public const string PASSENGER_DEBUFF = "PASSENGER_DEBUFF";

    // 자원/승객 고정 수치 감소: 자원이나 승객 수를 고정된 수치만큼 즉시 감소시킴
    public const string DECREASE_AMOUNT = "DECREASE_AMOUNT";


    // ==========================================================
    // 2. 이벤트 효과 대상 (Effect Targets)
    // CSV의 effectTarget_1, effectTarget_2 컬럼에 사용된 값
    // ==========================================================

    // 열차 전체 체력 (HP)
    public const string TRAIN_HP = "TRAIN_HP";

    // 엔진 모듈 (연료 생산, 속도 등에 영향)
    public const string ENGINE = "ENGINE";

    // 식량 생산 시설
    public const string FOOD = "FOOD";

    // 기계 부품 생산 시설
    public const string FACTORY = "FACTORY";

    // 승객 수
    public const string PASSENGER = "PASSENGER";

    // 모든 리소스 중 무작위로 하나
    public const string RANDOM_RESOURCE = "RANDOM_RESOURCE";

    // 열차 자체 (주로 속도 DEBUFF에 사용)
    public const string TRAIN = "TRAIN";

    // 모든 생산 시설 (ENGINE, FOOD, FACTORY 등)
    public const string ALL_PRODUCER = "ALL_PRODUCER";


    // ==========================================================
    // 3. 지속 시간 (Duration) 상수
    // ==========================================================

    // 이벤트가 해결되기 전까지 무한히 지속됨
    public const string DURATION_INFINITE = "INF";
}