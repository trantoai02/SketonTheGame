
using UnityEngine;

public class SquirrelController : MonoBehaviour
{
    public Transform[] wayPoints;
    public int currentWayPointIndex=0;
    public float speed = 5f;
    Animator animator;
    public GameObject starPrefab;
    public SpriteRenderer sr;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")
            /*&& (transform.position == wayPoints[currentWayPointIndex].transform.position)*/)

        {
            if (Vector2.Distance(transform.position, wayPoints[currentWayPointIndex].position) <= 0.1f)
            {
                Debug.Log("cham tui roi");
                if (currentWayPointIndex < wayPoints.Length)
                {

                    if (currentWayPointIndex == 3)
                    {
                        Debug.Log(currentWayPointIndex);
                        if (starPrefab != null)
                        {
                            starPrefab.transform.position = transform.position;
                            starPrefab.SetActive(true);
                        }
                        //else
                        //{
                        //    Instantiate(starPrefab, wayPoints[currentWayPointIndex].gameObject.transform.position, Quaternion.identity);

                        //}
                        // QuestManager.Instance.CompleteQuest("lv1_quest1");
                    }
                    currentWayPointIndex++;

                }


            }
        }
    }
    private void Update()
    {
        if (currentWayPointIndex < wayPoints.Length)
        {
            if (Vector2.Distance(transform.position, wayPoints[currentWayPointIndex].position) > 0.1f)
            {
                Vector2 moveDir = wayPoints[currentWayPointIndex].position - transform.position;
                //rotation

                sr.flipX = (moveDir.x < 0);

                

                transform.position = Vector2.MoveTowards(transform.position, wayPoints[currentWayPointIndex].position, speed * Time.deltaTime);
                if (animator)
                {
                    animator.SetBool("isRun", true);
                }
                else
                {
                    Debug.Log("Khong lay duoc animator");
                }


            }


            // đã chạm đến way point
            else
            {
                if(currentWayPointIndex == wayPoints.Length-1)
                {
                    Destroy(gameObject);
                }
                
                animator.SetBool("isRun", false);

            }
        }
    }
    
}
