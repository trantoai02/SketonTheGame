
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject particleOnHitEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        Indestructible indestructible = collision.gameObject.GetComponent<Indestructible>();
      
        if (enemyHealth || indestructible || collision.gameObject.CompareTag("DestructableObject"))
        {
            Instantiate(particleOnHitEffect, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
