
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerXPLevelUpManager : MonoBehaviour
{
    public static PlayerXPLevelUpManager instance;

    public int[] levelToUnlockStats = { 1,2,3,4,5,6,7 };

    public int level;
    public float currentXp;
    public float requiredXp;

    float lerpTime;

    [Header("UI")]
    public Image frontXP;
    public TMP_Text currentLevelText;

    //for pc
    public Image frontXP_PC;
    public TMP_Text currentLevelText_PC;
    public GameObject xpContainerForPcCanvas;
    public GameObject MobileCanvas;
    public bool isPC;

    [Range(0f, 300f)]
    public float additionMultiplieer = 300;

    [Range(2f, 4f)]
    public float powerMultiplier = 2;

    [Range(7f, 14f)]
    public float divisionMultiplier = 7;


    private void Awake()
    {
        instance = this;

        if (MobileCanvas.gameObject.activeSelf)
        {
            xpContainerForPcCanvas.SetActive(false);
            isPC = false;
        }
        else
        {
            xpContainerForPcCanvas.SetActive(true);
            isPC = true;

        }

        LoadXP();
    }
    void Start()
    {
        frontXP.fillAmount = currentXp / requiredXp;
        currentLevelText.text = level.ToString();

        frontXP_PC.fillAmount = currentXp / requiredXp;
        currentLevelText_PC.text = level.ToString();

    }


    // Update is called once per frame
    void Update()
    {
       

        UpdateXPUI();
        if (Input.GetKeyDown(KeyCode.T))
        {
            GainXPRate(500);

        }
        if (currentXp >= requiredXp)
        {
            LevelUp();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SaveXP();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadXP();
        }
    }

    public void UpdateXPUI()
    {
        
        if (!isPC)
        {
            float xpFraction = currentXp / requiredXp;
            float fXP = frontXP.fillAmount;
            frontXP.fillAmount = xpFraction;

            currentLevelText.text = level.ToString();

            if (fXP < xpFraction)
            {
                lerpTime += Time.deltaTime;
                float percentComplete = lerpTime;
                frontXP.fillAmount = Mathf.Lerp(fXP, xpFraction, percentComplete);

            }
        }
        else
        {
            float xpFraction = currentXp / requiredXp;
            float fXP = frontXP_PC.fillAmount;
            frontXP_PC.fillAmount = xpFraction;
            currentLevelText_PC.text = level.ToString();

            if (fXP < xpFraction)
            {
                lerpTime += Time.deltaTime;
                float percentComplete = lerpTime;
                frontXP_PC.fillAmount = Mathf.Lerp(fXP, xpFraction, percentComplete);

            }
        }
    }

    public void GainXPRate(float xpGain)
    {
        currentXp += xpGain;
        lerpTime = 0;

    }

    public void LevelUp()
    {
      
        level++;
       // Player.instance.ChangePlayerColor();
        currentLevelText.text = level.ToString();
        frontXP.fillAmount = 0;
        frontXP_PC.fillAmount = 0;

        currentXp = Mathf.RoundToInt(currentXp - requiredXp);
       
        requiredXp = CalculateRequiredXP();

        //nếu level hiện tại của người chơi lớn hơn bằng chỉ số unlock yêu cầu thì mới mở slot upgrade

        if (IsUnlockLevel(level) && !ReachedMaxStatLevel())
        {
            PlayerStats.Instance.UnlockStats();
        }

        PlayerStats.Instance.SavePlayerStatToPlayPref();
       
    }

    private bool IsUnlockLevel(int currentLevel)
    {
        foreach (int unlockLevel in levelToUnlockStats)
        {
            if (currentLevel == unlockLevel)
                return true;
        }
        return false;
    }
    private bool ReachedMaxStatLevel()
    {
        return PlayerStats.Instance.maxHealthLevel >= PlayerStats.maxStatLevel &&
               PlayerStats.Instance.maxShieldLevel >= PlayerStats.maxStatLevel &&
               PlayerStats.Instance.maxStaminaLevel >= PlayerStats.maxStatLevel &&
               PlayerStats.Instance.maxStrengthLevel >= PlayerStats.maxStatLevel;
    }

    int CalculateRequiredXP()
    {
        int solveForeRequiredXp = 0;
        for (int levelCycle = 1; levelCycle <= level; levelCycle++)
        {
            solveForeRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplieer
                * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }
        return solveForeRequiredXp / 4;
    }

    public void SaveXP()
    {
        PlayerPrefs.SetFloat("XP", currentXp);
        PlayerPrefs.SetFloat("requiredXP", requiredXp);
        PlayerPrefs.SetInt("level", level);

        PlayerPrefs.Save();
    }

    public void ResetXP()
    {
        PlayerPrefs.SetFloat("XP", 0);
        PlayerPrefs.SetFloat("requiredXP", 120);
        PlayerPrefs.SetInt("level", 1);


        PlayerPrefs.Save();

        LoadXP();

    }
    public void LoadXP()
    {
        currentXp = PlayerPrefs.GetFloat("XP", 0);
        requiredXp = PlayerPrefs.GetFloat("requiredXP", CalculateRequiredXP());
        level = PlayerPrefs.GetInt("level", 1);

        UpdateXPUI();
    }
}
