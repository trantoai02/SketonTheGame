using UnityEngine;
using System.Collections.Generic;

namespace Inventory.Model
{
    [CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory/Item Database", order = 1)]
    public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
    {
        public ItemSO[] items;
        public ItemParameterSO[] parameters;
        public Dictionary<ItemSO, int> GetId = new Dictionary<ItemSO, int>();
        public Dictionary<int, ItemSO> GetItem = new Dictionary<int, ItemSO>();

        public Dictionary<int, ItemParameterSO> GetParameterSO = new Dictionary<int, ItemParameterSO>();

        public void OnAfterDeserialize()
        {
           GetId = new Dictionary<ItemSO, int>();
           GetItem = new Dictionary<int, ItemSO>();
            for (int i = 0; i < items.Length; i++)
            {
                GetId.Add(items[i], i);
                GetItem.Add(i, items[i]);
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                GetParameterSO.Add(i, parameters[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            
        }
    }
}
