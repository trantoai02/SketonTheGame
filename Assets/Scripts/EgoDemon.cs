using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyHealth))]
public class EgoDemon : Enemy
{
    public float delayTimeBeforeStart;
    [Header("Ego Demon Settings")]
    private float maxHealthPhase1;
    private float maxHealthPhase2;

    private EnemyHealth health;
    private NavMeshAgent agent;
    private Animator animator;
    //private Rigidbody2D rb;
    private EnemyStats stats;

    private bool isFlying = true;
    private bool isGroundedPhase = false;
    private bool isDead = false;
    private bool isRecovering = false;

    private Coroutine currentRoutine;

    [Header("Prefabs & Points")]
    public GameObject fireballPrefab;
    public List<Transform> fireballSpawnPoints = new List<Transform>();


    [Header("Minion Spawn Settings")]
    public GameObject minionPrefab;
    public List<Transform> minionSpawnPoints = new List<Transform>();

    public Transform fireballSpawnPos;

    public Transform rockSpawnPos;
    public Transform crushDownPos;

    public UnityEvent onEgoDeath;

    public GameObject demonEgoCollider;
    Collider2D attackCollider;


    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float stopDistance = 0.1f;

    private Vector2 lockedTarget;   // mục tiêu được khóa một lần
    private bool hasLockedTarget = false;
    private bool isRunning = false;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        stats = GetComponent<EnemyStats>();
        attackCollider = demonEgoCollider.GetComponent<Collider2D>();
        animator.SetBool("isFlying", isFlying);

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.enabled = false;

        health.controlledByBoss = true;


