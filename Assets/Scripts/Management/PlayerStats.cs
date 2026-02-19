using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;


    public int health;
    public float shield;
    public float stamina;
    public int strength;

    public int healthLevel = 0;
    public int shieldLevel = 0;
    public int staminaLevel = 0;
    public int strengthLevel = 0;

    public int maxHealthLevel = 0;
    public int maxShieldLevel = 0;
    public int maxStaminaLevel = 0;
    public int maxStrengthLevel = 0;

    public const int maxStatLevel = 7;


    public int healthCost;
    public int shieldCost;
    public int staminaCost;
    public int strengthCost;

    public int[] healthUpgradeCost = { 10, 25, 50, 80, 120, 160, 200};
    public int[] shieldUpgradeCost = { 10, 25, 50, 80, 120, 160, 200};
    public int[] staminaUpgradeCost = { 10, 25, 50, 80, 120, 160, 200};
    public int[] strengthUpgradeCost = { 10, 25, 50, 80, 120, 160, 200};

    private void Awake()
    {
        Instance = this;

        LoadPlayerStatFromPlayPref();

        UpdateLevelCost();
    }

    private void OnApplicationQuit()
    {
        SavePlayerStatToPlayPref();
    }

    public void LoadPlayerStatFromPlayPref()
    {
        //Player's current stats
        health = PlayerPrefs.GetInt("playerHealth", 3);
        shield = PlayerPrefs.GetFloat("playerShield", 10);
        stamina = PlayerPrefs.GetFloat("playerStamina", 10);
        strength = PlayerPrefs.GetInt("playerStrength", 1);

       

        //Player current upgrade
        healthLevel = PlayerPrefs.GetInt("healthLevel", 0);
        shieldLevel = PlayerPrefs.GetInt("shieldLevel", 0);
        staminaLevel = PlayerPrefs.GetInt("staminaLevel", 0);
        strengthLevel = PlayerPrefs.GetInt("strengthLevel", 0);
        //Player current max upgrade
        maxHealthLevel = PlayerPrefs.GetInt("maxHealthLevel", 0);
        maxShieldLevel = PlayerPrefs.GetInt("maxShieldLevel", 0);
        maxStaminaLevel = PlayerPrefs.GetInt("maxStaminaLevel", 0);
        maxStrengthLevel = PlayerPrefs.GetInt("maxStrengthLevel", 0);

        //
        PlayerHealth.Instance.maxHealth = health;
        PlayerHealth.Instance.UpdatePlayerHealthUI();
    }

    public void SavePlayerStatToPlayPref()
    {
        //Player's current stats
        PlayerPrefs.SetInt("playerHealth", health);
        PlayerPrefs.SetFloat("playerShield", shield);
        PlayerPrefs.SetFloat("playerStamina", stamina);
        PlayerPrefs.SetInt("playerStrength", strength);


        //Player current upgrade
        PlayerPrefs.SetInt("healthLevel", healthLevel);
        PlayerPrefs.SetInt("shieldLevel", shieldLevel);
        PlayerPrefs.SetInt("staminaLevel", staminaLevel);
        PlayerPrefs.SetInt("strengthLevel", strengthLevel);
        //Player current max upgrade
        PlayerPrefs.SetInt("maxHealthLevel", maxHealthLevel);
        PlayerPrefs.SetInt("maxShieldLevel", maxShieldLevel);
        PlayerPrefs.SetInt("maxStaminaLevel", maxStaminaLevel);
        PlayerPrefs.SetInt("maxStrengthLevel", maxStrengthLevel);

        //Player XP
        PlayerXPLevelUpManager.instance.SaveXP();


        PlayerPrefs.Save();

    }

    public void ResetPlayerStatInPlayPref()
    {
        //Player's current stats
        PlayerPrefs.SetInt("playerHealth", 3);
        PlayerPrefs.SetFloat("playerShield", 10);
        PlayerPrefs.SetFloat("playerStamina", 10);
        PlayerPrefs.SetInt("playerStrength", 1);


        //Player current upgrade
        PlayerPrefs.SetInt("healthLevel", 0);
        PlayerPrefs.SetInt("shieldLevel", 0);
        PlayerPrefs.SetInt("staminaLevel", 0);
        PlayerPrefs.SetInt("strengthLevel", 0);
        //Player current max upgrade
        PlayerPrefs.SetInt("maxHealthLevel", 0);
        PlayerPrefs.SetInt("maxShieldLevel", 0);
        PlayerPrefs.SetInt("maxStaminaLevel", 0);
        PlayerPrefs.SetInt("maxStrengthLevel", 0);

       


        PlayerPrefs.Save();

        LoadPlayerStatFromPlayPref();

        UpdateLevelCost();

        PlayerXPLevelUpManager.instance.ResetXP();

        EconomyManager.instance.AddCoins(9999);
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            UpdateLevelCost();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            SavePlayerStatToPlayPref();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ResetPlayerStatInPlayPref();
            PlayerXPLevelUpManager.instance.ResetXP();
        }
    }



    public void UpdateLevelCost()
    {
        healthCost = (healthLevel < healthUpgradeCost.Length) ? healthUpgradeCost[healthLevel] : 0;
        shieldCost = (shieldLevel < shieldUpgradeCost.Length) ? shieldUpgradeCost[shieldLevel] : 0;
        staminaCost = (staminaLevel < staminaUpgradeCost.Length) ? staminaUpgradeCost[staminaLevel] : 0;
        strengthCost = (strengthLevel < strengthUpgradeCost.Length) ? strengthUpgradeCost[strengthLevel] : 0;
    }
    public void UnlockStats()
    {
        if (maxHealthLevel < maxStatLevel) 
            maxHealthLevel++;
        if (maxShieldLevel < maxStatLevel) 
            maxShieldLevel++;
        if (maxStaminaLevel < maxStatLevel) 
            maxStaminaLevel++;
        if (maxStrengthLevel < maxStatLevel) 
            maxStrengthLevel++;

        SavePlayerStatToPlayPref();
    }

    public void UpgradeHealth()
    {
        if (healthLevel < maxHealthLevel && healthLevel < healthUpgradeCost.Length)
        {
            int cost = healthCost;
            if (EconomyManager.instance.CanAfford(cost))
            {
                EconomyManager.instance.SpendMoney(cost);
                healthLevel++;


                health += 1; // Increase health by 10 per level

                PlayerHealth.Instance.AddMaxHealth(1);


                AudioManager.instance.PlaySFX("healing", transform);
                PlayerHealth.Instance.currentHealth = health;
                SavePlayerStatToPlayPref();
                UpdateLevelCost();

                CharacterUpdateUI.instance.UpdatePlayerStatsUI();

                // return true;
            }
        }
        return;
        //return false;
    }

    public void UpgradeShield()
    {
        if (shieldLevel < maxShieldLevel && shieldLevel < shieldUpgradeCost.Length)
        {
            int cost = shieldCost;
            if (EconomyManager.instance.CanAfford(cost))
            {
                EconomyManager.instance.SpendMoney(cost);
                shieldLevel++;
                //shield += 2;
                PlayerShieldManager.instance.AddMaxShield(2);
                SavePlayerStatToPlayPref();
                UpdateLevelCost();
                CharacterUpdateUI.instance.UpdatePlayerStatsUI();
                


                // return true;
            }
        }
        //return false;
    }

    public void UpgradeStamina()
    {
        if (staminaLevel < maxStaminaLevel && staminaLevel < staminaUpgradeCost.Length)
        {
            int cost = staminaCost;
            if (EconomyManager.instance.CanAfford(cost))
            {
                EconomyManager.instance.SpendMoney(cost);
                staminaLevel++;
                //stamina += 2; 
                PlayerStaminaManager.instance.AddMaxStamina(2);
                SavePlayerStatToPlayPref();
                UpdateLevelCost();
                CharacterUpdateUI.instance.UpdatePlayerStatsUI();
               


                //  return true;
            }
        }
        //return false;
    }

    public void UpgradeStrength()
    {
        if (strengthLevel < maxStrengthLevel && strengthLevel < strengthUpgradeCost.Length)
        {
            int cost = strengthCost;
            if (EconomyManager.instance.CanAfford(cost))
            {
                EconomyManager.instance.SpendMoney(cost);
                strengthLevel++;
                strength += 1;



                PlayerPrefs.SetFloat("playerStrength", strength);
                //PlayerPrefs.Save();
               

                SavePlayerStatToPlayPref();



                UpdateLevelCost();

                CharacterUpdateUI.instance.UpdatePlayerStatsUI();


                //  return true;
            }
        }
      //  return false;
    }


}
