using UnityEngine;
using Pathfinding;
using System.Collections;

public class HellFlying : Enemy
{
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;

    [Header("Attack Settings")]
    [SerializeField] private float attackDelay = 2f;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 6f;

    [Header("Roam Settings")]
    [SerializeField] private Transform roamCenter;
    [SerializeField] private float roamRadius = 3f;

    [Header("Charge Attack")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 1f;

    private Vector2 roamPosition;
    private Vector2 lockedTargetPosition;
    private bool isAttacking = false;

    private int attackCount = 0; // đếm số lần attack type 1

    Animator animator;

    protected override void Awake()
    {
        base.Awake();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        aiPath.maxSpeed = moveSpeed;

        animator = GetComponent<Animator>();
    }

    protected override void MovementStateControl()
    {
        if (isAttacking) return;

        distanceToTarget = Vector2.Distance(transform.position, target.position);

        switch (state)
        {
            case State.Roaming:
                destinationSetter.target = null;

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

        aiPath.canMove = false;

        // nhớ vị trí player tại thời điểm tấn công
        lockedTargetPosition = target.position;

        yield return new WaitForSeconds(attackDelay);

        if (attackCount < 3)
        {
            // Attack Type 1: bắn fireball
            ShootFireball(lockedTargetPosition);
            attackCount++;
        }
        else
        {
            // Attack Type 2: lao thẳng
            yield return StartCoroutine(ChargeAttack(lockedTargetPosition));
            attackCount = 0; // reset chu kỳ
        }

        yield return new WaitForSeconds(0.5f);

        aiPath.canMove = true;
        isAttacking = false;

        // chuyển state
        state = Vector2.Distance(transform.position, target.position) < chaseRange ? State.Chasing : State.Roaming;
    }

    private void ShootFireball(Vector2 targetPos)
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("fireballPrefab chưa được gán trong Inspector!");
            return;
        }


        //animation
        StartCoroutine( PlayShootAnimation());

        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        FireballProjectile fireballScript = fireball.GetComponent<FireballProjectile>();
        fireballScript.Init(targetPos, fireballSpeed);

    }

    private IEnumerator PlayShootAnimation()
    {
        animator.SetBool("isShoot", true);
        yield return new WaitForSeconds(0.625f); // bằng thời gian animation Shoot
        animator.SetBool("isShoot", false);
    }


    private IEnumerator ChargeAttack(Vector2 targetPos)
    {
        Vector2 startPos = transform.position;
        float elapsed = 0f;

         UpdateSpriteDirection(targetPos - (Vector2) transform.position);
        while (elapsed < chargeDuration)
        {
            elapsed += Time.deltaTime;

            //animation
            animator.SetBool("isCharge", true);

            transform.position = Vector2.MoveTowards(transform.position, targetPos, chargeSpeed * Time.deltaTime);
           
            yield return null;
        }

        animator.SetBool("isCharge", false);

    }

    private Vector2 GetRandomRoamPosition()
    {
        if (roamCenter == null) roamCenter = this.transform;
        Vector2 randomOffset = Random.insideUnitCircle * roamRadius;
        return (Vector2)roamCenter.position + randomOffset;
    }
}
