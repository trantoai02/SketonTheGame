using Inventory;
using Inventory.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveInventory : MonoBehaviour
{
    public static ActiveInventory Instance;


    int activeSlotIndexNumber = 0;

    CustomInput playerControls;

    public InventorySO inventoryData;

    public List<InventoryItem> inventoryItems;

    public Transform aimTransform;

    public int currentIndex;

    public InventoryController inventoryController;

    private void Awake()
    {
        Instance = this;
        playerControls = new CustomInput();

        //khởi tạo danh sách tạm, lấy từ Player Inventory
        inventoryItems = new List<InventoryItem>();

        UpdateActiveInventoryData();

        // mặc định chọn ô vật phẩm đầu tiên
        // số 1 ở đây tương ứng với vị trí đầu tiên
        currentIndex = 1;


        if (currentIndex > 0 && currentIndex <= transform.childCount)
        {
            playerControls.Inventory.Keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());
        }
        else
        {
            return;
        }
    }

    private void Start()
    {
        if (inventoryData == null)
        {
            Debug.Log("inventoryData rỗng");
            return;
        }
        if (inventoryData.inventoryItems == null || inventoryData.inventoryItems.Count == 0)
        {
            Debug.Log("inventoryData.inventoryItems rỗng");
            return;
        }

    }

    private void OnApplicationQuit()
    {
            UpdateActiveInventoryData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UpdateActiveInventoryData();
        }
    }

    //tạo 1 danh sách tạm để tải ảnh 5 vật phẩm đầu tiên trong Player Inventory vào Active Inventory các ô truy cập nhanh
    public void UpdateActiveInventoryData()
    {
        inventoryItems.Clear();
        for (int i = 0; i < 5; i++)
        {
            //tải lên danh sách tạm
            inventoryItems.Add(new InventoryItem());
            if (inventoryData.inventoryItems[i].IsEmpty || inventoryData.inventoryItems[i].quantity ==0 )
            {

                transform.GetChild(i).gameObject.transform.GetChild(1).GetComponent<Image>().sprite = null;
                transform.GetChild(i).gameObject.transform.GetComponent<InventorySlot>().itemInfo = null;
                //inventoryItems[i] = new InventoryItem(-1, null, 0, null);
                continue;

            }

            inventoryItems[i] = new InventoryItem(inventoryData.inventoryItems[i].ID, inventoryData.inventoryItems[i].item, inventoryData.inventoryItems[i].quantity, inventoryData.inventoryItems[i].itemState);

            transform.GetChild(i).gameObject.GetComponent<InventorySlot>().itemInfo = inventoryItems[i].item;

            transform.GetChild(i).gameObject.transform.GetChild(1).GetComponent<Image>().sprite = inventoryItems[i].item.ItemImage;
        }

        inventoryController.Save();

        ToggleActiveSlot(currentIndex);
    }

    private void OnEnable()
    {
        playerControls.Enable();

    }

    private void OnDisable()
    {
        playerControls.Disable();

    }

    //hàm này còn được gọi ở ActiveInventorySlotItem để đổi slot bằng cách lăn chuột
    public void ToggleActiveSlot(int numValue)
    {
        // chỉ số index đầu tiên là 0

        if (numValue <= 0 || numValue > transform.childCount)
        {
           // Debug.LogError("Invalid slot number.");
            return;
        }
        ToggleActiveHighlight(numValue - 1);
        currentIndex = numValue;
    }

    void ToggleActiveHighlight(int indexNum)
    {
        // chỉ số index đầu tiên là 0

        if (indexNum < 0 || indexNum > transform.childCount)
        {
            Debug.LogError("Index number is out of range.");
            return;
        }
        activeSlotIndexNumber = indexNum;

        foreach (Transform inventorySlot in this.transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }
        //ActiveInventory --> Ô vật phẩm (index) --> Khung viền ô vật phẩm (true - bật khung viền)
        //hiển thị khung viền tại vị trí ô vật phẩm đang chọn
        this.transform.GetChild(indexNum).GetChild(0).gameObject.SetActive(true);

        ChangeActiveWeapon();
    }

    public void ChangeActiveWeapon()
    {
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }

        InventorySlot activeSlot = transform.GetChild(activeSlotIndexNumber).GetComponentInChildren<InventorySlot>();
        if (activeSlot == null || activeSlot.GetItemInfo() == null)
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }

        GameObject weaponToSpawn = activeSlot.GetItemInfo()?.itemPrefab;


        // Spawn weapon------------------------
        if (weaponToSpawn != null)
        {
            GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity, ActiveWeapon.Instance.gameObject.transform);
 
            newWeapon.transform.localScale = transform.parent.localScale;
            newWeapon.transform.rotation = ActiveWeapon.Instance.transform.rotation;
            ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
        }
        else
        {
            ActiveWeapon.Instance.WeaponNull();
        }
    }

    public void Hide()
    {
        UpdateActiveInventoryData();
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);

        }
    }

    public void Show()
    {
        UpdateActiveInventoryData();
        if (!gameObject.activeSelf)

            gameObject.SetActive(true);
    }
}