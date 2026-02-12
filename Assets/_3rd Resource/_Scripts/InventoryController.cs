using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        // tạo thể hiện duy nhất
        public static InventoryController instance;

        public string savePath;

        [SerializeField]
        UIInventoryPage inventoryUI;

        [SerializeField]
        public InventorySO inventoryData;


        [SerializeField]
        AudioClip dropClip;

        [SerializeField]
        AudioSource audioSource;

        CustomInput input;

        public TabManager tabManager;

        // tham chiếu menu (inspector)
        public GameObject playerMenu;
        public GameObject weaponCollider;

        //private string saveFilePath;

        private void Awake()
        {
            //instance
            instance = this;

            //input

            input = new CustomInput();
            input.Inventory.OpenInventory.performed += OpenInventory_performed;
            input.Inventory.OpenInventory.canceled += OpenInventory_canceled;

            LoadFile(saveFileName);
            PrepareUI();
        }

        //dành cho input system
        private void OnEnable()
        {
            input.Enable();
        }
        private void OnDisable()
        {
            input.Disable();
        }

       public void Save()
        {
            SaveFile(saveFileName);
        }

        private void OpenInventory_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            SaveFile(saveFileName);
            ActiveInventory.Instance.UpdateActiveInventoryData();
        }

      


        public void PrepareInventoryUI()
        {
            if (inventoryData == null || inventoryUI == null)
            {
                Debug.Log("inventoryData hoặc inventoryUI chưa được khởi tạo");
                return;
            }
            foreach (var item in inventoryData.GetCurrentInventoryState())
            {
                // gọi sang inventoryUI - cập nhật hình ảnh thông tin các vật phẩm trong túi đồ
                inventoryUI.UpdateData(item.Key,
                    item.Value.item.ItemImage,
                    item.Value.quantity);
            }
        }
        private void OpenInventory_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            // tạm dừng trò chơi khi bật các thanh menu
            Debug.Log("open inventory!");
            if (!playerMenu.activeSelf)
            {
                Time.timeScale = 0f;
                playerMenu.SetActive(true);
                inventoryUI.Show();
                PrepareInventoryUI();

                if (tabManager != null)
                {
                    tabManager.SwitchToTab(1);
                }
                else
                {
                    Debug.Log("tabManager is null.");
                }
            }
            else
            {
                Time.timeScale = 1f;

                playerMenu.SetActive(false);
                inventoryUI.Hide();
                PrepareInventoryUI();

                if (tabManager != null)
                {
                    tabManager.SwitchToTab(1);
                }
                else
                {
                    Debug.Log("tabManager is null.");
                }
            }
        }

        [SerializeField]
        private static string saveFileName = "/playerInventory.txt";

        // lưu và tải qua file
        public void SaveFile(string filePath)
        {
            string saveData = JsonUtility.ToJson(inventoryData, true);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Create(string.Concat(Application.persistentDataPath, filePath));
            bf.Serialize(fs, saveData);
            fs.Close();
        }
        public void LoadFile(string filePath)
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, filePath)))
            {
                //inventoryData.Initialize();
                inventoryData.OnInventoryChanged += UpdateInventoryUI;

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(string.Concat(Application.persistentDataPath, filePath), FileMode.Open);
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), inventoryData);
                file.Close();
            }
        }

        public void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                    item.Value.quantity);
            }
        }

        private void PrepareUI()
        {
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
            inventoryUI.InitializeInventoryUI(inventoryData.Size);

        }

        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                return;
            }

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryUI.ShowItemAction(itemIndex);

                if (inventoryItem.item is FoodItemSO)
                {
                    inventoryUI.AddAction("Drop", () => DropItem(itemIndex, 1));
                }
                else
                {
                    inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
                }
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
               // inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }
        }

        private void DropItem(int itemIndex, int quantity)
        {
            inventoryData.OnInventoryChanged += UpdateInventoryUI;
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            weaponCollider.SetActive(false);
            audioSource.PlayOneShot(dropClip);
        }

        public void PerformAction(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                return;
            }

            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }

            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                {
                    inventoryUI.ResetSelection();
                }
            }
        }
        private void HandleDragging(int itemIndex)
        {

            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                return;
            }
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage,
                inventoryItem.quantity);
        }

        private void HandleSwapItems(int itemIndex1, int itemIndex2)
        {
            inventoryData.OnInventoryChanged += UpdateInventoryUI;

            inventoryData.SwapItems(itemIndex1, itemIndex2);
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= inventoryData.Size)
            {
                Debug.LogError("Item index out of range");
                return;
            }
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;

            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage,
                item.name, description);
        }

        public string PrepareDescription(InventoryItem inventoryItem)
        {
       
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();
            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                Debug.Log(i.ToString() + " + " + inventoryItem.itemState.Count.ToString());
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName}" +
                    $" : {inventoryItem.itemState[i].value} / " + 
                    $" {inventoryItem.item.DefaultParametersList[i].value}");

                //ex: Durability : 60/100
            }
            return sb.ToString();
        }


        // gọi khi đánh trúng quái vật
        public void ModifyCurrentWeaponParameters()
        {
            // chỉ định item trong Player Inventory thông qua vị trí ô vật phẩm đang được chọn trên Active Inventory
            int itemIndex = ActiveInventory.Instance.currentIndex - 1;
            InventoryItem item = inventoryData.GetItemAt(itemIndex);

            //trừ 1 độ bền của vật phẩm
            ItemParameter a = item.itemState[0];
            a.value -= 1;
            item.itemState[0] = a;

            // xóa bỏ vật phẩm trong túi đồ khi độ bền giảm xuống bằng 0
            if (item.itemState[0].value <= 0)
            {
                DropItem(ActiveInventory.Instance.currentIndex - 1, item.quantity);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SaveFile(saveFileName);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                LoadFile(saveFileName);
            }
        }

    }
}