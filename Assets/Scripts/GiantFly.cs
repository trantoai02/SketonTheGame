using UnityEngine;
using Pathfinding;
using System.Collections;

public class GiantFly : Enemy
{
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;

    [SerializeField] private float attackDelay = 2f;
    [SerializeField] private GameObject acidProjectilePrefab;
    [SerializeField] private float acidSpeed = 5f;
    [SerializeField] private Transform roamCenter;
    [SerializeField] private float roamRadius = 3f;

    private Vector2 lockedTargetPosition;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
      
        aiPath.maxSpeed = moveSpeed;
    }

    protected override void MovementStateControl()
    {
        if (isAttacking) return;

        distanceToTarget = Vector2.Distance(transform.position, target.position);

        switch (state)
        {
            case State.Roaming:
                destinationSetter.target = null;

                // Nếu chưa có đích hoặc đã tới gần đích, chọn vị trí mới
                if (Vector2.Distance(transform.position, roamPosition) < 0.5f || roamPosition == Vector2.zero)
                {
                    roamPosition = GetRandomRoamPosition();
                }

                aiPath.destination = roamPosition;

                if (distanceToTarget < chaseRange)
                    state = State.Chasing;
                break;

            case State.Chasing:
                destinationSetter.target = target;

                if (distanceToTarget < attackRange)
                    state = State.Attacking;
                else if (distanceToTarget > chaseRange + 2f)
                    state = State.Roaming;
                break;

            case State.Attacking:
                StartCoroutine(AttackRoutine());
                break;
        }

        UpdateSpriteDirection(aiPath.desiredVelocity);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // Dừng lại
        aiPath.canMove = false;

        // Ghi nhớ vị trí player
        lockedTargetPosition = target.position;

        yield return new WaitForSeconds(attackDelay);

        // Bắn đạn axit
        ShootAcid(lockedTargetPosition);

        yield return new WaitForSeconds(0.5f); // chờ nhẹ để tránh spam

        aiPath.canMove = true;
        isAttacking = false;

        // Sau khi tấn công, chuyển về chasing nếu vẫn gần
        state = Vector2.Distance(transform.position, target.position) < chaseRange ? State.Chasing : State.Roaming;
    }

    private void ShootAcid(Vector2 targetPos)
    {
        if (acidProjectilePrefab == null)
        {
            Debug.LogWarning("acidProjectilePrefab chưa được gán trong Inspector!");
            return;
        }

        GameObject bullet = Instantiate(acidProjectilePrefab, transform.position, Quaternion.identity);
        AcidProjectile acidScript = bullet.GetComponent<AcidProjectile>();
        acidScript.Init(targetPos, acidSpeed);
    }

    private Vector2 GetRandomRoamPosition()
    {
        if (roamCenter == null) roamCenter = this.transform;

        Vector2 randomOffset = Random.insideUnitCircle * roamRadius;
        return (Vector2)roamCenter.position + randomOffset;
    }
}
