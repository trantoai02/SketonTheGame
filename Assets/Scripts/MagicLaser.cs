using UnityEngine;

public class MagicLaser : MonoBehaviour
{
    public float speed = 10f;
    public float range = 15f;

    private GameObject targetEnemy;
    private bool hasTarget = false;

    public void SetTarget(Vector3 _)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
        {
            Debug.LogWarning("Không tìm thấy enemy nào.");
            return;
        }

        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        // Ưu tiên enemy gần player và trong phạm vi của viên đạn
        if (player != null)
        {
            foreach (GameObject enemy in enemies)
            {
                float distanceToPlayer = Vector3.Distance(player.transform.position, enemy.transform.position);
                float distanceToBullet = Vector3.Distance(transform.position, enemy.transform.position);

                if (distanceToPlayer < shortestDistance && distanceToBullet <= range)
                {
                    shortestDistance = distanceToPlayer;
                    closestEnemy = enemy;
                }
            }
        }

        // Nếu chưa có target, tìm enemy gần viên đạn
        if (closestEnemy == null)
        {
            shortestDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies)
            {
                float distanceToBullet = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToBullet < shortestDistance && distanceToBullet <= range)
                {
                    shortestDistance = distanceToBullet;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;
            hasTarget = true;
        }
        else
        {
            Debug.Log("Không có enemy trong range.");
        }
    }

    void Update()
    {
        if (!hasTarget || targetEnemy == null)
        {
            Destroy(gameObject); // Nếu mất target (enemy chết), tự hủy viên đạn
            return;
        }

        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetEnemy.transform.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
