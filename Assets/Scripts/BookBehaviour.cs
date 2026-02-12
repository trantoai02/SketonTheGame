using UnityEngine;

public class BookBehaviour : MonoBehaviour
{
    [Header("Prefab xuất hiện khi animation kết thúc")]
    public GameObject egoDemonPrefab;

    // Gọi hàm này ở frame cuối cùng của animation quyển sách
    public void OnBookAnimationEnd()
    {
        // Lưu vị trí hiện tại
        Vector3 spawnPos = transform.position;

        // Ẩn hoặc destroy quyển sách (tùy bạn)
        gameObject.SetActive(false);
        // hoặc: Destroy(gameObject);

        // Spawn EgoDemon
        if (egoDemonPrefab != null)
        {
            if (egoDemonPrefab != null)
            {
                egoDemonPrefab.transform.position = spawnPos;
                egoDemonPrefab.SetActive(true);
                CameraSequenceController.Instance.FollowEgoDemon();
            }
           // Instantiate(egoDemonPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("EgoDemon Prefab chưa được gán trong BookBehaviour!");
        }
    }
}
