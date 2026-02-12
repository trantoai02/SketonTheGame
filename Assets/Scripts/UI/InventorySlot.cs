using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    public ItemSO itemInfo;

    public ItemSO GetItemInfo() { return itemInfo; }    
}
