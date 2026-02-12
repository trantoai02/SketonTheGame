using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    // [SerializeField] WeaponInfo weaponInfo;
    [SerializeField] EquippableItemSO equippableItemInfo;
    public void Attack()
    {
        Debug.Log("Rock Attack");
   
    }

    //public WeaponInfo GetWeaponInfo()
    //{
    //    return weaponInfo;
    //}

    public EquippableItemSO GetEquippableItemInfo()
    {
        return equippableItemInfo;
    }

}
