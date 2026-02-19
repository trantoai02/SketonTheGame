using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShieldManager : MonoBehaviour
{

    public static PlayerShieldManager instance;

  

    public float maxShield = 10;
    public float currentShield;
    public float shieldRegenRate = 0.5f;
    public float shieldAmount;

    public bool isRegenerating = true;

    [Header("UI")]
    public Image frontShield;
    public Image frameShield;

    private void Awake()
    {
        instance = this;
        maxShield = PlayerPrefs.GetFloat("playerShield", 10);
    }

    private void Start()
    {
        currentShield = maxShield;
    }

    void Update()
    {
        UpdateShieldUI();
        if (isRegenerating)
        {
            RegenerateShield();
        }
    }


    public void AddMaxShield(float amount)
    {
        maxShield += amount;

        PlayerPrefs.SetFloat("playerShield", maxShield);
        PlayerPrefs.Save();
    }


    public void UpdateShieldUI()
    {
        frontShield.fillAmount = currentShield / maxShield;
    }

    private IEnumerator StartRegeneration()
    {
        yield return new WaitForSeconds(1f);
        isRegenerating = true;
    }

    private void RegenerateShield()
    {
        if (currentShield < maxShield)
        {
            currentShield += shieldRegenRate * Time.deltaTime;
            if (currentShield > maxShield)
            {
                currentShield = maxShield;
            }
        }
    }

    public void ShieldConsume(float amount)
    {
        if (currentShield >= amount)
        {
            currentShield -= amount;
            if (currentShield < 0)
            {
                currentShield = 0;
            }
            isRegenerating = false;
            StartCoroutine(StartRegeneration());
        }
    }
}
