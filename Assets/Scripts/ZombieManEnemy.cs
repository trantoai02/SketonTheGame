using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieManEnemy : Enemy
{
    NavMeshAgent agent;
    public float roamRadius = 5f;

    Animator animator;
    private Coroutine attackCoroutine;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        InvokeRepeating("Roam", 0, 3);
        agent.speed = moveSpeed;
        animator = GetComponent<Animator>();
    }

    protected override void MovementStateControl()
    {
        if (PlayerHealth.Instance.isDead)
        {
            state = State.Roaming;
            animator.SetBool("isRun", false);
            StopAttack();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);

        if (chaseType == ChaseType.Free)
        {
            switch (state)
            {
                case State.Roaming:
                    animator.SetBool("isRun", agent.velocity.magnitude > 1);
                    StopAttack();

                    if (distanceToPlayer <= chaseRange)
                        state = State.Chasing;
                    break;

                case State.Chasing:
                    animator.SetBool("isRun", true);
                    animator.SetBool("isAttack", false);
                    StopAttack();

                    if (distanceToPlayer <= attackRange)
                        state = State.Attacking;
                    else if (distanceToPlayer > chaseRange)
                        state = State.Roaming;
                    else
                        ChasePlayer();
                    break;

                case State.Attacking:
                    if (distanceToPlayer > attackRange)
                        state = State.Chasing;
                    else
                        StartAttack();
                    break;
            }
        }
        else if (chaseType == ChaseType.Aim)
        {
            // Luôn xác định vị trí player và chase + attack (không roam)
            if (distanceToPlayer <= attackRange)
            {
                state = State.Attacking;
                StartAttack();
            }
            else
            {
                state = State.Chasing;
                animator.SetBool("isRun", true);
                animator.SetBool("isAttack", false);
                StopAttack();
                ChasePlayer();
            }
        }

        // Luôn cập nhật hướng sprite
        if (state != State.Attacking)
        {
            currentPos = transform.position;
            Vector2 moveDir = (currentPos - oldPos).normalized;
            oldPos = currentPos;
            UpdateSpriteDirection(moveDir);
        }
    }

    void StartAttack()
    {
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRepeatedly());
        }
    }

    void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        animator.SetBool("isAttack", false);
        agent.isStopped = false;
    }

    IEnumerator MoveToPlayerYAxis()
    {
        if (agent.isOnNavMesh)
        {
            Vector3 playerPosition = Player.instance.transform.position;
            Vector3 currentPosition = transform.position;

            Vector3 targetPosition = new Vector3(currentPosition.x, playerPosition.y, currentPosition.z);

            agent.SetDestination(targetPosition);
            agent.isStopped = false;

            while (Mathf.Abs(transform.position.y - playerPosition.y) > 0.2f)
            {
                yield return null;
            }

            agent.isStopped = true;
        }
    }

    void FlipTowardsPlayer()
    {
        Vector3 playerPosition = Player.instance.transform.position;
        Vector3 currentPosition = transform.position;

        if (playerPosition.x - currentPosition.x < 0.2)
            transform.localScale = new Vector3(-1, 1, 1); // trái
        else
            transform.localScale = new Vector3(1, 1, 1); // phải
    }

    IEnumerator AttackRepeatedly()
    {
        while (state == State.Attacking)
        {
            yield return MoveToPlayerYAxis();

            FlipTowardsPlayer(); // Lật sprite trước khi attack

            agent.isStopped = true;
            animator.SetBool("isAttack", true);

            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);

            animator.SetBool("isAttack", false);
            animator.SetBool("isRun", true);

            yield return new WaitForSeconds(1f); // Delay giữa các đòn tấn công
        }

        StopAttack();
    }

    void ChasePlayer()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(Player.instance.transform.position);
        }
    }

    void Roam()
    {
        if (state == State.Roaming)
        {
            Vector2 randomPosition = GetRandomPosition(transform.position, roamRadius);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                if (gameObject.activeSelf && agent != null && agent.isOnNavMesh)
                {
                    agent.SetDestination(hit.position);
                }
            }
            else
            {
                Roam(); // Thử lại nếu vị trí roam không hợp lệ
            }
        }
    }

    Vector2 GetRandomPosition(Vector2 center, float radius)
    {
        Vector2 randomDirection = Random.insideUnitCircle * radius;
        return center + randomDirection;
    }

    public GameObject zombieManCollider;

    public void ZombieManColliderOn()
    {
        zombieManCollider.SetActive(true);
    }

    public void ZombieManColliderOff()
    {
        zombieManCollider.SetActive(false);
    }
}
