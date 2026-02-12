using UnityEngine;
using Pathfinding;
using System.Collections;

public class Spider : Enemy
{
    [Header("Roaming")]
    [SerializeField] private Transform roamPoint;
    [SerializeField] private float roamRadius = 3f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Web Attack")]
    [SerializeField] private GameObject spiderWebPrefab;
    [SerializeField] private float postShootDelay = 2f;

    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private Animator animator;

    private float waitTimer = 0f;
    private Vector2 currentRoamTarget;
    private bool waiting = false;
    private bool isAttacking = false;
    private Vector2 lockedTargetPosition;

    protected override void Awake()
    {
        base.Awake();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateAnimationState();
    }

    protected override void MovementStateControl()
    {
        if (isAttacking) return;
        if (target == null) return;

        distanceToTarget = Vector2.Distance(transform.position, target.position);

        switch (state)
        {
            case State.Roaming:
                RoamingBehavior();
                if (distanceToTarget <= chaseRange)
                {
                    state = State.Chasing;
                }
                break;

            case State.Chasing:
                aiPath.canMove = true;
                destinationSetter.target = target;

                if (distanceToTarget <= attackRange)
                {
                    state = State.Attacking;
                   
                }
                else if (distanceToTarget > chaseRange + 2f)
                {
                    state = State.Roaming;
                    destinationSetter.target = null;
                }
                break;

            case State.Attacking:
                StartCoroutine(AttackRoutine());
                break;
        }
    }


    private void RoamingBehavior()
    {
        aiPath.canMove = true;
        destinationSetter.target = null;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                SetRandomRoamPosition();
            }
        }
        else if (Vector2.Distance(transform.position, currentRoamTarget) < 0.5f)
        {
            waitTimer = waitTimeAtPoint;
            waiting = true;
        }
        else
        {
            aiPath.destination = currentRoamTarget;
        }
    }

    private void SetRandomRoamPosition()
    {
        Vector2 offset = Random.insideUnitCircle * roamRadius;
        currentRoamTarget = (Vector2)roamPoint.position + offset;
        aiPath.destination = currentRoamTarget;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        state = State.Attacking;

        aiPath.canMove = false;
        animator.SetBool("isShoot", true);

        // Ghi nhớ vị trí của player lúc này
        lockedTargetPosition = target.position;

        // Bắn tơ
        ShootWeb(lockedTargetPosition);

        yield return new WaitForSeconds(postShootDelay);

        animator.SetBool("isShoot", false);

        // Bắt đầu chase đến vị trí đã nhớ
        destinationSetter.target = null; // hủy chase target để không override
        aiPath.canMove = true;
        aiPath.destination = lockedTargetPosition;

        // Chờ đến khi tới nơi
        yield return new WaitUntil(() => Vector2.Distance(transform.position, lockedTargetPosition) < 0.3f);

        // ✅ Dừng 3 giây trước khi trở về trạng thái chase
        aiPath.canMove = false;
        yield return new WaitForSeconds(3f);

        aiPath.canMove = true;

        // Sau đó trở về chế độ chase
        state = State.Chasing;
        isAttacking = false;
    }

    private void ShootWeb(Vector2 targetPosition)
    {
        if (spiderWebPrefab == null) return;

        GameObject web = Instantiate(spiderWebPrefab, transform.position, Quaternion.identity);
        SpiderWeb webScript = web.GetComponent<SpiderWeb>();

        if (webScript != null)
        {
            webScript.MoveTo(targetPosition, this); // truyền tham chiếu Spider hiện tại
        }
    }

    public void OnWebHitPlayer(Vector2 actualHitPosition)
    {
        lockedTargetPosition = actualHitPosition;
    }


    private void UpdateAnimationState()
    {
        if (animator == null) return;

        bool isMoving = !aiPath.isStopped && aiPath.velocity.magnitude > 0.1f;
        animator.SetBool("isMove", isMoving && !isAttacking);
    }

    public void Die()
    {
        animator.SetBool("isDie", true);
        aiPath.canMove = false;
        destinationSetter.enabled = false;
    }
}
