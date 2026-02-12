
using Inventory;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{

    public GameObject[] tabs;
    public Image[] tabButtons;
    public Sprite InactiveTabBG, ActiveTabBG;
    public Vector2 InactiveTabBtnSize, ActiveTabBtnSize;

    public void SwitchToTab(int tabIndex)
    {
        Debug.Log(tabIndex);
        foreach (GameObject tab in tabs)
        {
            tab.SetActive(false);
        }
        tabs[tabIndex].SetActive(true);

        if(InactiveTabBG != null || ActiveTabBG != null)
        {
            foreach (Image image in tabButtons)
            {
                image.sprite = InactiveTabBG;
                image.rectTransform.sizeDelta = InactiveTabBtnSize;
            }
            tabButtons[tabIndex].sprite = ActiveTabBG;
            tabButtons[tabIndex].rectTransform.sizeDelta = ActiveTabBtnSize;
        }
       
    }
}
