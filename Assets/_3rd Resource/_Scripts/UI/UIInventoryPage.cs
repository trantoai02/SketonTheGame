using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        //khai bao cac prefab UI
        //item UI
        [SerializeField]
        private UIInventoryItem itemPrefab;

        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        UIInventoryDescription itemDescription;

        List<UIInventoryItem> listOfItems = new List<UIInventoryItem>();


        //drag index mặc định - không có drag: -1
        int currentlyDraggedItemIndex = -1;

        public event Action<int> OnDescriptionRequested,
            OnItemActionRequested,
            OnStartDragging;

        public event Action<int, int> OnSwapItems;


        //nhằm hiển thị hiểu ứng vật phẩm đi theo con trỏ chuột khi drag
        [SerializeField]
        MouseFollower mouseFollower;

        [SerializeField]
        ItemActionPanel actionPanel;
        private void Awake()
        {
     

            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }
        public void InitializeInventoryUI(int inventorySize)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                UIInventoryItem uiItem =
                    Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, contentPanel);
                
               

                //nhận các phương thức cho sự kiện
                uiItem.OnItemClicked += HandleItemSelection;
                
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnRightMouseButtonClick += HandleShowItemActions;

                // uiItem.transform.SetParent(contentPanel);

                uiItem.gameObject.transform.localScale = Vector3.one;
                listOfItems.Add(uiItem);
            }
        }

        private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {
            int index = listOfItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {

                return;
            }
            OnItemActionRequested?.Invoke(index);
        }

        public void UpdateData(int itemIndex,
            Sprite itemImage, int itemQuantity)
        {
            if (listOfItems.Count > itemIndex )
            {

                if (itemQuantity == 0)
                {
                    // Nếu quantity = 0, hiển thị mục không có hình ảnh và không có số lượng
                    listOfItems[itemIndex].ResetData();
                }
                else
                {
                    listOfItems[itemIndex].SetData(itemImage, itemQuantity);
                }


            }

        }

        private void HandleSwap(UIInventoryItem inventoryItemUI)
        {
            int index = listOfItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {

                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(inventoryItemUI);
        }

        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }
        
        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDraggedItem();
            

        }

        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            int index = listOfItems.IndexOf(inventoryItemUI);
            if (index == -1)
                return;
            currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryItemUI);
            OnStartDragging?.Invoke(index);
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }

        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
           
            int index = listOfItems.IndexOf(inventoryItemUI);

            if (index < 0 || index >= listOfItems.Count)
            {
                return;
            }
            OnDescriptionRequested?.Invoke(index);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ActiveInventory.Instance.Hide();
            ResetSelection();
        }

        public void ResetSelection()
        {
           
            itemDescription.ResetDescription();
            DeselectAllItems();
        }

        public void AddAction(string actionName, Action performAction)
        {
            actionPanel.AddButton(actionName, performAction);
        }

        public void ShowItemAction(int itemIndex)
        {
            actionPanel.Toggle(true);
            actionPanel.transform.position = listOfItems[itemIndex].transform.position;
        }

        private void DeselectAllItems()
        {
            foreach (UIInventoryItem item in listOfItems)
            {
                item.Deselect();
            }
            actionPanel.Toggle(false);
        }

        public void Hide()
        {
            ActiveInventory.Instance.Show();
            actionPanel.Toggle(false);
            gameObject.SetActive(false);
            ResetDraggedItem();
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            listOfItems[itemIndex].Select();
        }

        internal void ResetAllItems()
        {
            foreach (var item in listOfItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        private void Update()
        {
            // Nếu actionPanel đang mở và click chuột trái
            if (actionPanel.gameObject.activeSelf && Input.GetMouseButtonDown(0))
            {
                // Lấy vị trí chuột
                Vector2 mousePosition = Input.mousePosition;

                // Kiểm tra xem chuột có nằm ngoài vùng actionPanel không
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                        actionPanel.GetComponent<RectTransform>(),
                        mousePosition,
                        null))
                {
                    // Ẩn panel
                    actionPanel.Toggle(false);
                }
            }
        }

    }
}