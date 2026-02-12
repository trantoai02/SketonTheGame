using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory.Model
{
    public class Cherry : MonoBehaviour
    {
        [SerializeField] public FoodItemSO foodItemInfo;

        public void Attack()
        {
            Debug.Log("food1");
        }

        public EquippableItemSO GetEquippableItemInfo()
        {
            return null;
        }

        public FoodItemSO GetFoodItemInfo() { return foodItemInfo; }

        public ItemSO GetItemInfo()
        {
            return foodItemInfo;
        }
    }

}
