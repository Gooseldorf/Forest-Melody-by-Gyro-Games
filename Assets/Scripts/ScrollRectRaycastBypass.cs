using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectRaycastBypass : MonoBehaviour, IBeginDragHandler,  IDragHandler, IEndDragHandler, IScrollHandler
{ 
    [HideInInspector] public ScrollRect scroll;

    public void OnBeginDrag(PointerEventData eventData)
    {
        scroll.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        scroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scroll.OnEndDrag(eventData);
    }
    
    public void OnScroll(PointerEventData data)
    {
        scroll.OnScroll(data);
    }
}
