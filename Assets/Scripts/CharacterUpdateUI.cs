using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUpdateUI : MonoBehaviour
{
    public static CharacterUpdateUI instance;

    public Image[] healthUpgradeBar, shieldUpgradeBar, staminaUpgradeBar, strengthUpgradeBar;


    public TMP_Text healthCostText;
    public TMP_Text shieldCostText;
    public TMP_Text staminaCostText;
    public TMP_Text strengthCostText;

    public TMP_Text levelText;
    public TMP_Text gemText;
    Color lockedColor;
    Color unLockedColor;
    Color upgradedColor;
    
    private void Awake()
    {
        instance = this;


        //set upgrade bar alpha color
        lockedColor = new Color(1,1,1,0);
        unLockedColor = new Color(1,1,1, 0.5f);
        upgradedColor = new Color(1, 1, 1, 1);


        for (int i = 0; i < PlayerStats.maxStatLevel; i++)
        {
            healthUpgradeBar[i].color = lockedColor;
            shieldUpgradeBar[i].color = lockedColor;
            staminaUpgradeBar[i].color = lockedColor;
            strengthUpgradeBar[i].color = lockedColor;
        }

    }

    private void Start()
    {
        UpdatePlayerStatsUI();

    }


    public void UpdatePlayerStatsUI()
    {

        healthCostText.text = PlayerStats.Instance.healthCost == 0 ? "MAX" : PlayerStats.Instance.healthCost.ToString();
        shieldCostText.text = PlayerStats.Instance.shieldCost == 0 ? "MAX" : PlayerStats.Instance.shieldCost.ToString();
        staminaCostText.text = PlayerStats.Instance.staminaCost == 0 ? "MAX" : PlayerStats.Instance.staminaCost.ToString();
        strengthCostText.text = PlayerStats.Instance.strengthCost == 0 ? "MAX" : PlayerStats.Instance.strengthCost.ToString();

        levelText.text = "LEVEL " + PlayerXPLevelUpManager.instance.level.ToString();
        gemText.text = EconomyManager.instance.currentCoin.ToString("D3");

        //health
        for (int i = 0;i < PlayerStats.Instance.maxHealthLevel;i++)
        {
           
            healthUpgradeBar[i].color = unLockedColor;
        }
        for (int i = 0; i < PlayerStats.Instance.healthLevel; i++)
        {
            healthUpgradeBar[i].color = upgradedColor;

        }

        //shield
        for (int i = 0; i < PlayerStats.Instance.maxShieldLevel; i++)
        {

            shieldUpgradeBar[i].color = unLockedColor;
        }
        for (int i = 0; i < PlayerStats.Instance.shieldLevel; i++)
        {
            shieldUpgradeBar[i].color = upgradedColor;

        }
        //stamina
        for (int i = 0; i < PlayerStats.Instance.maxStaminaLevel; i++)
        {

            staminaUpgradeBar[i].color = unLockedColor;
        }
        for (int i = 0; i < PlayerStats.Instance.staminaLevel; i++)
        {
            staminaUpgradeBar[i].color = upgradedColor;

        }
        //strength
        for (int i = 0; i < PlayerStats.Instance.maxStrengthLevel; i++)
        {

            strengthUpgradeBar[i].color = unLockedColor;
        }
        for (int i = 0; i < PlayerStats.Instance.strengthLevel; i++)
        {
            strengthUpgradeBar[i].color = upgradedColor;

        }
    }
}
