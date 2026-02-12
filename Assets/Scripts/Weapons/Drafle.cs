
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

namespace Inventory.Model
{


    public class Drafle : MonoBehaviour, IWeapon, IItemAction
    {
      //  [SerializeField] WeaponInfo weaponInfo;
        [SerializeField] EquippableItemSO equippableItemInfo;



        public GameObject melon;
        public Vector2 groundDispenseVelocity;
        public Vector2 verticalDispenseVelocity;

        //References
        public Transform trnsGun;
        public Transform trnsGunTip;

        public string ActionName => throw new System.NotImplementedException();

        public AudioClip actionSFX => throw new System.NotImplementedException();

   

        public void Attack()
        {
            Debug.Log("Drafle Attack");
            Shoot();

        }

        //public WeaponInfo GetWeaponInfo()
        //{
        //    return weaponInfo;
        //}
        public EquippableItemSO GetEquippableItemInfo()
        {
            return equippableItemInfo;
        }


        void Shoot()
        {
            AudioManager.instance.PlaySFX("draffle_sound", transform);
            GameObject instantiatedMelon = ObjectPooler.Instance.SpawnFromPool("Nut", trnsGunTip.position, Quaternion.identity);
            //GameObject instantiatedMelon = Instantiate(melon, trnsGunTip.position, Quaternion.identity);
            instantiatedMelon.GetComponent<FakeHeightObject>().Initialize(trnsGun.right
                * Random.Range(groundDispenseVelocity.x, groundDispenseVelocity.y),
                Random.Range(verticalDispenseVelocity.x, verticalDispenseVelocity.y));

        }

        public bool PerformAction(GameObject character, List<ItemParameter> itemState)
        {
            throw new System.NotImplementedException();
        }

        public ItemSO GetItemInfo()
        {
            return equippableItemInfo;
        }

        public FoodItemSO GetFoodItemInfo()
        {
            return null;
        }

     
    }
}
