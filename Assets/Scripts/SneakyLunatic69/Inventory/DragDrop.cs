using UnityEngine;
using UnityEngine.EventSystems;

namespace SneakyLunatic.Inventory
{
    public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {

        private RectTransform rectTransform;
        private Canvas canvas;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / CanvasGlobal.SCALE_FACTOR;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvas.overrideSorting = false;
            canvas.sortingOrder = 0;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            
            //Snap the item back to start position
            //If the item's slot has changed this will snap to the new parent's position
            rectTransform.anchoredPosition = Vector3.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Necessary interface for drag events
        }
    }
}
