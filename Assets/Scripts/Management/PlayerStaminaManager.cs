using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaManager : MonoBehaviour
{
 public static PlayerStaminaManager instance;


    public float maxStamina = 10;
    public float currentStamina;
    public float staminaRegenRate = 0.5f;
    public float staminaAmount;

    public bool isRegenerating = true;
    [Header("UI")]
    public Image frontStamina;
    public Image frameStamina;

    private void Awake()
    {
        instance = this;
        maxStamina = PlayerPrefs.GetFloat("playerStamina", 10);
    }

    private void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        UpdateStaminaUI();
        if (isRegenerating)
        {
            RegenerateStamina();
        }
    }

    public void UpdateStaminaUI()
    {
        frontStamina.fillAmount = currentStamina / maxStamina;
    }

    private IEnumerator StartRegeneration()
    {
        yield return new WaitForSeconds(1f);
        isRegenerating = true;
    }

    private void RegenerateStamina()
    {

        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }
    }

    public void StaminaConsume(float amount)
    {
        Debug.Log("stamina consume");
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            if (currentStamina < 0)
            {
                currentStamina = 0;
            }
            isRegenerating = false;
            StartCoroutine(StartRegeneration());
        }
    }
}
