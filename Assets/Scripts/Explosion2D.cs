using UnityEngine;
using System.Collections;

public class Explosion2D : MonoBehaviour
{
    [Header("Thiết lập vụ nổ")]
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public LayerMask affectedLayers;

    private bool hasExploded = false;

    [SerializeField] GameObject breakApartRef;
    [SerializeField] GameObject explosionEffectPrefab;
    [SerializeField] int health = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.GetComponent<DamageSource>() || collision.gameObject.CompareTag("ElectricSource"))
        {
            TakeDamage();

            if (health <= 0)
            {
                StartCoroutine(DelayedExplode(0.5f)); // gọi coroutine nổ sau 0.5s
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BreakableWall" || collision.gameObject.tag == "EnemyBoss")
        {
            ExecuteExplosion();
        }
    }
    public void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            StartCoroutine(DelayedExplode(0.5f)); // gọi coroutine nổ sau 0.5s
        }
    }
    private IEnumerator DelayedExplode(float delay)
    {
        if (hasExploded) yield break;
        hasExploded = true;

        yield return new WaitForSeconds(delay);
        ExecuteExplosion(); // gọi hàm thực hiện nổ thật sự
    }

    private void ExecuteExplosion()
    {
        // Tìm tất cả đối tượng trong vùng nổ
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, affectedLayers);

        foreach (Collider2D collider in colliders)
        {
            // Thêm lực đẩy nếu có Rigidbody2D
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = rb.position - (Vector2)transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);
            }

            // Nếu có script Explosion2D và chưa nổ → nổ tiếp
            Explosion2D otherExplosion = collider.GetComponent<Explosion2D>();
            if (otherExplosion != null && otherExplosion != this)
            {
                otherExplosion.Explode();
            }
        }

        // Instantiate hiệu ứng và phá hủy
        if (breakApartRef != null)
        {
            GameObject destructable = Instantiate(breakApartRef);
            destructable.transform.position = transform.position;
        }

        if (explosionEffectPrefab != null)
        {
            GameObject explosionEff = Instantiate(explosionEffectPrefab);
            explosionEff.transform.position = transform.position;

        }

        Destroy(gameObject);
    }

    // Gọi từ ngoài (ví dụ TNT khác nổ) cũng sẽ bắt đầu delay
    public void Explode()
    {
        if (!hasExploded)
        {
            StartCoroutine(DelayedExplode(0.5f));
        }
    }

    // Hiển thị vùng nổ trong Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
