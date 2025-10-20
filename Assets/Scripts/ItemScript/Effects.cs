using UnityEngine;
using static ItemEffectManager;

// 인터페이스 구현체 클래스 모음 : 인터페이스 종속, 생성자/add/remove/set 4함수 필수
// 자원 생산량 증가 효과
public class ResourceIncrease : ItemEffect
{
    private ResourceProducer rp;

    public ResourceIncrease(ResourceProducer target)
    {
        rp = target;  // 생성자 주입
    }

    /// <summary>
    /// 자원 생산량 증가 효과
    /// </summary>
    public void Add(ResourceType type, float value)
    {
        rp.AddCorrection(type, value);
    }

    /// <summary>
    /// 자원 생산량 감소 효과
    /// </summary>
    public void Remove(ResourceType type, float value)
    {
        rp.AddCorrection(type, -value);
    }

    /// <summary>
    /// 자원 생산량 수정 효과
    /// </summary>
    public void Set(ResourceType type, float value)
    {
        rp.SetCorrection(type, value);
    }
}
//최대리소스양 증가
public class MaxResourceIncrease : ItemEffect
{
    private ResourceManager rm;

    public MaxResourceIncrease(ResourceManager target)
    {
        rm = target;
    }

    public void Add(ResourceType type, float value)
    {
        Debug.Log("TODO : MaxResource 아이템 효과 적용");
    }
    public void Remove(ResourceType type, float value)
    {
    }
    public void Set(ResourceType type, float value)
    {
    }

}