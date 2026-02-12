using Inventory.Model;
using System;
using System.Collections;
using UnityEngine;



public class Item : MonoBehaviour
{
    [field: SerializeField] public ItemSO InventoryItem { get; private set; }
    [field: SerializeField] public int Quantity { get; set; } = 1;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float duration = 0.3f;
    public string itemName;

    private bool isPickedUp = false;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = InventoryItem.ItemImage;

        if (PlayerPrefs.GetInt(itemName, 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    public bool TryPickUp()
    {
        if (isPickedUp) return false;
        isPickedUp = true;
        return true;
    }

    public void DestroyItem()
    {
        StartCoroutine(AnimatePickup());
        if (itemName != "")
        {
            PlayerPrefs.SetInt(itemName, 1);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator AnimatePickup()
    {
        audioSource.Play();
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
