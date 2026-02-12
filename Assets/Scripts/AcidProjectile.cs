using UnityEngine;

public class AcidProjectile : MonoBehaviour
{
    private Vector2 targetPosition;
    private float speed = 5f;
    private float stopDistance = 0.1f;

    private Animator animator;

    private bool reachedTarget = false;
    private float explodeTimer = 0f;
    private float explodeDelay = 2f; // 2 giây đếm ngược từ lúc tới đích

    public void Init(Vector2 target, float projectileSpeed)
    {
        targetPosition = target;
        speed = projectileSpeed;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!reachedTarget)
        {
            // Bay tới đích
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) <= stopDistance)
            {
                reachedTarget = true;
                speed = 0f; // dừng lại
                explodeTimer = 0f; // bắt đầu đếm ngược
            }
        }
        else
        {
            // Đã đến đích, đếm 2 giây rồi nổ
            explodeTimer += Time.deltaTime;
            if (explodeTimer >= explodeDelay)
            {
                TriggerExplosion();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(1, transform);
            }
            // Không gọi TriggerExplosion ở đây nữa,
            // vì nổ sẽ theo timer 2 giây từ lúc đến đích
        }
    }

    private bool isExploding = false;

    private void TriggerExplosion()
    {
        if (isExploding) return;

        isExploding = true;

        if (animator != null)
        {
            animator.SetBool("isPlat", true);
        }

        speed = 0f; // đảm bảo dừng hẳn
    }

    // Gọi từ Animation Event cuối clip nổ
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
