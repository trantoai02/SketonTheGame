using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class UIInventoryDescription : MonoBehaviour
    {
        [SerializeField]
        Image itemImage;

        [SerializeField]
        TMP_Text title;

        [SerializeField]
        TMP_Text description;

        private void Awake()
        {
            ResetDescription();

        }

        public void ResetDescription()
        {
            itemImage.gameObject.SetActive(false);
            title.text = "";
            description.text = "";

        }

        public void SetDescription(Sprite sprite, string itemName,
            string itemDescription)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            title.text = itemName;
            description.text = itemDescription;
        }
    }
}