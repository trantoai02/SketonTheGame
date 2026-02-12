using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Vector2 targetPosition;
    private float speed = 6f;
    private float stopDistance = 0.1f;

    private Animator animator;

    private bool reachedTarget = false;
    private bool isExploding = false;

    private float explodeTimer = 0f;
    [SerializeField] private float explodeDelay = 2f; // phát nổ sau khi tới đích

    public void Init(Vector2 target, float projectileSpeed)
    {
        targetPosition = target;
        speed = projectileSpeed;
    }

    [SerializeField] private float dropDistance = 5f;
   
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isExploding) return;

        if (!reachedTarget)
        {

            // Hướng di chuyển
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            // Bay tới target
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);


            // Xoay theo hướng bay (trục X chỉa về hướng di chuyển)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Vector2.Distance(transform.position, targetPosition) <= stopDistance)
            {
                reachedTarget = true;
                speed = 0f;
                explodeTimer = 0f; // reset timer

                // Khi dừng lại, quay xuống trục -Y
                transform.rotation = Quaternion.AngleAxis(-90f, Vector3.forward);
            }
        }
        else
        {
            // Đã tới đích, đếm ngược để nổ
            explodeTimer += Time.deltaTime;
            if (explodeTimer >= explodeDelay)
            {
                TriggerExplosion();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploding) return;

        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(1, transform);
            }
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        if (isExploding) return;

        isExploding = true;
        speed = 0f;

      
            Destroy(gameObject);
        
    }

    // Gọi từ Animation Event cuối clip nổ
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
