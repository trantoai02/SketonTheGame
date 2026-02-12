using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler,
        IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private bool isLongPressing = false;
        private float longPressDuration = 0.5f;
        private float pressTime = 0f;

        [SerializeField]
        Image itemImage;

        [SerializeField]
        TMP_Text quantityText;

        [SerializeField]
        Image borderImage;

        public event Action<UIInventoryItem>
            OnItemClicked,
            OnItemDroppedOn,
            OnItemBeginDrag,
            OnItemEndDrag,
            OnRightMouseButtonClick;

        bool empty = true;

         private Coroutine longPressCoroutine;

        private void Awake()
        {
            ResetData();
            Deselect();
        }

        public void Deselect()
        {
            if(borderImage != null)
                borderImage.enabled = false;
        }

        public void ResetData()
        {
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(false);
            }
            empty = true;
        }

        public void SetData(Sprite sprite=null, int quantity=0)
        {
           
           if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = sprite;
            }
            if (quantityText != null)
            {
                quantityText.text = quantity.ToString();
            }
            empty = false;
        }

        public void Select()
        {
            borderImage.enabled = true;
        }

        public void OnPointerClick(PointerEventData pointerData)
        {

            if (!isLongPressing)
            {
                if (pointerData.button == PointerEventData.InputButton.Right)
                {
                    OnRightMouseButtonClick?.Invoke(this);
                }
                else
                {
                    OnItemClicked?.Invoke(this);
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty)
            {
                return;
            }

            if (longPressCoroutine != null)
            {
                StopCoroutine(longPressCoroutine);
                longPressCoroutine = null;
                isLongPressing = false;
            }

            OnItemBeginDrag?.Invoke(this);
        }
       

        public void OnEndDrag(PointerEventData eventData)
        {

            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {

            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("nhan giu");
            pressTime = Time.unscaledTime;
            isLongPressing = false;
            // bắt đầu tính thời gian giữ lâu khi người dùng nhấn
            longPressCoroutine = StartCoroutine(LongPressCoroutine());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // kiểm tra nếu coroutine đang chạy, dừng lại
            if (longPressCoroutine != null)
            {
                StopCoroutine(longPressCoroutine);
                longPressCoroutine = null;
            }

            if (!isLongPressing && Time.unscaledTime - pressTime < longPressDuration)
            {
                // Xử lý sự kiện click bình thường nếu không phải long press
                OnPointerClick(eventData);
            }
            //OnPointerClick(eventData);
            
        }
        IEnumerator LongPressCoroutine()
        {
        // Đợi cho đến khi thời gian giữ lâu đủ lâu
        yield return new WaitForSecondsRealtime(longPressDuration);
        // Gọi sự kiện right click
        OnRightMouseButtonClick?.Invoke(this);
        isLongPressing = true;
    }

       

     
    }
}