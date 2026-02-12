using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class MatrixCell : MonoBehaviour
{
    public bool wasVisited = false;
    public bool isStartCell = false;
    public bool isEndCell = false;

    private SpriteRenderer sr;
    private Color originalColor;
    public Color visitedColor = Color.yellow;
    public Color wrongColor = Color.red;

    public PathMatrixController controller;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        // Tìm controller của puzzle này
        //controller = GetComponent<PathMatrixController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Chưa visit → OK
        if (!wasVisited)
        {
            wasVisited = true;
            sr.color = visitedColor;

            controller.PlayerSteppedOnCell(this);
            controller.SetCurrentCell(this);
            return;
        }

        // Đã visit lần 2 → fail toàn bộ
        sr.color = wrongColor;
        controller.FailPuzzle();
    }

    public void ResetCell()
    {
        wasVisited = false;
        if (sr != null) sr.color = originalColor;
    }

    public void Explode(GameObject explosionPrefab)
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
