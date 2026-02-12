using UnityEngine;
using UnityEngine.UI;
public class CameraSwipeMove : MonoBehaviour
{
    [Header("ScrollRect chứa các level")]
    public ScrollRect scrollRect;

    [Header("Giới hạn X cho camera")]
    public float minX = -10f;
    public float maxX = 10f;

    [Header("Mức độ parallax, 1 = cùng tốc độ scroll")]
    [Range(0f, 1f)]
    public float parallaxAmount = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (scrollRect == null) return;

        // horizontalNormalizedPosition: 0 = đầu, 1 = cuối
        float t = scrollRect.horizontalNormalizedPosition;

        // Chuyển thành vị trí X
        float targetX = Mathf.Lerp(minX, maxX, t) * parallaxAmount;

        transform.position = new Vector3(startPos.x + targetX, startPos.y, startPos.z);
    }
}
