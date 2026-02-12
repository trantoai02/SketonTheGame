using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarSprite;

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBarSprite.fillAmount = (float)currentHealth/maxHealth;
    }
}
