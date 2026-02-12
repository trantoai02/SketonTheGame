
using Inventory.UI;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    //[SerializeField]
    public Canvas canvas;

    //[SerializeField]

    public UIInventoryItem item;

    private void Awake()
    {

        //truy cap den thu muc goc - root, la Canvas
       // canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //item = GetComponentInChildren<UIInventoryItem>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        item.SetData(sprite, quantity);
    }

    private void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition, canvas.worldCamera, out position);
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool value)
    {
        Debug.Log($"Item toggled {value}");


        gameObject.SetActive(value);
    }
}
