using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [Serializable]
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem 
    {
        [field: SerializeField]
        public AudioClip actionSFX {get; private set;}

        public float weaponCooldown;

        public int weaponDamage;

        public float weaponRange;

    }
}