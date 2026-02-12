using UnityEngine;
using System;

public class EnemyForm : MonoBehaviour
{
    public int maxHealth = 3;
    [SerializeField]
    private int currentHealth;

    public Action onDeath;

    bool isShaking = false;
    [SerializeField] float shakeAmount = 0.1f;

    Vector2 startPos;

    void Start()
    {
        currentHealth = maxHealth;
        startPos = transform.position;

    }

    private void Update()
    {
        if (isShaking)
        {
            transform.position = startPos + UnityEngine.Random.insideUnitCircle * shakeAmount;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            onDeath?.Invoke();
        }
        else
        {
            isShaking = true;
            Invoke("ResetShake", .1f);
        }
    }

    void ResetShake()
    {
        isShaking = false;
    }
    // Debug cho kiểm thử
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<DamageSource>() != null || collision.gameObject.tag == "ElectricSource")
        {
            TakeDamage(1);
        }

      
      
    }

 
}
