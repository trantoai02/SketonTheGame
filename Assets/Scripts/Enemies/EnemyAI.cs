using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : Enemy
{
    NavMeshAgent agent;

    public float roamRadius = 5f;

   
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        
    }

    private void Start()
    {
        // Nếu chưa có target, gán Player
        if (target == null && Player.instance != null)
        {
            target = Player.instance.transform;
        }

        InvokeRepeating("Roam", 0, 3);
        agent.speed = moveSpeed;
    }


    protected override void MovementStateControl()
    {
        if (chaseType == ChaseType.Free)
        {
            if (PlayerHealth.Instance.isDead)
            {
                state = State.Roaming;
            }
            else
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);

                switch (state)
                {
                    case State.Roaming:
                        if (distanceToTarget <= chaseRange)
                        {
                            state = State.Chasing;
                        }
                        break;

                    case State.Chasing:
                        if (distanceToTarget <= attackRange)
                        {
                            state = State.Attacking;
                        }
                        else if (distanceToTarget > chaseRange)
                        {
                            state = State.Roaming;
                        }
                        else
                        {
                            ChasePlayer();
                        }
                        break;

                    case State.Attacking:
                        if (distanceToTarget > attackRange)
                        {
                            state = State.Chasing;
                        }
                        break;
                }
            }
        }
        else if (chaseType == ChaseType.Aim)
        {
            // Gán target nếu chưa có
            if (target == null && Player.instance != null)
            {
                target = Player.instance.transform;
            }

            if (target != null)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (distanceToTarget <= attackRange)
                {
                    state = State.Attacking;
                }
                else
                {
                    state = State.Chasing;
                    ChasePlayer();
                }
            }
        }

        currentPos = transform.position;
        Vector2 moveDir = (currentPos - oldPos).normalized;
        oldPos = currentPos;
        UpdateSpriteDirection(moveDir);
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
    void ChasePlayer()
    {
        if (agent.isOnNavMesh)
        {
            if(target != null)
            {
                agent.SetDestination(target.position);

            }
        }
    }
  
 
    Vector2 GetRandomPosition(Vector2 center, float radius)
    {
        // Tạo một vị trí ngẫu nhiên trong bán kính radius từ center
        Vector2 randomDirection = Random.insideUnitCircle * radius;
        return center + randomDirection;
    }

}
