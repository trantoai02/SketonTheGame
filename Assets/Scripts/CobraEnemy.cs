
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CobraEnemy : Enemy
{
    NavMeshAgent agent;

    public float roamRadius = 5f;

    Animator animator;
    private Coroutine attackCoroutine;

    EnemyHealth enemyHealth;
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

        enemyHealth = GetComponent<EnemyHealth>();

        if(enemyHealth != null )
        {
            enemyHealth.EnemyInjured.AddListener(DoInjuredAnimation);

        }
    }

    protected override void MovementStateControl()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);

        if (chaseType == ChaseType.Free)
        {
            if (PlayerHealth.Instance.isDead)
            {
                state = State.Roaming;
            }
            else
            {
                switch (state)
                {
                    case State.Roaming:
                        animator.SetBool("isMove", false);
                        StopAttack();
                        if (distanceToPlayer <= chaseRange)
                        {
                            state = State.Chasing;
                        }
                        break;

                    case State.Chasing:
                        animator.SetBool("isMove", true);
                        animator.SetBool("isAttack", false);
                        StopAttack();
                        if (distanceToPlayer <= attackRange)
                        {
                            state = State.Attacking;
                        }
                        else if (distanceToPlayer > chaseRange)
                        {
                            state = State.Roaming;
                        }
                        else
                        {
                            ChasePlayer();
                        }
                        break;

                    case State.Attacking:
                        if (distanceToPlayer > attackRange)
                        {
                            state = State.Chasing;
                        }
                        else
                        {
                            StartAttack();
                        }
                        break;
                }


                if (state == State.Chasing || state == State.Attacking)
                {
                    FacePlayer();
                }
                //currentPos = transform.position;
                //Vector2 moveDir = (currentPos - oldPos).normalized;
                //oldPos = currentPos;
                //UpdateSpriteDirection(moveDir);
            }
        }
        else if (chaseType == ChaseType.Aim)
        {
             distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);

            // Không được Roam, luôn đuổi và tấn công
            if (PlayerHealth.Instance.isDead)
            {
                state = State.Roaming;
                StopAttack();
                animator.SetBool("isMove", false);
                return;
            }

            if (distanceToPlayer <= attackRange)
            {
                state = State.Attacking;
            }
            else
            {
                state = State.Chasing;
            }

            if (state == State.Chasing)
            {
                animator.SetBool("isMove", true);
                animator.SetBool("isAttack", false);
                StopAttack();
                ChasePlayer();
            }
            else if (state == State.Attacking)
            {
                StartAttack();
            }

            //currentPos = transform.position;
            //oldPos = currentPos;
            
        }

        // Đảm bảo sprite luôn flip đúng nếu không dùng UpdateSpriteDirection
        //UpdateSpriteDirection(moveDir);
    }

    void FacePlayer()
    {
        if (Player.instance == null) return;

        float dir = Player.instance.transform.position.x - transform.position.x;

        if (dir > 0)
            transform.localScale = new Vector3(1, 1, 1); // player bên phải
        else if (dir < 0)
            transform.localScale = new Vector3(-1, 1, 1);  // player bên trái
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
            // Lấy vị trí hiện tại của player và enemy
            Vector3 playerPosition = Player.instance.transform.position;
            Vector3 currentPosition = transform.position;

            // Chỉ thay đổi vị trí theo trục y, giữ nguyên x và z
            Vector3 targetPosition = new Vector3(currentPosition.x, playerPosition.y, currentPosition.z);

            // Đặt điểm đến cho agent để di chuyển đến ngang trục y của player
            agent.SetDestination(targetPosition);
            agent.isStopped = false;

            // Chờ cho đến khi agent di chuyển gần đến vị trí mục tiêu trên trục y
            while (Mathf.Abs(transform.position.y - playerPosition.y) > 0.2f) // Điều kiện chênh lệch nhỏ để dừng
            {
                yield return null; // Đợi mỗi frame cho đến khi đạt mục tiêu
            }

            // Dừng agent khi đã đến vị trí ngang trục y
            agent.isStopped = true;
        }
    }
    void FlipTowardsPlayer()
    {
        Vector3 playerPosition = Player.instance.transform.position;
        Vector3 currentPosition = transform.position;

        // Kiểm tra vị trí của player so với enemy để xác định hướng lật
        if (playerPosition.x < currentPosition.x)
        {
            // Nếu player ở bên trái, lật sprite qua trái
            transform.localScale = new Vector3(1, 1, 1); // Lật trên trục x
        }
        else
        {
            // Nếu player ở bên phải, giữ nguyên hướng
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    IEnumerator AttackRepeatedly()
    {
        while (state == State.Attacking)
        {
            yield return MoveToPlayerYAxis();

            // Lật hướng sprite để đối mặt với player
            //FlipTowardsPlayer();

            agent.isStopped = true;
            animator.SetBool("isAttack", true);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length); // Delay between attacks
            animator.SetBool("isAttack", false);
            
            animator.SetBool("isMove", true);

            yield return new WaitForSeconds(1); // Delay between attacks
            
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
            // lấy vị trí ngẫu nhiên trong bán kính roamRadius từ vị trí hiện tại của enemy
            Vector2 randomPosition = GetRandomPosition(transform.position, roamRadius);

            // Kiểm tra xem có tường hoặc vật cản ở vị trí ngẫu nhiên không
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                // 
                if (transform.gameObject.activeSelf && agent != null && agent.isOnNavMesh)
                {
                    agent.SetDestination(hit.position);

                }
            }
            else
            {
                Roam();
            }
        }
    }
  

    Vector2 GetRandomPosition(Vector2 center, float radius)
    {
        // Tạo một vị trí ngẫu nhiên trong bán kính radius từ center
        Vector2 randomDirection = Random.insideUnitCircle * radius;
        return center + randomDirection;
    }

    public void DoInjuredAnimation()
    {
        StartCoroutine(EnemyInjuredAnimationCoroutine());
    }

    IEnumerator EnemyInjuredAnimationCoroutine()
    {
        animator.SetBool("isInjured", true);
        agent.isStopped = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        animator.SetBool("isInjured", false);
        agent.isStopped = false;


    }

    public GameObject cobraCollider;

    public void CobraColliderOn()
    {
        cobraCollider.SetActive(true);
    }
    public void CobraColliderOff()
    {
        cobraCollider.SetActive(false);

    }
}
