
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class ActiveInventorySlotItem : MonoBehaviour, IPointerClickHandler
{
    public int id;
    CustomInput input;
    private void Awake()
    {
        input = new CustomInput();

        input.Inventory.Scrolling.performed += Scrolling_performed;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Scrolling_performed(InputAction.CallbackContext obj)
    {
        int val = (int)obj.ReadValue<float>();
        if (val > 0)
        {
            id--;
        }
        else if (val < 0)
        {
            id++;
        }

        int slotCount = ActiveInventory.Instance.transform.childCount;
        if (id < 1)
        {
            id = slotCount;
        }
        else if (id > slotCount)
        {
            id = 1;
        }

        ActiveInventory.Instance.ToggleActiveSlot(id);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("item slot " + id + " clicked!");
        ActiveInventory.Instance.ToggleActiveSlot(id);
    }

  
}
