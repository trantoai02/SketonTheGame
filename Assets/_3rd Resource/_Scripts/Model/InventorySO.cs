using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Inventory.Model
{
    [Serializable]
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject, ISerializationCallbackReceiver
    {

        public string savePath;

        private ItemDatabase database;

        private void OnEnable()
        {
#if UNITY_EDITOR
            database = (ItemDatabase)AssetDatabase.LoadAssetAtPath("Assets/Resources/ItemDatabase.asset", typeof(ItemDatabase));
#else
        database = Resources.Load<ItemDatabase>("ItemDatabase");
#endif
        }


        [SerializeField]
        public List<InventoryItem> inventoryItems = new List<InventoryItem>();

        [field: SerializeField]
        public int Size { get; private set; } = 15;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;

        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            if (item.IsStackable == false)
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {

                    while (quantity > 0 && IsInventoryFull() == false)
                    {
                        quantity -= AddItemToFirstFreeSlot(item, 1, itemState);

                    }
                    InformAboutChange();
                    return quantity;

                }
            }
            quantity = AddStackableItem(item, quantity);
            InformAboutChange();

            return quantity;
        }
        private int AddItemToFirstFreeSlot(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
           
            InventoryItem newItem = new InventoryItem
            {
                item = item,
                quantity = quantity,
                itemState = new List<ItemParameter>(itemState
                == null ? item.DefaultParametersList : itemState)
            };

            for (int i = 0; i < inventoryItems.Count; i++)
            {

                if (inventoryItems[i].IsEmpty)
                {
                    int newID = database.GetId[newItem.item];
                    newItem.ID = newID;
                    inventoryItems[i] = newItem;
                    return quantity;
                }

            }
            return 0;
        }

        
        private int AddStackableItem(ItemSO item, int quantity)
        {
            int itemID = database.GetId[item];

            // --- BƯỚC 1: TÌM STACK CÓ SẴN ---
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                    continue;

                if (inventoryItems[i].ID == itemID)
                {
                    int amountPossibleToTake =
                        inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;

                    if (amountPossibleToTake <= 0)
                        continue;

                    if (quantity > amountPossibleToTake)
                    {
                        inventoryItems[i] = inventoryItems[i]
                            .ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                    }
                    else
                    {
                        inventoryItems[i] = inventoryItems[i]
                            .ChangeQuantity(inventoryItems[i].quantity + quantity);
                        InformAboutChange();
                        return 0;
                    }
                }
            }

            // --- BƯỚC 2: KHÔNG CÓ STACK → TẠO SLOT MỚI ---
            while (quantity > 0 && IsInventoryFull() == false)
            {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity);
            }

            InformAboutChange();
            return quantity;
        }


        private bool IsInventoryFull()
        => inventoryItems.Where(x => x.IsEmpty).Any() == false;

        // danh sách các "chỉ mục - vật phẩm" trong túi đồ
        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue =
                new Dictionary<int, InventoryItem>();

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    continue;
                }
                returnValue[i] = inventoryItems[i];
            }
            return returnValue;
        }
        public InventoryItem GetItemAt(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= Size)
            {
                Debug.LogError("Item index out of range");
                return InventoryItem.GetEmptyItem();
            }
            return inventoryItems[itemIndex];
        }

        public void SwapItems(int itemIndex1, int itemIndex2)
        {
            if (itemIndex1 < 0 || itemIndex1 >= Size || itemIndex2 < 0 || itemIndex2 >= Size)
            {
                Debug.LogError("Item index out of range");
                return;
            }

            InventoryItem item1 = inventoryItems[itemIndex1];
            inventoryItems[itemIndex1] = inventoryItems[itemIndex2];
            inventoryItems[itemIndex2] = item1;
            InformAboutChange();
        }

        
        public void InformAboutChange()
        {
            OnInventoryChanged?.Invoke(GetCurrentInventoryState());
            ActiveInventory.Instance.UpdateActiveInventoryData();
        }

        public void RemoveItem(int itemIndex, int amount)
        {
            if (itemIndex < 0 || itemIndex >= Size)
            {
                Debug.LogError("Item index out of range");
                return;
            }

            if (inventoryItems[itemIndex].IsEmpty)
            {
                return;
            }
            int remainder = inventoryItems[itemIndex].quantity - amount;
            if (remainder <= 0)
                inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
            else
                inventoryItems[itemIndex] = inventoryItems[itemIndex]
                    .ChangeQuantity(remainder);
            InformAboutChange();

        }
        public void OnBeforeSerialize()
        {

        }

        //có liên kết với itemDatabase
        public void OnAfterDeserialize()
        {
            try
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    //---BƯỚC 1: ĐỒNG BỘ ITEM VÀ ID
                    // inventoryItems[i].item = database.GetItem[inventoryItems[i].ID];
                    // Lấy ra ItemSO mới từ ID

                    ItemSO newItem = database.GetItem[inventoryItems[i].ID];
                    int newID = database.GetId[newItem];


                    InventoryItem updatedItem = inventoryItems[i];

                    // đồng bộ item và id
                    updatedItem.item = newItem;
                    updatedItem.ID = newID;


                    //---BƯỚC 2: ĐỒNG BỘ ITEM STATE CỦA MỖI ITEM
                    // tạo danh sách tạm, lưu giữ danh sách các state
                    List<ItemParameter> updatedItemState = new List<ItemParameter>();


                    for (int j = 0; j < updatedItem.itemState.Count; j++)
                    {
                        ItemParameterSO parameterSO = database.GetParameterSO[inventoryItems[i].itemState[j].itemStateID];

                        ItemParameter newItemParameter = new ItemParameter
                        {
                            itemParameter = parameterSO,
                            value = inventoryItems[i].itemState[j].value
                        };

                        updatedItemState.Add(newItemParameter);

                    }

                    updatedItem.itemState = updatedItemState;

                    //---BƯỚC 3: cập nhật item tạm vào inventoryItems


                    inventoryItems[i] = updatedItem;
                }
            }
            catch
            {
                return;
            }
            
        }
    }
    [Serializable]
    public struct InventoryItem
    {
        public int ID;
        public int quantity;
        public ItemSO item;
        public List<ItemParameter> itemState;
        

        public InventoryItem(int id, ItemSO item, int quantity, List<ItemParameter> state = null) : this()
        {
            this.ID = id;
            this.item = item;
            this.quantity = quantity;

            this.itemState = state;

        }

        public bool IsEmpty => quantity == 0;

        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem
            {
                ID = this.ID,
                item = this.item,
                quantity = newQuantity,
                itemState = new List<ItemParameter>(this.itemState)
            };
        }

        public static InventoryItem GetEmptyItem() => new InventoryItem
        {
            //ID = 0,
            //item = null,
            //quantity = 0,
            //itemState = new List<ItemParameter>()
        };


    }
}

