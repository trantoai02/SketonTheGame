using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageRedirector
{
    bool CanReceiveDamage();
    EnemyHealth GetEnemyHealth();
}

public class BossHandDamageRedirector : MonoBehaviour, IDamageRedirector
{
    [SerializeField] private EnemyHealth bossHealth;
    public bool canReceiveDamage = false;

    public bool CanReceiveDamage()
    {
        return canReceiveDamage;
    }

    public EnemyHealth GetEnemyHealth()
    {
        return bossHealth;
    }
}