        // Khi Enemy bị thương → gọi sự kiện này
        health.EnemyInjured.AddListener(OnInjured);
    }

    private void Start()
    {
        // Lấy giá trị máu dựa trên EnemyStats
        if (stats != null)
        {
            maxHealthPhase1 = stats.health;
            maxHealthPhase2 = Mathf.RoundToInt(stats.health * 1.5f); // phase 2 mạnh hơn 1.5 lần, bạn có thể chỉnh
        }

        health.currentHealth = Mathf.RoundToInt(maxHealthPhase1);
        health.oldHealth = Mathf.RoundToInt(maxHealthPhase1);

      

    }



    protected override void MovementStateControl()
    {
        if (isDead) return;

        if (!isGroundedPhase)
        {
            if (currentRoutine == null)
                currentRoutine = StartCoroutine(FlyBehaviour());
        }
        else
        {
            if (currentRoutine == null)
                currentRoutine = StartCoroutine(GroundBehaviour());
        }
    }


    private bool brokeAtTwoThird = false;
    private bool brokeAtOneThird = false;
    private void OnInjured()
    {
        if (isDead) return;

        float currentHP = health.currentHealth;

       

        // Bay phase
        if (!isGroundedPhase)
        {
            // Nếu chết thì chuyển sang phase 2 (rơi xuống đất)
            if (currentHP <= 0)
            {
                StopAllCoroutines();
                //goi complete quest 2
                QuestManager.Instance?.CompleteQuest("lv12_quest2");
                animator.SetTrigger("isTransform");
                StartCoroutine(TransformToGroundPhase());
                return;
            }

            float oneThird = maxHealthPhase1 / 3f;

            // Gục khi còn <= 2/3 máu (lần đầu)
            if (currentHP <= oneThird * 2 && !brokeAtTwoThird)
            {
                brokeAtTwoThird = true;
                StartCoroutine(DamagedBreak());
            }
            // Gục khi còn <= 1/3 máu (lần hai)
            else if (currentHP <= oneThird && !brokeAtOneThird)
            {
                brokeAtOneThird = true;
                StartCoroutine(DamagedBreak());
            }
        }
        else
        {
            // Ground phase: chết hẳn
            if (currentHP <= 0 && !isDead)
            {
                StopAllCoroutines();
                StartCoroutine(DeathSequence());
            }
        }
    }

    private Coroutine breakCoroutine;
    IEnumerator DamagedBreak()
    {
        if (breakCoroutine != null) yield break;

        breakCoroutine = StartCoroutine(BreakRoutine());
    }

    IEnumerator BreakRoutine()
    {
        isRecovering = true;

        if (agent != null && agent.enabled)
            agent.isStopped = true;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        animator.SetBool("isDamagedBreak", true);

        yield return new WaitForSeconds(5f);

        animator.SetBool("isDamagedBreak", false);
        yield return new WaitForSeconds(.75f);

        if (rb != null)
            rb.isKinematic = false;

        if (agent != null && agent.enabled)
            agent.isStopped = false;

        isRecovering = false;
        breakCoroutine = null;

    }

    IEnumerator DeathSequence()
    {
        isDead = true;
        QuestManager.Instance?.CompleteQuest("lv12_quest3");
        agent.isStopped = true;
        animator.SetBool("isDie", true);
        yield return new WaitForSeconds(5f);


        onEgoDeath?.Invoke();

        health.controlledByBoss = false;

        health.DetectDeath();
        //Destroy(gameObject);
    }

    // ==========================================
    // 🕊️ PHASE 1 - BAY
    // ==========================================
    IEnumerator FlyBehaviour()
    {
        yield return new WaitForSeconds(delayTimeBeforeStart);

        CameraSequenceController.Instance.FollowPlayer();
        while (!isGroundedPhase && !isDead)
        {
            if (isRecovering)
            {
                yield return null; // Đợi đến khi hết break
                continue;
            }

            yield return MoveToPlayer();
            yield return new WaitForSeconds(1f);

            yield return FlyAttack();
            yield return new WaitForSeconds(2f);

            yield return MoveToPlayer();
            yield return new WaitForSeconds(1f);

            yield return CrushDown();
            yield return new WaitForSeconds(2f);

            SpawnMinions();
        }

        currentRoutine = null;
    }

    IEnumerator MoveToPlayer()
    {
        if (Player.instance == null) yield break;
        if (isRecovering) yield break;

        Vector3 targetPos = Player.instance.transform.position;
        float flySpeed = 4f;

        while (Vector3.Distance(transform.position, targetPos) > 1f && !isGroundedPhase)
        {
            if (isRecovering) yield break;

            Vector3 dir = (targetPos - transform.position).normalized;
            rb.MovePosition(rb.position + (Vector2)dir * flySpeed * Time.deltaTime);
            UpdateSpriteDirection(dir);
            yield return null;
        }
    }

    IEnumerator FlyAttack()
    {
        Vector2 lockedTarget = Player.instance.transform.position;

       // Vector3 targetPos = Player.instance.transform.position;
        if (isRecovering) yield break;
        Vector2 dir = (lockedTarget - (Vector2)transform.position).normalized;
        
        UpdateSpriteDirection(dir);
        animator.SetTrigger("isFlyAttack");
        yield return new WaitForSeconds(1.1f);
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPos.position, Quaternion.identity);

        if (isRecovering || Player.instance == null) yield break;

        FireballProjectile fb = fireball.GetComponent<FireballProjectile>();
        if (fb != null)
            fb.Init(lockedTarget, 6f);
    }

    public GameObject shockwavePrefab;
    IEnumerator CrushDown()
    {
        animator.SetTrigger("isCrushing");
        yield return new WaitForSeconds(0.85f);

       
    }

    public void SpawnShockWave()
    {
        if (shockwavePrefab != null && rockSpawnPos != null)
        {
            Instantiate(shockwavePrefab, crushDownPos.position, Quaternion.identity);
        }
    }

    // ==========================================
    // 🐾 PHASE 2 - ĐI BỘ
    // ==========================================
    IEnumerator GroundBehaviour()
    {
        agent.enabled = true;

        while (isGroundedPhase && !isDead)
        {
            yield return RunToLockedTarget(Player.instance.transform);
            //StartCoroutine(RunToPlayer());
            //yield return new WaitForEndOfFrame();


            yield return AttackCombo();
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(2f);


            SpawnMinions();
            yield return new WaitForSeconds(2f);

            yield return RunToLockedTarget(Player.instance.transform);
            //StartCoroutine(RunToPlayer());
            //yield return new WaitForEndOfFrame();


            yield return AttackCombo();
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(2f);
            //// 🔹 Chạy FireballFall song song, không chờ nó xong
            yield return FireballFall();

            // 🔹 Tiếp tục hành động khác ngay lập tức

            // (tuỳ bạn muốn bao lâu trước khi làm hành động tiếp theo)
        }

        currentRoutine = null;
    }

  
    public IEnumerator RunToLockedTarget(Transform player)
    {
        if (isRunning) yield break; // tránh gọi chồng chéo

        isRunning = true;

        // ✅ Khóa tọa độ mục tiêu 1 lần duy nhất
        if (!hasLockedTarget)
        {
            lockedTarget = player.position;
            hasLockedTarget = true;
        }

        animator.SetBool("isRun", true);

        while (Vector2.Distance(transform.position, lockedTarget) > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, lockedTarget, runSpeed * Time.deltaTime);

            // Hướng nhìn về hướng di chuyển
            Vector2 dir = lockedTarget - (Vector2)transform.position;
            UpdateSpriteDirection(dir);


            yield return null;
        }

        // ✅ Khi đã tới đích
        animator.SetBool("isRun", false);
        isRunning = false;
        hasLockedTarget = false;

        // (Tuỳ bạn) gọi tiếp hành vi khác sau khi tới
        // ví dụ: StartCoroutine(FireballFall());
    }


