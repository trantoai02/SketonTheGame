using UnityEngine;

public class NPCMoveToPoint : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform targetPoint;
    public Animator animator;

    private bool isMoving = false;

    public System.Action OnReachedTarget;

    void Update()
    {
        if (isMoving && targetPoint != null)
        {
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            // Bật animation chạy
            if (animator != null) animator.SetBool("isRun", true);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                isMoving = false;

                // Tắt animation chạy
                if (animator != null) animator.SetBool("isRun", false);

                OnReachedTarget?.Invoke(); // Gọi khi đến đích
            }
        }
    }

    public void MoveTo(Transform point)
    {
        targetPoint = point;
        isMoving = true;
    }
}
