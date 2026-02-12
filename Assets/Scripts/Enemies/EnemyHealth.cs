using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
  //  [SerializeField]
   // int startHealth = 3;

    [SerializeField] GameObject deathEffectPrefab;

    public int currentHealth;
 

    Knockback knockBack;

    Flash flash;

    EnemyStats stats;

    public float ExpCount;

    public UnityEvent EnemyInjured;

    [SerializeField] HealthBar healthBar;

    public int oldHealth;

    public bool controlledByBoss = false;

    [Header("Hit Effect")]
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] float hitEffectOffset = 0.1f;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockBack = GetComponent<Knockback>();

        stats = GetComponent<EnemyStats>();
        if(stats == null)
        {
            stats = GetComponentInChildren<EnemyStats>();
        }
    }
    void SpawnHitEffect()
    {
        if (hitEffectPrefab == null) return;
        if (Player.instance == null) return;

        // Hướng lực giống knockback
        Vector2 attackDir = (transform.position - Player.instance.transform.position).normalized;

        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        // Particle bắn theo Y+, nên trừ 90°
        Quaternion rot = Quaternion.Euler(0, 0, angle - 90f);

        Vector2 spawnPos = (Vector2)transform.position + attackDir * hitEffectOffset;

        Instantiate(hitEffectPrefab, spawnPos, rot);
    }



    public void RefreshHealthBar(int maxHealth)
    {
        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }
    private void Start()
    {
        if (stats != null)
            currentHealth = stats.health;
        else
        {
            oldHealth = currentHealth;
        }
        if(healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, stats.health);

        }
        ExpCount = ((int)Mathf.Floor(EnemyStats.Instance.baseXPQuantity + (stats.level - 1) * 1.25f));
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        SpawnHitEffect();
        if (healthBar != null)
        {
            if (stats != null)
            {
                healthBar.UpdateHealthBar(currentHealth, stats.health);

            }
            else
            {
                healthBar.UpdateHealthBar(currentHealth, oldHealth);

            }

        }
        if (knockBack != null)
        {
            knockBack.GetKnockedBack(Player.instance.transform, 15f);

        }

        if (flash != null)
        {
            StartCoroutine(flash.FlashRoutine());
            StartCoroutine(CheckDetectDeathRoutine());

        }
        else
        {
            DetectDeath(); // 🔥 QUAN TRỌNG
        }
        EnemyInjured?.Invoke();

    }

    public void TakeDamageWithoutKnockedBack(int damage)
    {
        currentHealth -= damage;
        SpawnHitEffect();
        if (healthBar != null)
        {
            if (stats != null)
            {
                healthBar.UpdateHealthBar(currentHealth, stats.health);

            }
            else
            {
                healthBar.UpdateHealthBar(currentHealth, oldHealth);

            }

        }
        if (flash != null)
        {
            StartCoroutine(flash.FlashRoutine());
            StartCoroutine(CheckDetectDeathRoutine());
        }
        else
        {
            DetectDeath(); // 🔥 QUAN TRỌNG
        }
        EnemyInjured?.Invoke();
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        if(flash !=null)
        {
            yield return new WaitForSeconds(flash.GetRestoreMatTime());
        }
        DetectDeath();
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            if (controlledByBoss)
            {
                // Boss tự xử lý chết
                return;
            }
            if(stats !=null)
            {

                var spawner = GetComponent<PickupSpawner>();
                if (spawner != null)
                {
                    spawner.DropHeartOrCoin();
                    spawner.DropExpPoints(
                        Mathf.FloorToInt(
                            EnemyStats.Instance.baseXPQuantity *
                            (1 + ((float)stats.level / 10))
                        )
                    );
                    spawner.DropCustomItem();
                }

            }
            // Enemy thường
            AudioManager.instance.PlaySFX("enemy_die", transform);
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);

            if (QuestManager.Instance != null) { if (CompareTag("Enemy")) { QuestManager.Instance.OnEnemyKilled(gameObject); } }

            Destroy(gameObject);
        }
    }



}
