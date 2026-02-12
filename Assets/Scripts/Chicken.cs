using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Chicken : MonoBehaviour
{
    [Header("Movement Settings")]
    public bool canMove = true;          // ✅ Bật/tắt di chuyển tự do
    public float roamRadius = 4f;        // Bán kính di chuyển ngẫu nhiên
    public float roamInterval = 3f;      // Thời gian đổi hướng
    public float moveSpeed = 3f;         // Tốc độ đi bộ

    private NavMeshAgent agent;
    private SpriteRenderer sr;
    private Animator animator;

    private bool isCaught = false;
    private Transform handleTarget;
    private float followSpeed = 7f;

    private bool movingToCoop = false;
    private Vector3 coopTarget;
    private Action onReachCoop;

    private Coroutine roamRoutine;

    private Vector3 lastHandlePos; // dùng để phát hiện hướng tay cầm di chuyển

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // NavMeshAgent cho 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;

        // Chỉ bắt đầu roam nếu được phép di chuyển
        if (canMove)
            roamRoutine = StartCoroutine(RoamRoutine());
    }

    private void Update()
    {
        // Nếu không cho di chuyển thì dừng agent
        if (!canMove && agent.enabled && !isCaught && !movingToCoop)
        {
            agent.ResetPath();
            return;
        }

        // Khi bị bắt → gà dính chặt vào tay cầm
        if (isCaught && handleTarget != null)
        {
            if (agent.enabled)
                agent.enabled = false;

            // ✅ Gà luôn dính vào tay cầm
            transform.position = handleTarget.position;

            // ✅ Flip hướng theo hướng tay cầm di chuyển
            float moveX = handleTarget.position.x - lastHandlePos.x;
            if (Mathf.Abs(moveX) > 0.01f)
                sr.flipX = moveX > 0; // Nếu tay cầm đi qua phải → gà nhìn phải

            lastHandlePos = handleTarget.position;
            animator.SetBool("isMove", Mathf.Abs(moveX) > 0.01f);
            return;
        }

        // Khi di chuyển vào chuồng
        if (movingToCoop)
        {
            transform.position = Vector3.MoveTowards(transform.position, coopTarget, Time.deltaTime * followSpeed);
            if (Vector3.Distance(transform.position, coopTarget) < 0.05f)
            {
                movingToCoop = false;
                onReachCoop?.Invoke();

                // ✅ Khi vào chuồng, bật lại di chuyển tự do (kể cả gà bị disable trước đó)
                canMove = true;

                if (roamRoutine == null)
                    roamRoutine = StartCoroutine(RoamRoutine());

                // Bật lại agent nếu bị tắt
                if (!agent.enabled)
                    agent.enabled = true;
            }

        }

        // Flip sprite theo hướng di chuyển (khi tự do)
        if (agent.enabled)
        {
            if (agent.velocity.x > 0.01f)
                sr.flipX = true;
            else if (agent.velocity.x < -0.01f)
                sr.flipX = false;
        }

        // Cập nhật animation (khi tự do hoặc đi vào chuồng)
        bool isMoving = (agent.enabled && agent.velocity.magnitude > 0.1f) || movingToCoop;
        animator.SetBool("isMove", isMoving);
    }

    IEnumerator RoamRoutine()
    {
        while (true)
        {
            if (canMove && !isCaught && !movingToCoop)
            {
                Vector2 randomDir = UnityEngine.Random.insideUnitCircle * roamRadius;
                Vector3 dest = transform.position + new Vector3(randomDir.x, randomDir.y, 0);

                if (NavMesh.SamplePosition(dest, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }

            yield return new WaitForSeconds(roamInterval);
        }
    }

    public void MoveToHandle(Transform handle)
    {
        isCaught = true;
        handleTarget = handle;
        lastHandlePos = handle.position; // khởi tạo hướng ban đầu

        canMove = false; // không roam khi bị bắt

        if (agent.enabled)
            agent.ResetPath();
    }

    public void MoveToCoop(Vector3 coopPos, Action onReach)
    {
        isCaught = false;
        movingToCoop = true;
        coopTarget = coopPos;
        onReachCoop = onReach;

        canMove = false; // tạm thời không roam khi đang đi vào chuồng
        if (agent.enabled)
            agent.ResetPath();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ChickenCatchQuest quest = FindObjectOfType<ChickenCatchQuest>();
            if (quest != null)
                quest.TryCatchChicken(this);
        }
    }
}
