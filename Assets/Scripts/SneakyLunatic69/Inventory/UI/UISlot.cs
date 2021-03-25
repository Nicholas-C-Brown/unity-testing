using System;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable CS0649
namespace SneakyLunatic.Inventory.UI
{
    public class UISlot : MonoBehaviour, IDropHandler
    {
        [SerializeField]
        private GameObject uiItemPrefab;

        private UIItem uiItem;
        public int ID { get; set; }

        public Action<int, int, Inventory> ItemDropped;

        private void Awake()
        {
            uiItem = GetComponentInChildren<UIItem>();
        }

        public UIItem GetUIItem()
        {
            return uiItem;
        }

        public void SetUIItem(Item item, int amount)
        {
            if(uiItem != null)
            {
                Destroy(uiItem.gameObject);
            }

            if (item == null) return;

            GameObject instance = Instantiate(uiItemPrefab, transform);
            instance.transform.position = new Vector3(instance.transform.position.x, instance.transform.position.y, 1);
            uiItem = instance.GetComponent<UIItem>();

            uiItem.SetAmount(amount);

            uiItem.Icon.sprite = item.Icon;
        }

        public void OnDrop(PointerEventData eventData)
        {
            Transform dragged = eventData.pointerDrag.transform;

            if (dragged != null)
            {
                UISlot startSlot = dragged.parent.GetComponent<UISlot>();
                Inventory inventory = startSlot.GetComponentInParent<UIInventory>().GetInventory();

                ItemDropped?.Invoke(startSlot.ID, ID, inventory);
            }
        }
    }
}
