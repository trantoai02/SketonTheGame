using System.Collections;

using UnityEngine;

public class KillerPlant : MonoBehaviour
{


    [SerializeField] private float attackRange = 3f;

    [SerializeField] private MonoBehaviour enemyType;
   

    Animator animator;

    private enum State
    {
        Idle,
        Attacking
    }


    private State state;


    private void Awake()
    {
        animator = GetComponent<Animator>();

        state = State.Idle;
    }


    private void Update()
    {
        MovementStateControl();
    }
    bool isCharging = false;
    private void MovementStateControl()
    {
        if (isCharging)
        {
            state = State.Attacking;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, Player.instance.transform.position);
        switch (state)
        {
            default:
            case State.Idle:
             
                if (distanceToPlayer <= attackRange)
                {
                    state = State.Attacking;
                }
                break;

            case State.Attacking:


                if (distanceToPlayer > attackRange)
                {
                    state = State.Idle;
                }
                
               
                if (!isCharging)
                {
                 
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

                break;
        }


    }

    public float chargingTime = 2f;
    public float attackTime = 1f;


    private IEnumerator Charging()
    {

        //yield return new WaitForSeconds(chargingTime);
        animator.SetBool("isAttack", true);
        animator.SetBool("isCharging", false);
        yield return new WaitForSeconds(attackTime);

        animator.SetBool("isAttack", false);


        chargingTime = 2f;
        attackTime = 1f;
        isCharging = false;

       

    }

    public GameObject killerPlantCollider;

    public void killerPlantColliderOn()
    {
        killerPlantCollider.SetActive(true);
    }

    public void killerPlantColliderOff()
    {
        killerPlantCollider.SetActive(false);

    }

    private void OnDrawGizmos()
    {

        // Vẽ vùng phạm vi Attacking
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
