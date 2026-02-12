using UnityEngine;
using Pathfinding;

public class AllyAI : MonoBehaviour
{
    public GameObject magicLaser;
    public Transform laserSpawnPoint;

    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    public float fireRate = 1f;
    private float fireCooldown = 0f;

    public float laserRange = 15f; // Phạm vi tìm enemy để bắn

    public Transform enemyGFX;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, .25f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath((Vector3)rb.position, target.position, OnPathCompleted);
    }

    void OnPathCompleted(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > speed)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }

        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (rb.velocity.x >= 0.1f)
        {
            enemyGFX.localScale = new Vector3(-Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, 1);
        }
        else if (rb.velocity.x <= -0.1f)
        {
            enemyGFX.localScale = new Vector3(Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, 1);
        }
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f && IsEnemyInRange())
        {
            SpawnLaser();
            fireCooldown = 1f / fireRate;
        }
    }

    bool IsEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= laserRange)
                return true;
        }
        return false;
    }

    public void SpawnLaser()
    {
        GameObject newLaser = Instantiate(magicLaser, laserSpawnPoint.position, Quaternion.identity);

        MagicLaser laserScript = newLaser.GetComponent<MagicLaser>();
        if (laserScript != null)
        {
            laserScript.SetTarget(Vector3.zero); // Bên MagicLaser tự chọn mục tiêu phù hợp
        }
    }
}
