using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaypointChaser : MonoBehaviour
{
    [Header("Các waypoint cần chạy tới (theo thứ tự)")]
    public Transform[] waypoints;

    [Header("Tốc độ di chuyển")]
    public float moveSpeed = 3f;

    [Header("Tag của Player")]
    public string playerTag = "Player";

    private int currentIndex = 0;
    private bool isMoving = false;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            currentIndex = 1;
        }

        // Đảm bảo collider là trigger
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Nếu chưa chạy và vẫn còn waypoint thì bắt đầu di chuyển
            if (!isMoving && currentIndex < waypoints.Length)
            {
                StartCoroutine(MoveToNextWaypoint());
            }
        }
    }

    IEnumerator MoveToNextWaypoint()
    {
        yield return new WaitForSeconds(1f);

        isMoving = true;

        Vector3 target = waypoints[currentIndex].position;

        // Chạy tới waypoint tiếp theo
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            // Nếu có Animator thì bật trạng thái chạy
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null) anim.SetBool("isRun", true);

            yield return null;
        }

        // Tắt animation chạy khi đến nơi
        Animator endAnim = GetComponentInChildren<Animator>();
        if (endAnim != null) endAnim.SetBool("isRun", false);

        // Chờ lần trigger tiếp theo để đi tiếp
        currentIndex++;
        isMoving = false;

        if (currentIndex >= waypoints.Length)
        {
            Debug.Log("🎉 Đã đến waypoint cuối cùng!");
        }
    }
}
