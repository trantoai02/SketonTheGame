using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Vector2 roamPosition;
    protected Vector2 moveDir;
    protected Vector2 oldPos;
    protected Vector2 currentPos;
    protected State state;
    protected float timeRoaming = 0f;


    public Transform target;
    public float chaseRange = 5f;
    public float attackRange = 3f;

    public float moveSpeed = 2f;

    public float distanceToTarget;
    public float shorterDis;

    private WaveSpawner waveSpawner;
    public enum ChaseType
    {
        Free,
        Aim
    }

    public ChaseType chaseType;

    protected enum State
    {
        Roaming,
        Chasing,
        Attacking
    }



    protected virtual void Awake()
    {
        Instance = this;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        state = State.Roaming;

        if(Player.instance != null )
            target = Player.instance.transform;
    }

    private void Start()
    {
        moveSpeed = EnemyStats.Instance.speed;

        waveSpawner = GetComponent<WaveSpawner>();


        if (Player.instance != null)
            target = Player.instance.transform;
    }

    protected virtual void Update()
    {
            MovementStateControl();
    }

    protected virtual void MovementStateControl()
    {
       
    }

    protected void UpdateSpriteDirection(Vector2 moveDir)
    {
        if (moveDir.x > 0.2)
        {
           // spriteRenderer.flipX = false;
            transform.localScale = new Vector3(1, 1, 1);

        }
        else if (moveDir.x < 0.2)
        {
            //spriteRenderer.flipX = true;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
