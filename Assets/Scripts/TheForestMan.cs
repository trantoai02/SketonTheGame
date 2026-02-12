using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheForestMan : Enemy
{

    public static TheForestMan Instance;

    [Header("Boss Stats")]
    private EnemyHealth health;
    private EnemyStats stats;


    private float maxPhaseHP;
    public float maxHP = 3000;
    private float phaseHP;
    private float nextStunHP;


    public float stunDuration = 3f;
    public float restTime = 2f;

    private float nextStunThreshold;
    private bool isTransforming = false;

    private Phase currentPhase;
    private BossState bossState;

    private bool isCastingSkill;

    private Animator animator;

    [Header("Spike Ritual")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private Transform[] spikeSpawnPoints;
    [SerializeField] private float spikeSpawnDelay = 0.1f;
    public int spikesPerSpawn;

    [Header("Smash Wave")]
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private Transform leftHandPoint;
    [SerializeField] private Transform rightHandPoint;
    [SerializeField] private Transform twoHandsPoint;

    [Header("Sweep Hand Hitbox")]
    [SerializeField] private Collider2D leftSweepHitbox;
    [SerializeField] private Collider2D rightSweepHitbox;

    [Header("Throw Object")]
    [SerializeField] private GameObject[] throwPrefabs;

    [SerializeField] private Transform throwHandBone;
    [SerializeField] private float throwForce = 12f;

  


    private GameObject currentThrownObject;
    private Vector2 lockedTargetPos;

    private bool isDead;
 
    private bool isStunned;
    private bool hasStunnedThisPhase = false;

   

    #region ENUM
    private enum Phase
    {
        Phase1,
        Phase2
    }

    private enum BossState
    {
        Idle,
        Attacking,
        Stunned,
        Dead
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }

        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        stats = GetComponent<EnemyStats>();

        health.controlledByBoss = true;

        health.currentHealth = stats.health;
        maxHP = stats.health;

        maxPhaseHP = maxHP;
        phaseHP = maxPhaseHP;
        nextStunThreshold = maxPhaseHP * 2f / 3f;

        currentPhase = Phase.Phase1;
        bossState = BossState.Idle;

        health.EnemyInjured.AddListener(OnDamaged);
    }

    protected override void Update()
    {
        if (isDead || isTransforming || isStunned)
            return;

        if (!isCastingSkill && phaseRoutine == null)
            phaseRoutine = StartCoroutine(PhaseBehaviour());
    }

    public bool canHandHurt;
    public bool IsHandCanHurt()
    {
        return canHandHurt;
    }

    public void EnableHandHurt()
    {
        canHandHurt = true;
    }

    public void DisableHandHurt()
    {
        canHandHurt = false;
    }

    #region PHASE LOGIC
    IEnumerator PhaseBehaviour()
    {
        isCastingSkill = true;
        if(isDead || isTransforming || isStunned) yield break;

        if (currentPhase == Phase.Phase1)
        {
             yield return StartCoroutine(SpikeRitual());

            yield return Rest();

            yield return StartCoroutine(SmashHand());
        }
        else if (currentPhase == Phase.Phase2)
        {
           
            yield return StartCoroutine(SweepAttack());
            yield return Rest();

            yield return StartCoroutine(SpikeRitual());
            yield return Rest();

            yield return StartCoroutine(ThrowObject());
        }

        yield return Rest();
        isCastingSkill = false;
        phaseRoutine = null;
    }
    #endregion

    #region SKILLS

    void PlayAnim(string anim)
    {
      

        animator.ResetTrigger("Spike");
        animator.ResetTrigger("Wave");
        animator.ResetTrigger("Sweep");
        animator.ResetTrigger("Throw");
        animator.ResetTrigger("Shield");


        animator.SetTrigger(anim);
    }


    // Phase 1 & 2
    IEnumerator SpikeRitual()
    {
        bossState = BossState.Attacking;

        PlayAnim("Spike");

        yield return new WaitForSeconds(1f);

        Debug.Log("Forest Man: Spike Up!");

        // ===== SHUFFLE DANH SÁCH SPAWN POINT =====
        List<Transform> shuffledPoints = new List<Transform>(spikeSpawnPoints);

        for (int i = 0; i < shuffledPoints.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledPoints.Count);
            (shuffledPoints[i], shuffledPoints[randomIndex]) =
                (shuffledPoints[randomIndex], shuffledPoints[i]);
        }

        // ===== SPAWN 6 SPIKE ĐẦU TIÊN =====
        int spawnCount = Mathf.Min(spikesPerSpawn, shuffledPoints.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(
                spikePrefab,
                shuffledPoints[i].position,
                Quaternion.identity
            );

            yield return new WaitForSeconds(spikeSpawnDelay);
        }

        yield return new WaitForSeconds(3.2f);
    }



    // Phase 1


    IEnumerator SmashHand()
    {
        bossState = BossState.Attacking;
        PlayAnim("Wave");

        Debug.Log("Forest Man: Smash");
        yield return new WaitForSeconds(5.2f); // length anim
    }
    public void OnLeftHandImpact()
    {
       
        SpawnShockwave(leftHandPoint);
    }

    public void OnRightHandImpact()
    {
       
        SpawnShockwave(rightHandPoint);
    }

 
    void SpawnShockwave(Transform point)
    {
        Instantiate(shockwavePrefab, point.position, Quaternion.identity);
    }


    // Phase 2
    IEnumerator SweepAttack()
    {
        bossState = BossState.Attacking;

        PlayAnim("Sweep");
        Debug.Log("Forest Man: Sweep");
        yield return new WaitForSeconds(5f); // length anim Sweep
    }

    public void EnableLeftSweep()
    {
        if (bossState != BossState.Attacking) return;
        leftSweepHitbox.isTrigger = true;
    }

    public void DisableLeftSweep()
    {
        leftSweepHitbox.isTrigger = false;
    }

    public void EnableRightSweep()
    {
        if (bossState != BossState.Attacking) return;
        rightSweepHitbox.isTrigger = true;
    }

    public void DisableRightSweep()
    {
        rightSweepHitbox.isTrigger = false;
    }



    // Phase 2
    IEnumerator ThrowObject()
    {
        bossState = BossState.Attacking;

        PlayAnim("Throw");

        // thời gian = length animation
        yield return new WaitForSeconds(6f);
    }

    public void AE_GrabThrowObject()
    {
        if (throwPrefabs == null || throwPrefabs.Length == 0)
        {
            Debug.LogWarning("ForestMan: throwPrefabs rỗng!");
            return;
        }

        GameObject randomPrefab =
            throwPrefabs[Random.Range(0, throwPrefabs.Length)];

        currentThrownObject = Instantiate(
            randomPrefab,
            throwHandBone.position,
            throwHandBone.rotation
        );

        // Gắn vào tay
        currentThrownObject.transform.SetParent(throwHandBone);
    }


    public void AE_ReleaseThrowObject()
    {
        if (currentThrownObject == null) return;

        // Tháo khỏi tay
        currentThrownObject.transform.SetParent(null);

        // Khóa target tại THỜI ĐIỂM THẢ
        Vector2 lockedTargetPos = target.position;

        ForestThrowProjectile projectile =
            currentThrownObject.GetComponent<ForestThrowProjectile>();

        projectile.StartThrow(lockedTargetPos, throwForce);
    }





    #endregion

    #region DAMAGE & PHASE CHANGE

    public void OnDamaged()
    {
        if (isDead || isTransforming)
            return;

        // ===== DIE / TRANSFORM ƯU TIÊN TUYỆT ĐỐI =====
        if (health.currentHealth <= 0)
        {
            StopAllCoroutines();
            isCastingSkill = false;

            if (currentPhase == Phase.Phase1)
            {
                StartCoroutine(Phase1_DeathToTransform());
            }
            else
            {
                StartCoroutine(Die());
            }
            return;
        }

        // ===== STUN (MỖI PHASE 1 LẦN DUY NHẤT) =====
        if (!isStunned
            && !hasStunnedThisPhase
            && health.currentHealth <= nextStunThreshold)
        {
            hasStunnedThisPhase = true;
            StartCoroutine(StunRoutine());
        }

    }
    private Coroutine phaseRoutine;

    IEnumerator Rest()
    {
        animator.SetTrigger("Shield");
        yield return new WaitForSeconds(restTime);
 
    }
    IEnumerator StunRoutine()
    {

        // DỪNG PHASE NGAY KHI STUN
        if (phaseRoutine != null)
        {
            StopCoroutine(phaseRoutine);
            phaseRoutine = null;
            isCastingSkill = false;
        }

        isStunned = true;
        EnableHandHurt();
        bossState = BossState.Stunned;

        PlayAnim("Stun");
       

        float timer = 0f;

        while (timer < stunDuration)
        {
            // 💀 DIE LUÔN TRONG STUN
            if (health.currentHealth <= 0)
            {
                isStunned = false;
                StartCoroutine(Die());
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (isDead || isTransforming)
            yield break;

        isStunned = false;
        DisableHandHurt() ;
        bossState = BossState.Idle;
       
    }




    IEnumerator Phase1_DeathToTransform()
    {
        isTransforming = true;
        isStunned = false;

        PlayAnim("Transform");
        yield return new WaitForSeconds(6.2f);

        // ===== CHUYỂN PHASE =====
        currentPhase = Phase.Phase2;

        maxPhaseHP = maxHP * 1.6f;
        stats.health = Mathf.RoundToInt(maxPhaseHP);
        health.currentHealth = stats.health;
        health.RefreshHealthBar(stats.health);

        nextStunThreshold = maxPhaseHP * 2f / 3f;
        hasStunnedThisPhase = false;

        bossState = BossState.Idle;
        isTransforming = false;


        isCastingSkill = false;

        if (phaseRoutine != null)
        {
            StopCoroutine(phaseRoutine);
            phaseRoutine = null;
        }
        Debug.Log("Forest Man bước vào Phase 2!");
    }


    IEnumerator Die()
    {
        if (isDead) yield break;

        isDead = true;

        //StopAllCoroutines();
        animator.Play("Die", 0, 0f); // FORCE PLAY
        yield return new WaitForSeconds(5f);
        health.controlledByBoss = false;
        health.DetectDeath();
    }



    #endregion
}
