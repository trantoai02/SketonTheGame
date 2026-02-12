
using UnityEngine;

public class BreakApartObject : MonoBehaviour
{
    [SerializeField] GameObject breakApartRef;


    [SerializeField] int health = 3;

    bool isShaking = false;
    [SerializeField] float shakeAmount = 0.1f;

    Vector2 startPos;

    [Header("Loot Drop (optional)")]
    [SerializeField] GameObject[] spawnPrefabs;

    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        if (isShaking)
        {
            transform.position = startPos + UnityEngine.Random.insideUnitCircle * shakeAmount;
        }
    }

    public void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            ExplodeThisGameObject();
        }
        else
        {
            isShaking = true;
            Invoke("ResetShake", .2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<DamageSource>() != null)
        {

            TakeDamage();
           

            //if(health <= 0)
            //{
            //    ExplodeThisGameObject();
            //}
            //else
            //{
            //    isShaking = true;
            //    Invoke("ResetShake", .2f);
            //}
        }
    }

    void ResetShake()
    {
        isShaking = false;
    }

    private void ExplodeThisGameObject()
    {
        // 💥 hiệu ứng vỡ
        GameObject destructable = Instantiate(breakApartRef);
        destructable.transform.position = transform.position;
        destructable.transform.localScale = transform.localScale;

        // 🎁 Spawn loot nếu có
        if (spawnPrefabs != null && spawnPrefabs.Length > 0)
        {
            foreach (var prefab in spawnPrefabs)
            {
                if (prefab == null) continue;

                Vector2 offset = Random.insideUnitCircle * 0.3f; // rơi tản nhẹ
                Instantiate(prefab, (Vector2)transform.position + offset, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }


}
