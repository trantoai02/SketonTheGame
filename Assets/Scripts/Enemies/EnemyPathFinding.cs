
using UnityEngine;

public class EnemyPathFinding : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;
    private SpriteRenderer spriteRenderer;


    Knockback knockBack;

    Rigidbody2D rb;
    Vector2 moveDir;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockBack = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (knockBack.gettingKnockback) { return; }

        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        if (moveDir.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    public void MoveTo(Vector2 targetPos)
    {
        moveDir = (targetPos - (Vector2)transform.position).normalized;


    }

    public void StopMoving()
    {
        moveDir = Vector3.zero;
    }
}
