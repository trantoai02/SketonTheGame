using UnityEngine;

public class ShockwaveDamage : MonoBehaviour
{
    [Header("Shockwave Settings")]
    public float expandSpeed = 8f;       // Tốc độ lan rộng
    public int damage = 20;              // Sát thương
    public float lifetime = 1.2f;        // Thời gian tồn tại
    public float hitThickness = 0.25f;   // Độ dày vùng "rìa" gây damage

    private float currentScaleFactor = 0.1f;
    private float timer = 0f;
    private bool hasHitPlayer = false;

    private SpriteRenderer sr;
    private Vector3 originalScale;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale; // 🔹 lưu scale ban đầu (ví dụ 2x1)

        // Bắt đầu rất nhỏ nhưng vẫn giữ tỉ lệ ellipse
        transform.localScale = originalScale * currentScaleFactor;
    }

    private void Update()
    {
        ExpandShockwave();
        CheckHitPlayer();

        timer += Time.deltaTime;
        if (timer >= lifetime)
            Destroy(gameObject);
    }

    private void ExpandShockwave()
    {
        // 🔹 tăng factor mở rộng (không động đến originalScale)
        currentScaleFactor += expandSpeed * Time.deltaTime;

        // 🔹 scale theo factor này, giữ nguyên tỉ lệ x:y ban đầu
        transform.localScale = originalScale * currentScaleFactor;
    }

    private void CheckHitPlayer()
    {
        if (hasHitPlayer || Player.instance == null) return;

        float dist = Vector2.Distance(transform.position, Player.instance.transform.position);

        // 🔹 Bán kính dựa trên scale trung bình
        float approxRadius = (transform.localScale.x + transform.localScale.y) * 0.25f;

        if (dist >= approxRadius - hitThickness && dist <= approxRadius + hitThickness)
        {
            if (!Player.instance.isRolling)
            {
                if (PlayerHealth.Instance != null && !PlayerHealth.Instance.isDead)
                {
                    PlayerHealth.Instance.TakeDamage(damage, transform);
                    hasHitPlayer = true;
                }
            }
        }
    }
}
