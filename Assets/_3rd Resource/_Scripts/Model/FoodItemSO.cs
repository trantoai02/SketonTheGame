using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [Serializable]
    [CreateAssetMenu]
    public class FoodItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifierDatas = new List<ModifierData>();

        public string ActionName => "Consume";

        [field: SerializeField]
        public AudioClip actionSFX {get; private set;}

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            //foreach (ModifierData data in modifierDatas)
            //{
            //    data.statModifier.AffectCharacter(character, data.value);
            //}
            PlayerHealth.Instance.HealPlayer((int)modifierDatas[0].value);

            return true;
        }
    }

    public interface IDestroyableItem
    {
        // chỉ dành cho việc ám chỉ vật phẩm có thể bỏ đi sau khi đã trang bị
    }
    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
        bool PerformAction(GameObject character, List<ItemParameter> itemState);
    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifier;
        public float value;
    }
}