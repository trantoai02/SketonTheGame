using Inventory;
using Inventory.Model;
using UnityEngine;

public class ThrowWeapon : MonoBehaviour, IWeapon, IItemAction
{
    public FakeHeightObject throwObjectPrefab;
    public Transform throwPoint;

    public Vector2 groundDispenseVelocity;
    public Vector2 verticalDispenseVelocity;

    [SerializeField] EquippableItemSO equippableItemInfo;

    public string ActionName => "Throw";
    public AudioClip actionSFX => null;

    
    public void Attack()
    {
        // Không có đá thì không bắn
        if (!ConsumeAmmo()) return;

        Debug.Log("Throw Rock");
        Shoot();
    }

    /// <summary>
    /// Trừ 1 item trong slot đang active
    /// </summary>
    bool ConsumeAmmo()
    {
        if (ActiveInventory.Instance == null) return false;

        int itemIndex = ActiveInventory.Instance.currentIndex - 1;

        InventorySO inventory = InventoryController.instance.inventoryData;
        InventoryItem item = inventory.GetItemAt(itemIndex);

        if (item.IsEmpty || item.quantity <= 0)
            return false;

        // Trừ 1
        inventory.RemoveItem(itemIndex, 1);

        return true;
    }

    void Shoot()
    {
        AudioManager.instance.PlaySFX("draffle_sound", transform);

        FakeHeightObject throwObject = Instantiate(throwObjectPrefab, throwPoint.position, Quaternion.identity);

        throwObject.Initialize(
            transform.right * Random.Range(groundDispenseVelocity.x, groundDispenseVelocity.y),
            Random.Range(verticalDispenseVelocity.x, verticalDispenseVelocity.y)
        );
    }

    public EquippableItemSO GetEquippableItemInfo() => equippableItemInfo;
    public FoodItemSO GetFoodItemInfo() => null;
    public ItemSO GetItemInfo() => equippableItemInfo;

    // Không dùng tới nhưng phải có vì implement interface
    public bool PerformAction(GameObject character, System.Collections.Generic.List<ItemParameter> itemState)
    {
        return false;
    }
}
