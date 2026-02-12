using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpiderWeb : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject keyPromptImage;
    public GameObject progressFrame;
    public Image progressFillImage;

    [Header("Mash Settings")]
    public float maxProgress = 100f;
    public float progressPerMash = 8f;
    public float decayPerSecond = 25f;
    public float effectDuration = 5f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Events")]
    public UnityEvent onWebDestroyed;

    private float currentProgress = 0f;
    private bool isPlayerStuck = false;
    private bool hasCompleted = false;

    private GameObject stuckPlayer;
    private Player playerScript;

    private Vector2 targetPos;
    private bool isFlying = false;
    private bool hasLanded = false;

    public bool webbing = false;

    private Spider spiderOwner; // tham chiếu Spider gốc

    public void MoveTo(Vector2 destination, Spider spider)
    {
        targetPos = destination;
        isFlying = true;
        spiderOwner = spider;
    }

    private void Start()
    {
        if (keyPromptImage != null) keyPromptImage.SetActive(false);
        if (progressFrame != null) progressFrame.SetActive(false);
        if (progressFillImage != null) progressFillImage.fillAmount = 0f;
    }

    private void ResetWebState()
    {
        isPlayerStuck = false;
        stuckPlayer = null;
        playerScript = null;

        currentProgress = 0f;
        hasCompleted = false;

        if (keyPromptImage != null) keyPromptImage.SetActive(false);
        if (progressFrame != null) progressFrame.SetActive(false);
        if (progressFillImage != null) progressFillImage.fillAmount = 0f;
    }
    private void Update()
    {
        if (isFlying && !hasLanded)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPos) < 0.05f)
            {
                LandAtPosition(targetPos);
            }

            return;
        }

        if (!isPlayerStuck || hasCompleted) return;

        // ❌ Chặn di chuyển
        if (playerScript != null)
        {
            // Nếu player đã chết, reset trạng thái dính
            if (PlayerHealth.Instance != null && PlayerHealth.Instance.isDead)
            {
                ResetWebState();
                return;
            }

            playerScript.moveVector = Vector2.zero;
        }



        // ✅ Nhấn E để phá
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (progressFrame != null && !progressFrame.activeSelf)
            {
                if (keyPromptImage != null) keyPromptImage.SetActive(false);
                progressFrame.SetActive(true);
            }

            currentProgress += progressPerMash;
            currentProgress = Mathf.Min(currentProgress, maxProgress);
        }

        // ✅ Hoàn thành
        if (currentProgress >= maxProgress)
        {
            hasCompleted = true;

            if (progressFillImage != null) progressFillImage.fillAmount = 1f;
            if (keyPromptImage != null) keyPromptImage.SetActive(false);
            if (progressFrame != null) progressFrame.SetActive(false);
            if (progressFillImage != null) progressFillImage.gameObject.SetActive(false);

            onWebDestroyed?.Invoke();
            Destroy(gameObject);
            return;
        }

        // ⏬ Giảm dần
        if (progressFrame != null && progressFrame.activeSelf)
        {
            currentProgress -= decayPerSecond * Time.deltaTime;
            currentProgress = Mathf.Max(currentProgress, 0f);

            if (progressFillImage != null)
                progressFillImage.fillAmount = currentProgress / maxProgress;

            if (currentProgress <= 0f && keyPromptImage != null)
            {
                progressFrame.SetActive(false);
                keyPromptImage.SetActive(true);
            }
        }
    }

    private void LandAtPosition(Vector2 position)
    {
        isFlying = false;
        hasLanded = true;
        transform.position = position;
        webbing = true;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetBool("webbing", true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isPlayerStuck || hasCompleted) return;

        if (collision.CompareTag("Player"))
        {
            stuckPlayer = collision.gameObject;
            playerScript = stuckPlayer.GetComponentInParent<Player>();
            isPlayerStuck = true;
            currentProgress = 0f;

            if (keyPromptImage != null)
                keyPromptImage.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu đang bay và chạm player → đổi hướng dừng
        if (isFlying && !hasLanded && collision.CompareTag("Player"))
        {
            Vector2 hitPos = collision.ClosestPoint(transform.position);
            LandAtPosition(hitPos);

            // Báo ngược lại cho Spider
            if (spiderOwner != null)
                spiderOwner.OnWebHitPlayer(hitPos);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            ResetWebState();
    }
}
