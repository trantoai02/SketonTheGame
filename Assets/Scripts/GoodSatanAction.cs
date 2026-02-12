using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class GoodSatanAction : MonoBehaviour
{
    public bool isPinned = false;
    public float moveSpeed = 2f;
    public float roamRadius = 3f;
    public GameObject gfx;
    public Transform target; // đối tượng để chase

    private Animator animator;
    private NavMeshAgent agent;
    private Coroutine currentCoroutine;

    void Awake()
    {
        animator = gfx.GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // 2D top-down setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
    }

    void Start()
    {
        if (isPinned)
            BeingAttack();
        else
            Run();
    }

    void Update()
    {
        // Ép Z luôn = 0 để agent di chuyển trên XY plane
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;

        // cập nhật hướng sprite theo velocity của agent
        Vector2 moveDir = new Vector2(agent.velocity.x, agent.velocity.y);
        if (moveDir.magnitude > 0.01f)
        {
            UpdateSpriteDirection(moveDir);
        }
    }

    protected void UpdateSpriteDirection(Vector2 moveDir)
    {
        if (moveDir.x > 0.01f)
            gfx.transform.localScale = new Vector3(1, 1, 1);
        else if (moveDir.x < -0.01f)
            gfx.transform.localScale = new Vector3(-1, 1, 1);
    }

    #region States

    public void Roam()
    {
        StopCurrentCoroutine();
        currentCoroutine = StartCoroutine(RoamLoop());
    }

    public void Run()
    {
        StopCurrentCoroutine();
        currentCoroutine = StartCoroutine(RunLoop());
    }

    public void Idle()
    {
        StopCurrentCoroutine();
        animator.SetBool("isRun", false);
      

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    public void BeingAttack()
    {
        if (!isPinned) return;

        StopCurrentCoroutine();
       
        animator.SetBool("isPinned", true);

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    #endregion

    #region Coroutines

    private IEnumerator RoamLoop()
    {
        while (true)
        {
            Vector3 randomTarget = GetRandomPosition(transform.position, roamRadius);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomTarget, out hit, 1f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                agent.isStopped = false;
                animator.SetBool("isRun", true);

                while (Vector3.Distance(transform.position, agent.destination) > 0.1f)
                    yield return null;
            }

            animator.SetBool("isRun", false);
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator RunLoop()
    {
            animator.SetBool("isRun", true);

        animator.SetBool("isPinned", false);
        agent.isStopped = false;

        while (true)
        {
            Vector3 randomTarget = GetRandomPosition(transform.position, roamRadius);

            if (agent != null && agent.isOnNavMesh && randomTarget != null)
            {
                // targetPosition tự động lấy vị trí của target
                
                agent.SetDestination(randomTarget);
            }
            yield return new WaitForSeconds(1f);

        }
    }

    #endregion

    private Vector3 GetRandomPosition(Vector3 center, float radius)
    {
        Vector2 randomDir = Random.insideUnitCircle * radius;
        return center + new Vector3(randomDir.x, randomDir.y, 0);
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }
}
