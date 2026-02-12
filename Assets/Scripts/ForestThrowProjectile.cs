using System.Collections;
using UnityEngine;

public class ForestThrowProjectile : MonoBehaviour
{




    private Vector2 targetPosition;
    private float speed;
    private float stopDistance = 0.1f;

    private bool isFlying = false;
    private bool reachedTarget = false;

    [SerializeField] private int damage = 1;

    private Animator animator;
    public SpriteRenderer spriteRenderer;

    private bool isHeld = true;

    private Collider2D collider;

    private void Awake()
    {

       

        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();

        animator.enabled = false;
    }

    // ===== GỌI TỪ ANIMATION EVENT (KHI THẢ VẬT THỂ) =====
    public void StartThrow(Vector2 target, float projectileSpeed)
    {
        targetPosition = target;
        speed = projectileSpeed;


        // 👉 Flip sprite dựa trên hướng target
        if (target.x > transform.position.x)
            //spriteRenderer.flipX = true;
            transform.localScale = new Vector3(-1, 1, 1);
        else
            //spriteRenderer.flipX = false;
            transform.localScale = new Vector3(1, 1, 1);


        isHeld = false;
        isFlying = true;
    }


    void Update()
    {
        if (isHeld) return;

        if (!isFlying || reachedTarget) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Chỉ khóa -180 SAU KHI ĐÃ THẢ
        transform.rotation = Quaternion.Euler(0f, 0f, -180f);

        if (Vector2.Distance(transform.position, targetPosition) <= stopDistance)
        {
            reachedTarget = true;
            isFlying = false;
            speed = 0f;

            OnReachedTarget();
        }
    }


    void OnReachedTarget()
    {
       
        // 👉 Play animation chạm đất
        if (animator != null && reachedTarget)
        {
            animator.enabled = true;
        }
       

        // Destroy sau khi animation xong (có thể chỉnh theo clip length)
        Destroy(gameObject, 3.5f);
    }

    public void EnableHitBox()
    {
        if (collider != null)
        {
            collider.enabled = true;
        }
    }   
    public void DisableHitBox()
    {
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (reachedTarget) return;

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponentInParent<Player>();
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, transform);
            }

            if (player != null)
            {
                player.EnterFlatState();
                player.StartCoroutine(ExitFlatAfterDelay(player, 5f));
            }

        }
    }
    IEnumerator ExitFlatAfterDelay(Player player, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player != null)
            player.ExitFlatState();
    }

}
