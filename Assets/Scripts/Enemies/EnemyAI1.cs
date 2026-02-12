using System.Collections;
using UnityEngine;

public class EnemyAI1 : Enemy
{
    //vị trí của player
    public Vector3 posOld;

    [SerializeField] private float roamChangeDirFloat = 2f;
    private bool isCharging = false;
    public float dashAmount = 5;
    public float chargingTime = 1f;


    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }

    protected override void MovementStateControl()
    {
        if (chaseType == ChaseType.Free)
        {
            if (isCharging)
            {
                state = State.Attacking;
            }
            float distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);
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
                    }

                    if (chargingTime > 0)
                    {
                        chargingTime -= Time.deltaTime;
                        if (chargingTime <= 0)
                        {
                            StartCoroutine(Charging());
                        }
                    }

                    if (distanceToPlayer > attackRange)
                    {
                        state = State.Chasing;
                    }

                    break;
            }

            currentPos = transform.position;
            Vector2 moveDir = (currentPos - oldPos).normalized;
            oldPos = currentPos;
        }
        else if (chaseType == ChaseType.Aim)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);

            if (state != State.Attacking)
            {
                isCharging = false; // Reset khi không đang tấn công
                chargingTime = 2f;  // Reset lại thời gian charge (hoặc giá trị bạn muốn)
            }

            if (distanceToPlayer <= attackRange)
            {
                state = State.Attacking;
            }
            else
            {
                state = State.Chasing;
            }

            switch (state)
            {
                case State.Chasing:
                    ChasePlayer();
                    break;

                case State.Attacking:
                    if (!isCharging)
                    {
                        posOld = Player.instance.transform.position;
                        isCharging = true;
                        chargingTime = 2f;  // Đảm bảo reset chargingTime đúng lúc bắt đầu charge
                    }

                    if (chargingTime > 0)
                    {
                        chargingTime -= Time.deltaTime;
                        if (chargingTime <= 0)
                        {
                            StartCoroutine(Charging());
                        }
                    }

                    if (distanceToPlayer > attackRange)
                    {
                        state = State.Chasing;
                        isCharging = false; // thoát trạng thái charge khi ra khỏi tầm
                    }
                    break;
            }

            currentPos = transform.position;
            Vector2 moveDir = (currentPos - oldPos).normalized;
            oldPos = currentPos;
        }
            UpdateSpriteDirection(moveDir);
        }



    Vector3 targetPosition;
    private IEnumerator Charging()
    {
        targetPosition = posOld;

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(chargingTime);

        Vector3 direction = (targetPosition - transform.position).normalized;

        // Cập nhật hướng di chuyển chính xác ngay trước khi di chuyển
        UpdateSpriteDirection(direction);

        Vector3 finalPosition = transform.position + direction * 2;

        while (transform.position != finalPosition)
        {
            Vector3 posToMove = Vector3.MoveTowards(transform.position, finalPosition, moveSpeed * dashAmount * Time.deltaTime);
            rb.MovePosition(posToMove);
            yield return null;
        }

        chargingTime = 2f;
        isCharging = false;
    }


    private void ChasePlayer()
    {
        target = Player.instance.transform;
        moveDir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.deltaTime));
    }

    private void Roaming()
    {
        
        timeRoaming += Time.deltaTime;

        moveDir = (roamPosition - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        if (Vector2.Distance(transform.position, Player.instance.transform.position) < attackRange)
        {
            state = State.Attacking;
        }

        if (timeRoaming > roamChangeDirFloat)
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
