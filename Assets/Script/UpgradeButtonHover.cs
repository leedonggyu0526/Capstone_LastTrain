using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TrainUpgradeManager upgradeManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        upgradeManager.ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        upgradeManager.HideTooltip();
    }
}
