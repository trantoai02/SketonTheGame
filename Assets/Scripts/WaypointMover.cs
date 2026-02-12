using UnityEngine;

public class WaypointMover : Enemy
{
    [Header("Waypoint Settings")]
    public Transform[] waypoints;
    public float[] speeds;

    [Header("Target to chase")]
    public Transform player;

    private int currentIndex = 0;
    private bool chasingPlayer = false;

    void Update()
    {
        if (player == null || waypoints.Length == 0) return;

        // Kiểm tra nếu boss đã bằng hoặc vượt vị trí player theo trục X
        if (!chasingPlayer && transform.position.x >= player.position.x)
        {
            chasingPlayer = true;
        }

        if (chasingPlayer)
        {
            // Rượt player
            float speed = (currentIndex < speeds.Length) ? speeds[currentIndex] : speeds[speeds.Length - 1];
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else
        {
            // Di chuyển theo waypoint
            Transform target = waypoints[currentIndex];
            float speed = (currentIndex < speeds.Length) ? speeds[currentIndex] : speeds[speeds.Length - 1];
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < 0.05f)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = waypoints.Length - 1; // hoặc giữ nguyên, tùy bạn muốn boss đứng lại hay sao
                }
            }
        }
    }
}
