using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MoveEnemy();
        UpdateFacingDirection();
    }

    void MoveEnemy()
    {
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.deltaTime));
    }

    void UpdateFacingDirection()
    {
        // Nếu enemy đang di chuyển về phía bên phải, quay sang phải
        if (moveDir.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        // Ngược lại, nếu enemy đang di chuyển về phía bên trái, quay sang trái
        else if (moveDir.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        // Nếu enemy đang đứng yên trên trục x, giữ nguyên hướng nhìn
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDir = direction.normalized;
    }
}