IEnumerator RunToPlayer()
    {
        if (Player.instance == null || agent == null || !agent.enabled || !agent.isOnNavMesh)
            yield break;

        // 🔹 Khóa vị trí player hiện tại — không cập nhật trong lúc chạy
        lockedTarget = Player.instance.transform.position;

        agent.isStopped = false;
        animator.SetBool("isRun", true);
        agent.SetDestination(lockedTarget);
       

        Vector3 dir = ((Vector3)lockedTarget - transform.position).normalized;
        UpdateSpriteDirection(dir);

         //🔹 Chạy đến vị trí đã khóa
        while (agent.enabled && agent.isOnNavMesh && agent.remainingDistance > 1.5f)
        {
            if (isRecovering || isDead) yield break;
            yield return null;
        }

 
        // 🔹 Khi tới gần, dừng lại
        agent.isStopped = true;
        animator.SetBool("isRun", false);
    }



    IEnumerator AttackCombo()
    {
        lockedTarget = Player.instance.transform.position;
        Vector2 dir = lockedTarget - (Vector2)transform.position;
        UpdateSpriteDirection(dir);

        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(.5f);

        animator.SetBool("isAttack", false);
    }

    IEnumerator DashBack()
    {
        //animator.SetTrigger("isDashBack");
        Vector3 backDir = -transform.right;
        transform.position += backDir * 2f;
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunToRandomPoint()
    {
        animator.SetBool("isRun", true);
        Vector3 rand = transform.position + new Vector3(Random.Range(-3, 3), 0, 0);
        if (agent.isOnNavMesh)
            agent.SetDestination(rand);

        yield return new WaitForSeconds(1f);
        animator.SetBool("isRun", false);
    }

    [SerializeField] private GameObject fireballWarningPrefab; // prefab cảnh báo đỏ
    [SerializeField] private float fireballFallHeight = 10f;   // cao hơn spawnpoint bao nhiêu
    [SerializeField] private float fireballFallSpeed = 10f;    // tốc độ rơi
    [SerializeField] private float warningDuration = 1.2f;     // thời gian hiện cảnh báo trước khi rơi

    IEnumerator FireballFall()
    {
        if (fireballSpawnPoints.Count == 0 || fireballPrefab == null || fireballWarningPrefab == null)
            yield break;

        // 🔹 Chọn ngẫu nhiên 4–6 điểm spawn (nếu ít hơn thì lấy toàn bộ)
        int spawnCount = Mathf.Clamp(Random.Range(4, 7), 1, fireballSpawnPoints.Count);

        // Tạo danh sách random vị trí
        List<Transform> selectedPoints = new List<Transform>(fireballSpawnPoints);
        for (int i = selectedPoints.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (selectedPoints[i], selectedPoints[randomIndex]) = (selectedPoints[randomIndex], selectedPoints[i]);
        }

        selectedPoints = selectedPoints.GetRange(0, spawnCount);

        // 🔥 Hiện cảnh báo đỏ tại vị trí spawn
        List<GameObject> warnings = new List<GameObject>();
        foreach (var point in selectedPoints)
        {
            GameObject warning = Instantiate(fireballWarningPrefab, point.position, Quaternion.identity);
            warnings.Add(warning);
        }

        // ⏳ Chờ 1 khoảng trước khi fireball rơi
        yield return new WaitForSeconds(warningDuration);

        // 🔥 Rơi từng viên fireball
        foreach (var point in selectedPoints)
        {
            StartCoroutine(SpawnAndDropFireball(point));
        }

        // 🔥 Xoá cảnh báo khi fireball rơi tới
        yield return new WaitForSeconds(1.2f);
        foreach (var w in warnings)
        {
            if (w != null) Destroy(w);
        }
    }

    /// <summary>
    /// Rơi 1 viên fireball từ trên cao xuống spawnpoint.
    /// </summary>
    IEnumerator SpawnAndDropFireball(Transform spawnPoint)
    {
        Vector3 startPos = spawnPoint.position + Vector3.up * fireballFallHeight;
        GameObject fireball = Instantiate(fireballPrefab, startPos, Quaternion.identity);

        fireball.GetComponent<FireballProjectile>().Init(new Vector2(spawnPoint.position.x, spawnPoint.position.y), 6f);

        //while (fireball != null && fireball.transform.position.y > spawnPoint.position.y)
        //{
        //    fireball.transform.position += Vector3.down * fireballFallSpeed * Time.deltaTime;
        //    yield return null;
        //}

        //// 🔹 Snap đúng vị trí spawnPoint
        //if (fireball != null)
        //    fireball.transform.position = spawnPoint.position;

        yield return null;
    }


    // ==========================================
    // 🔁 TRANSFORM BAY -> ĐI BỘ
    // ==========================================
    private bool isTransforming = false;

    IEnumerator TransformToGroundPhase()
    {
        isTransforming = true; // 🔥 Chặn mọi hành động khác

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        agent.enabled = false;

        // Animation transform
        animator.SetTrigger("isTransform");

        yield return new WaitForSeconds(7.4f);

        isGroundedPhase = true;
        health.currentHealth = Mathf.RoundToInt(maxHealthPhase2);
        stats.health = Mathf.RoundToInt(maxHealthPhase2);

        health.RefreshHealthBar(stats.health);


        //yield return new WaitForEndOfFrame();
        if (agent != null && !agent.enabled)
            agent.enabled = true;

        while (!agent.isOnNavMesh)
            yield return null;

        animator.SetBool("isFlying", false);

        isTransforming = false; // ✅ Cho phép hành động trở lại
        currentRoutine = null;

        // 🔥 Khởi động phase mới
        yield return new WaitUntil(() => agent.isOnNavMesh);
        MovementStateControl();
    }


    [SerializeField] GameObject breakApartSwingsRef;

    private void LoseSwings()
    {
        GameObject destructable = Instantiate(breakApartSwingsRef);

        destructable.transform.position = transform.position;

        destructable.transform.localScale = transform.localScale;

     
    }

    void SpawnMinions()
    {
        if (minionPrefab == null || minionSpawnPoints.Count == 0)
            return;

        if (!isGroundedPhase)
        {
            // 🟣 Phase bay → spawn 1 minion ngẫu nhiên
            int randomIndex = Random.Range(0, minionSpawnPoints.Count);
            Transform spawnPoint = minionSpawnPoints[randomIndex];
            Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            // 🟤 Phase đi bộ → spawn 2 minion ở 2 vị trí ngẫu nhiên KHÔNG trùng nhau
            if (minionSpawnPoints.Count == 1)
            {
                Instantiate(minionPrefab, minionSpawnPoints[0].position, Quaternion.identity);
                return;
            }

            // Lấy ngẫu nhiên 2 vị trí khác nhau
            int firstIndex = Random.Range(0, minionSpawnPoints.Count);
            int secondIndex;
            do
            {
                secondIndex = Random.Range(0, minionSpawnPoints.Count);
            } while (secondIndex == firstIndex);

            Instantiate(minionPrefab, minionSpawnPoints[firstIndex].position, Quaternion.identity);
            Instantiate(minionPrefab, minionSpawnPoints[secondIndex].position, Quaternion.identity);
        }
    }

  
    public void DemonEgoColliderOn()
    {
        attackCollider.gameObject.SetActive(true);
    }
    public void DemonEgoColliderOff()
    {
        attackCollider.gameObject.SetActive(false);


    }
}
