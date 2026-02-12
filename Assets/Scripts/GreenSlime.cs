using System.Collections;
using UnityEngine;

public class GreenSlime : Enemy
{
    Animator animator;
    [SerializeField] private float roamChangeDirFloat = 2f;

    bool isCharging = false;
    public float dashAmount = 5;
    public float chargingTime = 1f;
    private Vector3 posOld;
    [SerializeField] private float attackCooldown = 2f;


    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

    }

    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }


    protected override void MovementStateControl()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);

        if (chaseType == ChaseType.Free)
        {
            if (isCharging)
            {
                state = State.Attacking;
            }

            switch (state)
            {
                default:
                case State.Roaming:
                    Roaming();
                    if (distanceToPlayer <= chaseRange)
                    {
                        state = State.Chasing;
                    }
                    break;

                case State.Chasing:
                    target = Player.instance.transform;

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
                    if (!isCharging)
                    {
                        posOld = Player.instance.transform.position;
                        isCharging = true;
                        animator.SetBool("isCharging", true);
                    }

                    if (chargingTime > 0)
                    {
                        chargingTime -= Time.deltaTime;
                        if (chargingTime <= 0)
                        {
                            StartCoroutine(Charging());
                        }
                    }

                    if (distanceToPlayer > attackRange && !isCharging)
                    {
                        state = State.Chasing;
                    }

                    break;
            }
        }
        else if (chaseType == ChaseType.Aim)
        {
            // Aim mode: luôn chasing hoặc attacking
            float attackDistance = Vector3.Distance(transform.position, Player.instance.transform.position);

            if (state != State.Attacking && attackDistance <= attackRange)
            {
                state = State.Attacking;
            }
            else if (attackDistance > attackRange && !isCharging)
            {
                state = State.Chasing;
            }

            switch (state)
            {
                case State.Chasing:
                    target = Player.instance.transform;
                    ChasePlayer();
                    break;

                case State.Attacking:
                    if (!isCharging)
                    {
                        posOld = Player.instance.transform.position;
                        isCharging = true;
                        animator.SetBool("isCharging", true);
                    }

                    if (chargingTime > 0)
                    {
                        chargingTime -= Time.deltaTime;
                        if (chargingTime <= 0)
                        {
                            StartCoroutine(Charging());
                        }
                    }

                    if (attackDistance > attackRange && !isCharging)
                    {
                        state = State.Chasing;
                    }

                    break;
            }
        }

        // Cập nhật hướng nhìn sprite
        UpdateSpriteDirection(moveDir);
    }


    Vector3 targetPosition;
    private IEnumerator Charging()
    {
        // xác định vị trí player
        targetPosition = posOld;

        rb.velocity = Vector2.zero;
        //chargingTime đang bị trừ dần về 0, đảm bảo enemy đang đứng yên
        yield return new WaitForSeconds(chargingTime);

        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 finalPosition = transform.position + direction * 3;

        while (transform.position != finalPosition)
        {
            Vector3 posToMove = Vector3.MoveTowards(transform.position, finalPosition, moveSpeed * dashAmount * Time.deltaTime);
            animator.SetBool("isAttack", true);
            animator.SetBool("isCharging", false);
            rb.MovePosition(posToMove);
            yield return null;
        }

        chargingTime = 2f;
        isCharging = false;

        animator.SetBool("isAttack", false);
        animator.SetBool("isIdle", true);
    }
    private void ChasePlayer()
    {
      
        moveDir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
    }


    private void Roaming()
    {

        timeRoaming += Time.deltaTime;
        
        moveDir = (roamPosition - (Vector2)transform.position);


        
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.deltaTime));

        if (Vector2.Distance(transform.position, Player.instance.transform.position) < attackRange)
        {
            state = State.Attacking;
        }

        if (timeRoaming >= roamChangeDirFloat)
        {
            roamPosition = GetRoamingPosition();
        }
    }

    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;
        Vector2 currentPos = transform.position;
        return currentPos + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

}
