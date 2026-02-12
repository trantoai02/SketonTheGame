using UnityEngine;

public class PuzzleTile : MonoBehaviour
{
    public int row, col;
    public bool isCorrectStep = false;

    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sr.color = Color.red;

            // Nếu là bước sai → báo PuzzleManager
            if (!isCorrectStep)
            {
                PuzzleManager.Instance.PlayerSteppedWrong();
            }
        }
        else if (collision.CompareTag("PuzzleTag"))
        {
            sr.color = Color.red;
            isCorrectStep = true; // enemy bước qua → đúng
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PuzzleTag"))
        {
            sr.color = originalColor;
        }
    }

    // Hàm để tile phát nổ
    public void Explode(GameObject explosionPrefab)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject); // hoặc tắt if want
    }
}
