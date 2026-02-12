using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IWeapon
{
    public void Attack();
    //public WeaponInfo GetWeaponInfo();
    public EquippableItemSO GetEquippableItemInfo();
    public ItemSO GetItemInfo();
    public FoodItemSO GetFoodItemInfo();

   


}
