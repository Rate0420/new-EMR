using UnityEngine;
using UnityEngine.EventSystems; // これが必要です

public class MouseHoverSelect : MonoBehaviour, IPointerEnterHandler
{
    // 引数の型を「PointerEventData」に修正しました
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
